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
        private readonly IWebSessionLogger _Logger;
        private readonly JavascriptBindingMode _BindingMode;
        private readonly CSharpToJavascriptConverter _JSObjectBuilder;
        private readonly IJavascriptSessionInjector _sessionInjector;
        private readonly SessionCacher _SessionCache;
        private readonly IJSCSGlue _Root;
        private readonly FullListenerRegister _ListenerRegister;
        private readonly List<IJSCSGlue> _UnrootedEntities= new List<IJSCSGlue>();
        private bool _IsListening = false;

        public IJSCSGlue JSValueRoot => _Root;
        public bool ListenToCSharp => (_BindingMode != JavascriptBindingMode.OneTime);
        public JavascriptBindingMode Mode => _BindingMode;
        public HTMLViewContext Context => _Context;

        internal BidirectionalMapper(object iRoot, HTMLViewEngine contextBuilder, JavascriptBindingMode iMode, object addicionalObject, IWebSessionLogger logger)
        {        
            _BindingMode = iMode;
            _Logger = logger;
            var javascriptObjecChanges = (iMode == JavascriptBindingMode.TwoWay) ? (IJavascriptChangesObserver)this : null;
            _Context = contextBuilder.GetMainContext(javascriptObjecChanges);
            _sessionInjector = _Context.JavascriptSessionInjector;  
            _SessionCache = new SessionCacher();
            _ListenerRegister = new FullListenerRegister(
                                        (n) => n.PropertyChanged += CSharpPropertyChanged,
                                        (n) => n.PropertyChanged -= CSharpPropertyChanged,
                                        (n) => n.CollectionChanged += CSharpCollectionChanged,
                                        (n) => n.CollectionChanged -= CSharpCollectionChanged,
                                        (c) => c.ListenChanges(),
                                        (c) => c.UnListenChanges());
            var commandFactory = new CommandFactory(_Context, this);
            RegisterJavascriptHelper();
            _JSObjectBuilder = new CSharpToJavascriptConverter(_Context, _SessionCache, commandFactory, _Logger) ;
            _Root = _JSObjectBuilder.Map(iRoot, addicionalObject); 
        }

        private void RegisterJavascriptHelper()
        {
            IJavascriptObject res;
            var resource = new ResourceReader("MVVM.HTML.Core.scripts", this);
            _Context.WebView.Eval(resource.Load("Infra.js"), out res);
        }

        private async Task RunInJavascriptContext(Action run)
        {
            await _Context.WebView.RunAsync(run);
        }

        private Task RunInJavascriptContext(Func<Task> run)
        {
            return _Context.WebView.Evaluate(run);
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

            _Context.Dispose();
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

            if ((iroot.CValue != null) && (res==null))
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

                var propertyAccessor = new PropertyAccessor(res.CValue, propertyName, _Logger);
                if (!propertyAccessor.IsSettable)
                {
                    _Logger.Info(() => $"Unable to set C# from javascript object: property: {propertyName} is readonly.");
                    return;
                }
               

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
                _Logger.Error(() =>$"Unable to update ViewModel from View, exception raised: {e.Message}");
            }
        }

        public void OnJavaScriptCollectionChanges(JavascriptCollectionChanges changes)
        {
            try
            {
                var res = _SessionCache.GetGlobalCached(changes.Collection) as JSArray;
                if (res == null) return;

               var collectionChanges = res.GetChanger(changes, this);

                using (ReListen()) 
                using (_ListenerRegister.GetColllectionSilenter(res.CValue))
                {
                    res.UpdateEventArgsFromJavascript(collectionChanges);
                }
            }
            catch (Exception e)
            {
                _Logger.Error(() =>$"Unable to update ViewModel from View, exception raised: {e.Message}");
            }
        }

        private async void CSharpPropertyChanged(object sender, PropertyChangedEventArgs e)
        { 
            var pn = e.PropertyName;
            var propertyAccessor = new PropertyAccessor(sender, pn, _Logger);
            if (!propertyAccessor.IsGettable)
                return;

            var currentfather = _SessionCache.GetCached(sender) as JSGenericObject;
            if (currentfather == null) 
                return;

            var nv = propertyAccessor.Get();
            var oldbridgedchild = currentfather.Attributes[pn];

            if (Object.Equals(nv, oldbridgedchild.CValue))
                return;

            await RegisterAndDo(() => _JSObjectBuilder.UnsafelMap(nv), (child) => currentfather.ReRoot(pn, child) );
        }

        private async void CSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await UnsafeCSharpCollectionChanged(sender, e);
        }

        private async Task UnsafeCSharpCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var arr = _SessionCache.GetCached(sender) as JSArray;
            if (arr == null)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    await RegisterAndDo(() => _JSObjectBuilder.UnsafelMap(e.NewItems[0]), (addvalue) => arr.Add(addvalue, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    await RegisterAndDo(() => _JSObjectBuilder.UnsafelMap(e.NewItems[0]), (newvalue) => arr.Replace(newvalue, e.NewStartingIndex));
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
            return await RegisterAndDo(() => _JSObjectBuilder.UnsafelMap(nv), (newbridgedchild) => { _UnrootedEntities.Add(newbridgedchild); });
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

        private async Task<IJSCSGlue> RegisterAndDo(Func<IJSCSGlue> valueBuilder, Action<IJSCSGlue> Do)
        {
            IJSCSGlue value=null;

            await RunInJavascriptContext(async () =>
            {
                value = valueBuilder();
                if (value!=null)
                    await InjectInHTLMSession(value);
            });

            if (value == null)
                return null;

            await RunInJavascriptContext(() =>
            {
                using (ReListen())
                {
                    Do(value);
                }
            });

            return value;
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

        private IJSCSGlue GetCachedOrCreateBasicUnsafe(IJavascriptObject globalkey, Type targetType)
        {
            if (globalkey == null)
                return null;

            //Use local cache for objet not created in javascript session such as enum
            var res = _SessionCache.GetGlobalCached(globalkey) ?? _SessionCache.GetCachedLocal(globalkey);
            if (res != null)
                return res;

            object targetvalue = null;
            bool converted = _Context.WebView.Converter.GetSimpleValue(globalkey, out targetvalue, targetType);
            if ((!converted) && (!globalkey.IsNull) && (!globalkey.IsUndefined))
            {
                var message = $"Unable to convert javascript object: {globalkey} to C# session. Value will be default to null. Please check javascript bindings.";
                _Logger.Info(message);
                throw new ArgumentException(message);
            }

            return new JSBasicObject(globalkey, targetvalue);
        }
    }
}
