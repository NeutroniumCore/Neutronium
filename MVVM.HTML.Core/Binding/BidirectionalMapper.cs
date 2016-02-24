using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.Binding.Listeners;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core.Binding
{
    public class BidirectionalMapper : IDisposable, IVisitable, IJavascriptToCSharpConverter, IJavascriptChangesObserver   
    {
        private readonly HTMLViewContext _Context;        
        private readonly JavascriptBindingMode _BindingMode;
        private readonly CSharpToJavascriptConverter _JSObjectBuilder;
        private readonly IJavascriptSessionInjector _sessionInjector;
        private readonly SessionCacher _SessionCache;
        private readonly IJSCSGlue _Root;
        private readonly FullListenerRegister _ListenerRegister;
        private readonly List<IJSCSGlue> _UnrootedEntities= new List<IJSCSGlue>();
        private bool _IsListening = false;

        public IJSCSGlue JSValueRoot { get { return _Root; } }
        public bool ListenToCSharp { get { return (_BindingMode != JavascriptBindingMode.OneTime); } }
        public JavascriptBindingMode Mode { get { return _BindingMode; } }

        internal BidirectionalMapper(object iRoot, HTMLViewContext context, JavascriptBindingMode iMode, object iadd)
        { 
            _Context = context;  
            _BindingMode = iMode; 
            var javascriptObjecChanges = (iMode == JavascriptBindingMode.TwoWay) ? (IJavascriptChangesObserver)this : null;
            _sessionInjector = _Context.CreateInjector(javascriptObjecChanges);    
            _SessionCache = new SessionCacher();
            _ListenerRegister = new FullListenerRegister(
                                        (n) => n.PropertyChanged += CSharpPropertyChanged,
                                        (n) => n.PropertyChanged -= CSharpPropertyChanged,
                                        (n) => n.CollectionChanged += CSharpCollectionChanged,
                                        (n) => n.CollectionChanged -= CSharpCollectionChanged,
                                        (c) => c.ListenChanges(),
                                        (c) => c.UnListenChanges());
            var commandFactory = new CommandFactory(context, this);
            RegisterJavascriptHelper();
            _JSObjectBuilder = new CSharpToJavascriptConverter(_Context, _SessionCache, commandFactory) ;
            _Root = _JSObjectBuilder.Map(iRoot, iadd); 
        }

        private void RegisterJavascriptHelper()
        {
            IJavascriptObject res;
            var resource = new ResourceReader("MVVM.HTML.Core.scripts", this);
            _Context.WebView.Eval(resource.Load("Infra.js"), out res);
        }


        private async Task RunInJavascriptContext(Action Run)
        {
            await _Context.WebView.RunAsync(Run);
        }

        internal async Task Init()
        {
            var res = await InjectInHTLMSession(_Root);

            await _sessionInjector.RegisterMainViewModel(res);

            await RunInJavascriptContext(() =>
                  {
                      if (ListenToCSharp)
                      {
                          ListenToCSharpChanges();
                      }
                      _IsListening = true;
                  });
        }

        public void Dispose()
        {
            if (ListenToCSharp)
                UnlistenToCSharpChanges();

            _sessionInjector.Dispose();
            _UnrootedEntities.Clear();
        }

        public void Visit(IListenableObjectVisitor visitor)
        {
            _Root.ApplyOnListenable(visitor);
            _UnrootedEntities.ForEach(js => js.ApplyOnListenable(visitor));
        }

        private void ListenToCSharpChanges()
        {
            Visit(_ListenerRegister.GetOn());
        }

        private void UnlistenToCSharpChanges()
        {
            Visit(_ListenerRegister.GetOff());
        }

        private async Task<IJavascriptObject> InjectInHTLMSession(IJSCSGlue iroot)
        {
            if (iroot == null)
                return null;

            switch (iroot.Type)
            {
                case JSCSGlueType.Basic:
                    return null;

                case JSCSGlueType.Object:
                    if ((iroot.JSValue.IsNull))
                        return null;
                    break;
            }

            var jvm = _SessionCache.GetMapper(iroot as IJSObservableBridge);
            var res = _sessionInjector.Inject(iroot.JSValue, jvm);

            if ((iroot.CValue != null) && ((res==null) || (res.IsUndefined)))
            {
                throw ExceptionHelper.GetUnexpected();
            }

            await jvm.UpdateTask;
            return res;
        }

        public void OnJavaScriptObjectChanges(IJavascriptObject objectchanged, string propertyName, IJavascriptObject newValue)
        {
            try
            {
                var res = _SessionCache.GetGlobalCached(objectchanged) as JSGenericObject;
                if (res == null)
                    return;

                var propertyAccessor = new PropertyAccessor(res.CValue, propertyName);
                if (!propertyAccessor.IsSettable)
                    return;

                var targetType = propertyAccessor.GetTargetType();
                var glue = GetCachedOrCreateBasic(newValue, targetType);

                using (_IsListening ? _ListenerRegister.GetPropertySilenter(res.CValue) : null)
                {
                    propertyAccessor.Set(glue.CValue);
                    res.UpdateCSharpProperty(propertyName, glue);
                }
            }
            catch (Exception e)
            {
                ExceptionHelper.Log(string.Format("Unable to update ViewModel from View, exception raised: {0}", e));
            }
        }

        public void OnJavaScriptCollectionChanges(JavascriptCollectionChanges changes)
        {
            try
            {
                var res = _SessionCache.GetGlobalCached(changes.Collection) as JSArray;
                if (res == null) return;

                CollectionChanges.CollectionChanges cc = res.GetChanger(changes, this);

                using (ReListen()) 
                using (_ListenerRegister.GetColllectionSilenter(res.CValue))
                {
                    res.UpdateEventArgsFromJavascript(cc);
                }
            }
            catch (Exception e)
            {
                ExceptionHelper.Log(string.Format("Unable to update ViewModel from View, exception raised: {0}", e));
            }
        }

        private async void CSharpPropertyChanged(object sender, PropertyChangedEventArgs e)
        { 
            var pn = e.PropertyName;
            var propertyAccessor = new PropertyAccessor(sender, pn);
            if (!propertyAccessor.IsGettable)
                return;

            var currentfather = _SessionCache.GetCached(sender) as JSGenericObject;
            if (currentfather == null) 
                return;

            var nv = propertyAccessor.Get();
            var oldbridgedchild = currentfather.Attributes[pn];

            if (Object.Equals(nv, oldbridgedchild.CValue))
                return;

            var newbridgedchild = _JSObjectBuilder.Map(nv);
            await RegisterAndDo(newbridgedchild, () => currentfather.ReRoot(pn, newbridgedchild) );
        }

        private async void CSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await RunInJavascriptContext(async () =>
            {
                await UnsafeCSharpCollectionChanged(sender, e);
            });
        }

        private async Task UnsafeCSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var arr = _SessionCache.GetCached(sender) as JSArray;
            if (arr == null)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var addvalue = _JSObjectBuilder.Map(e.NewItems[0]);

                    if (addvalue == null) return;

                    await RegisterAndDo(addvalue, () => arr.Add(addvalue, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    var newvalue = _JSObjectBuilder.Map(e.NewItems[0]);

                    if (newvalue == null) return;

                    await RegisterAndDo(newvalue, () => arr.Replace(newvalue, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    await RegisterAndDo(() => arr.Remove(e.OldStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Reset:
                    await RegisterAndDo(() => arr.Reset());
                    break;

                case NotifyCollectionChangedAction.Move:
                    arr.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
            }
        }

        public async Task<IJSCSGlue> RegisterInSession(object nv)
        {
            var newbridgedchild = _JSObjectBuilder.Map(nv);
            await RegisterAndDo(newbridgedchild, () => { _UnrootedEntities.Add(newbridgedchild); });
            return newbridgedchild;
        }

        private async Task RegisterAndDo(Action Do)
        {
            await RunInJavascriptContext(() =>
            {
                using (ReListen())
                {
                    Do();
                }
            } );
        }

        private async Task RegisterAndDo(IJSCSGlue ivalue, Action Do)
        {
            var idisp = ReListen();
            await InjectInHTLMSession(ivalue);
            await RunInJavascriptContext(() =>
                    {
                        using (idisp)
                        {
                            Do();
                        }
                    }
               );
        }

        private ReListener _ReListen = null;
        private IDisposable ReListen()
        {
            return (_ReListen != null) ? _ReListen.AddRef() :
                        _ReListen = new ReListener(this, _ListenerRegister, () => _ReListen = null);
        }

        public IJSCSGlue GetCachedOrCreateBasic(IJavascriptObject globalkey, Type iTargetType)
        {
            return _Context.WebView.Evaluate(() => GetCachedOrCreateBasicUnsafe(globalkey, iTargetType));
        }

        private IJSCSGlue GetCachedOrCreateBasicUnsafe(IJavascriptObject globalkey, Type targetType) {
            if (globalkey == null)
                return null;

            //Use local cache for objet not created in javascript session such as enum
            var res = _SessionCache.GetGlobalCached(globalkey) ?? _SessionCache.GetCachedLocal(globalkey);
            if (res!=null)
                return res;

            object targetvalue;
            bool converted = _Context.WebView.Converter.GetSimpleValue(globalkey, out targetvalue, targetType);
            if ((!converted) && (!globalkey.IsNull) && (!globalkey.IsUndefined))
                throw ExceptionHelper.Get(string.Format("Unable to convert javascript object: {0}", globalkey));

            return new JSBasicObject(globalkey, targetvalue);
        }
    }
}
