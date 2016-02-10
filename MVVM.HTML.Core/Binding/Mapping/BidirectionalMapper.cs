using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Exceptions;

using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.Binding.Listeners;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class BidirectionalMapper : IDisposable, IVisitable, IJavascriptToCSharpConverter, 
                                    IJSCBridgeCache, IJavascriptChangesListener
    {
        private readonly HTMLViewContext _Context;        
        private readonly JavascriptBindingMode _BindingMode;
        private readonly CSharpToJavascriptConverter _JSObjectBuilder;
        private readonly IJavascriptSessionInjector _sessionInjector;
        private readonly IJSCSGlue _Root;
      
        private readonly List<IJSCSGlue> _UnrootedEntities;

        private bool _IsListening = false;

        private readonly IDictionary<object, IJSCSGlue> _FromCSharp = new Dictionary<object, IJSCSGlue>();
        private readonly IDictionary<uint, IJSCSGlue> _FromJavascript_Global = new Dictionary<uint, IJSCSGlue>();
        private readonly IDictionary<uint, IJSCSGlue> _FromJavascript_Local = new Dictionary<uint, IJSCSGlue>();
        private readonly FullListenerRegister _ListenerRegister;

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
            _JSObjectBuilder = new CSharpToJavascriptConverter(_Context, this);
            _Root = _JSObjectBuilder.Map(iRoot, iadd);
            _UnrootedEntities = new List<IJSCSGlue>();
            _BindingMode = iMode;

             var JavascriptObjecChanges = (iMode == JavascriptBindingMode.TwoWay) ? (IJavascriptChangesListener)this : null;
            _sessionInjector = _Context.CreateInjector(JavascriptObjecChanges);
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

        #region IJavascriptMapper

        private class JavascriptMapper : IJavascriptMapper
        {
            private readonly IJSObservableBridge _Root;
            private readonly BidirectionalMapper _LiveMapper;
            private readonly TaskCompletionSource<object> _TCS = new TaskCompletionSource<object>();
            public JavascriptMapper(IJSObservableBridge iRoot, BidirectionalMapper iFather)
            {
                _LiveMapper = iFather;
                _Root = iRoot;
            }

            public void MapFirst(IJavascriptObject iRoot)
            {
                _LiveMapper.Update(_Root, iRoot);
            }

            public void Map(IJavascriptObject iFather, string att, IJavascriptObject iChild)
            {
                _LiveMapper.RegisterMapping(iFather, att, iChild);
            }

            public void MapCollection(IJavascriptObject iFather, string att, int index, IJavascriptObject iChild)
            {
                _LiveMapper.RegisterCollectionMapping(iFather, att, index, iChild);
            }

            internal Task UpdateTask { get { return _TCS.Task; } }

            public void EndMapping(IJavascriptObject iRoot)
            {
                _TCS.TrySetResult(null);
            }
        }

        private IJSCSGlue GetFromJavascript(IJavascriptObject jsobject)
        {
            return _FromJavascript_Global[jsobject.GetID()];
        }

        private void Update(IJSObservableBridge ibo, IJavascriptObject jsobject)
        {
            ibo.SetMappedJSValue(jsobject, this);
            if (jsobject.HasRelevantId())
                _FromJavascript_Global[jsobject.GetID()] = ibo;
        }

        public void RegisterMapping(IJavascriptObject iFather, string att, IJavascriptObject iChild)
        {
            var jso = GetFromJavascript(iFather) as JSGenericObject;
            Update(jso.Attributes[att] as IJSObservableBridge, iChild);
        }

        public void RegisterCollectionMapping(IJavascriptObject iFather, string att, int index, IJavascriptObject iChild)
        {
            var father = GetFromJavascript(iFather);
            var jsos = (att == null) ? father : (father as JSGenericObject).Attributes[att];

            Update(((JSArray) jsos).Items[index] as IJSObservableBridge, iChild);
        }

        #endregion


        public bool ListenToCSharp { get { return (_BindingMode != JavascriptBindingMode.OneTime); } }

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

        public IJSCSGlue JSValueRoot { get { return _Root; } }

        private async Task InjectInHTLMSession(IJSCSGlue iroot, bool isroot = false)
        {
            if ((iroot == null) || (iroot.Type == JSCSGlueType.Basic))
                return;

            if ((iroot.Type == JSCSGlueType.Object) && (iroot.JSValue.IsNull))
                return;

            var jvm = new JavascriptMapper(iroot as IJSObservableBridge, this);
            var res = _sessionInjector.Inject(iroot.JSValue, jvm, (iroot.CValue != null));

            await jvm.UpdateTask;

            if (isroot)
                await _sessionInjector.RegisterMainViewModel(res);
        }

        public void OnJavaScriptObjectChanges(IJavascriptObject objectchanged, string PropertyName, IJavascriptObject newValue)
        {
            try
            {
                var res = GetFromJavascript(objectchanged) as JSGenericObject;
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
                var res = GetFromJavascript(changes.Collection) as JSArray;
                if (res == null) return;

                CollectionChanges cc = res.GetChanger(changes, this);

                using (ReListen()) 
                using (_IsListening ? _ListenerRegister.GetColllectionSilenter(res.CValue) : null)
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

            var currentfather = _FromCSharp[sender] as JSGenericObject;
            if (currentfather == null)
                return;

            var nv = propertyAccessor.Get();
            var oldbridgedchild = currentfather.Attributes[pn];

            if (Object.Equals(nv, oldbridgedchild.CValue)) return;

            var newbridgedchild = _JSObjectBuilder.Map(nv);
            RegisterAndDo(newbridgedchild, () => currentfather.Reroot(pn, newbridgedchild));
        }

        public void RegisterInSession(object nv, Action<IJSCSGlue> Continue)
        {
            IJSCSGlue newbridgedchild = _JSObjectBuilder.Map(nv);
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
            JSArray arr = _FromCSharp[sender] as JSArray;
            if (arr == null) return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    IJSCSGlue addvalue = _JSObjectBuilder.Map(e.NewItems[0]);

                    if (addvalue == null) return;

                    RegisterAndDo(addvalue, () => arr.Add(addvalue, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    IJSCSGlue newvalue = _JSObjectBuilder.Map(e.NewItems[0]);

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

        public void Dispose()
        {
            if (ListenToCSharp)
                UnlistenToCSharpChanges();

            _sessionInjector.Dispose();
            _UnrootedEntities.Clear();
        }

        void IJSCBridgeCache.Cache(object key, IJSCSGlue value)
        {
            _FromCSharp.Add(key, value);
        }

        void IJSCBridgeCache.CacheLocal(object key, IJSCSGlue value)
        {
            _FromCSharp.Add(key, value);
            _FromJavascript_Local.Add(value.JSValue.GetID(), value);
        }

        IJSCSGlue IJSCBridgeCache.GetCached(object key)
        {
            return _FromCSharp.GetOrDefault(key);
        }

        public IJSCSGlue GetCached(IJavascriptObject globalkey)
        {
            return _Context.WebView.Evaluate(() =>
                {
                    if (!globalkey.HasRelevantId())
                        return null;

                    return _FromJavascript_Global.GetOrDefault(globalkey.GetID());
                });
        }

        public IJSCSGlue GetCachedOrCreateBasic(IJavascriptObject globalkey, Type iTargetType)
        {
            IJSCSGlue res = null;
            IJavascriptObject obj = globalkey;

            //Use local cache for objet not created in javascript session such as enum
            if ((obj != null) && ((res = GetCached(globalkey) ?? GetCachedLocal(globalkey)) != null))
                return res;

            object targetvalue = null;
            bool converted = _Context.WebView.Converter.GetSimpleValue(globalkey, out targetvalue, iTargetType);
            if ((!converted) && (!globalkey.IsNull) && (!globalkey.IsUndefined))
                throw ExceptionHelper.Get(string.Format("Unable to convert javascript object: {0}", globalkey));

            return new JSBasicObject(globalkey, targetvalue);
        }

        private IJSCSGlue GetCachedLocal(IJavascriptObject localkey)
        {
            if (!localkey.HasRelevantId())
                return null;

            return _FromJavascript_Local.GetOrDefault(localkey.GetID());
        }
    }
}
