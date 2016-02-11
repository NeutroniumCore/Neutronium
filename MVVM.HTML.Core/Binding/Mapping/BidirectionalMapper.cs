using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Exceptions;

using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.Binding.Listeners;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class BidirectionalMapper : IDisposable, IVisitable, IJavascriptToCSharpConverter, IJavascriptChangesListener   
    {
        private readonly HTMLViewContext _Context;        
        private readonly JavascriptBindingMode _BindingMode;
        private readonly CSharpToJavascriptConverter _JSObjectBuilder;
        private readonly IJavascriptSessionInjector _sessionInjector;
        private readonly SessionCacher _SessionCache;
        private readonly IJSCSGlue _Root;
        private readonly FullListenerRegister _ListenerRegister;
        private readonly List<IJSCSGlue> _UnrootedEntities;
        private bool _IsListening = false;

        public IJSCSGlue JSValueRoot { get { return _Root; } }
        public bool ListenToCSharp { get { return (_BindingMode != JavascriptBindingMode.OneTime); } }

        internal BidirectionalMapper(object iRoot, HTMLViewContext context, JavascriptBindingMode iMode, object iadd)
        {
            _ListenerRegister = new FullListenerRegister(
                                        (n) => n.PropertyChanged += Object_PropertyChanged,
                                        (n) => n.PropertyChanged -= Object_PropertyChanged,
                                        (n) => n.CollectionChanged += CollectionChanged,
                                        (n) => n.CollectionChanged -= CollectionChanged,
                                        (c) => c.ListenChanges(),
                                        (c) => c.UnListenChanges());
            _Context = context;
            _SessionCache = new SessionCacher();
            _JSObjectBuilder = new CSharpToJavascriptConverter(_Context, _SessionCache, new CommandFactory(this));
            _Root = _JSObjectBuilder.Map(iRoot, iadd);
            _UnrootedEntities = new List<IJSCSGlue>();
            _BindingMode = iMode;

             var javascriptObjecChanges = (iMode == JavascriptBindingMode.TwoWay) ? (IJavascriptChangesListener)this : null;
            _sessionInjector = _Context.CreateInjector(javascriptObjecChanges);
        }

        internal async Task Init()
        {
            await InjectInHTLMSession(_Root, true);

            await _Context.WebView.RunAsync(() =>
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

        private async Task InjectInHTLMSession(IJSCSGlue iroot, bool isroot = false)
        {
            if (iroot == null)
                return;

            switch (iroot.Type)
            {
                case JSCSGlueType.Basic:
                    return;

                case JSCSGlueType.Object:
                    if ((iroot.JSValue.IsNull))
                        return;
                    break;
            }

            var jvm = _SessionCache.GetMapper(iroot as IJSObservableBridge);
            var res = _sessionInjector.Inject(iroot.JSValue, jvm, (iroot.CValue != null));

            await jvm.UpdateTask;

            if (isroot)
                await _sessionInjector.RegisterMainViewModel(res);
        }

        public void OnJavaScriptObjectChanges(IJavascriptObject objectchanged, string PropertyName, IJavascriptObject newValue)
        {
            try
            {
                var res = _SessionCache.GetGlobalCached(objectchanged) as JSGenericObject;
                if (res == null)
                    return;

                var propertyAccessor = new PropertyAccessor(res.CValue, PropertyName);
                if (!propertyAccessor.IsSettable)
                    return;

                var targetType = propertyAccessor.GetTargetType();
                var glue = GetCachedOrCreateBasic(newValue, targetType);

                using (_IsListening ? _ListenerRegister.GetPropertySilenter(res.CValue) : null)
                {
                    propertyAccessor.Set(glue.CValue);
                    res.UpdateCSharpProperty(PropertyName, glue);
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

                CollectionChanges cc = res.GetChanger(changes, this);

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

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
            RegisterAndDo(newbridgedchild, () => currentfather.Reroot(pn, newbridgedchild));
        }

        public void RegisterInSession(object nv, Action<IJSCSGlue> Continue)
        {
            var newbridgedchild = _JSObjectBuilder.Map(nv);
            RegisterAndDo(newbridgedchild, () => { _UnrootedEntities.Add(newbridgedchild); Continue(newbridgedchild); });
        }

        private ReListener _ReListen = null;
        private IDisposable ReListen()
        {
            return (_ReListen != null) ?  _ReListen.AddRef() :        
                        _ReListen = new ReListener(this, _ListenerRegister, () => _ReListen=null);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _Context.WebView.RunAsync(() =>
            {
                UnsafeCollectionChanged(sender, e);
            });
        }

        private void UnsafeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var arr = _SessionCache.GetCached(sender) as JSArray;
            if (arr == null)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var addvalue = _JSObjectBuilder.Map(e.NewItems[0]);

                    if (addvalue == null) return;

                    RegisterAndDo(addvalue, () => arr.Add(addvalue, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    var newvalue = _JSObjectBuilder.Map(e.NewItems[0]);

                    if (newvalue == null) return;

                    RegisterAndDo(newvalue, () => arr.Insert(newvalue, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RegisterAndDo(null, () => arr.Remove(e.OldStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Reset:
                    RegisterAndDo(null, () => arr.Reset());
                    break;
            }
        }

        private async void RegisterAndDo(IJSCSGlue ivalue, Action Do)
        {
            var idisp = ReListen();

            await InjectInHTLMSession(ivalue);
            await _Context.WebView.RunAsync(() =>
                    {
                        using (idisp)
                        {
                            Do();
                        }
                    }
               );
        }

        private IJSCSGlue GetCachedOrCreateBasicUnsafe(IJavascriptObject globalkey, Type iTargetType)
        {
            IJSCSGlue res;

            //Use local cache for objet not created in javascript session such as enum
            if ((globalkey != null) && 
                ((res = _SessionCache.GetGlobalCached(globalkey) ?? _SessionCache.GetCachedLocal(globalkey)) != null))
                return res;

            object targetvalue;
            bool converted = _Context.WebView.Converter.GetSimpleValue(globalkey, out targetvalue, iTargetType);
            if ((!converted) && (!globalkey.IsNull) && (!globalkey.IsUndefined))
                throw ExceptionHelper.Get(string.Format("Unable to convert javascript object: {0}", globalkey));

            return new JSBasicObject(globalkey, targetvalue);
        }

        public IJSCSGlue GetCachedOrCreateBasic(IJavascriptObject globalkey, Type iTargetType)
        {
            return _Context.WebView.Evaluate(()=> GetCachedOrCreateBasicUnsafe(globalkey,iTargetType));
        }
    }
}
