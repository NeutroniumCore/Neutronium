using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Exceptions;

using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Window;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class BidirectionalMapper : IDisposable, IJSCBridgeCache, IJavascriptListener
    {
        private readonly JavascriptBindingMode _BindingMode;
        private readonly IJSCSGlue _Root;
        private readonly IWebView _IWebView;
        private readonly List<IJSCSGlue> _UnrootedEntities;

        private CSharpToJavascriptMapper _JSObjectBuilder;
        private JavascriptSessionInjector _SessionInjector;

        private bool _IsListening = false;

        private IDictionary<object, IJSCSGlue> _FromCSharp = new Dictionary<object, IJSCSGlue>();
        private IDictionary<uint, IJSCSGlue> _FromJavascript_Global = new Dictionary<uint, IJSCSGlue>();
        private IDictionary<uint, IJSCSGlue> _FromJavascript_Local = new Dictionary<uint, IJSCSGlue>();

        internal BidirectionalMapper(object iRoot, IWebView iwebview, IDispatcher UIDispatcher, JavascriptBindingMode iMode, object iadd)
        {
            _IWebView = iwebview;
            _JSObjectBuilder = new CSharpToJavascriptMapper(iwebview,UIDispatcher, this);
            _Root = _JSObjectBuilder.Map(iRoot, iadd);
            _UnrootedEntities = new List<IJSCSGlue>();
            _BindingMode = iMode;

            IJavascriptListener JavascriptObjecChanges = null;
            if (iMode == JavascriptBindingMode.TwoWay)
                JavascriptObjecChanges = this;

            _SessionInjector = new JavascriptSessionInjector(iwebview, JavascriptObjecChanges);
        }

        internal async Task Init()
        {
            await InjectInHTLMSession(_Root, true);

            await _IWebView.RunAsync(() =>
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
            private IJSObservableBridge _Root;
            private BidirectionalMapper _LiveMapper;
            private TaskCompletionSource<object> _TCS = new TaskCompletionSource<object>();
            public JavascriptMapper(IJSObservableBridge iRoot, BidirectionalMapper iFather)
            {
                _LiveMapper = iFather;
                _Root = iRoot;
            }

            public void RegisterFirst(IJavascriptObject iRoot)
            {
                _LiveMapper.Update(_Root, iRoot);
            }

            public void RegisterMapping(IJavascriptObject iFather, string att, IJavascriptObject iChild)
            {
                _LiveMapper.RegisterMapping(iFather, att, iChild);
            }

            public void RegisterCollectionMapping(IJavascriptObject iFather, string att, int index, IJavascriptObject iChild)
            {
                _LiveMapper.RegisterCollectionMapping(iFather, att, index, iChild);
            }

            internal Task UpdateTask { get { return _TCS.Task; } }

            public void End(IJavascriptObject iRoot)
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
            JSGenericObject jso = GetFromJavascript(iFather) as JSGenericObject;
            Update(jso.Attributes[att] as IJSObservableBridge, iChild);
        }

        public void RegisterCollectionMapping(IJavascriptObject iFather, string att, int index, IJavascriptObject iChild)
        {
            var father = GetFromJavascript(iFather);
            var jsos = (att == null) ? father : (father as JSGenericObject).Attributes[att];

            Update((jsos as JSArray).Items[index] as IJSObservableBridge, iChild);
        }

        #endregion

        public bool ListenToCSharp { get { return (_BindingMode != JavascriptBindingMode.OneTime); } }

        private void ApplyOnListenableReferencedObjects(JSCBridgeListenableVisitor visitor)
        {
            _Root.ApplyOnListenable(visitor);
            _UnrootedEntities.ForEach(js => js.ApplyOnListenable(visitor));
        }

        private void ListenToCSharpChanges()
        {
            var list = new JSCBridgeListenableVisitor(n => n.PropertyChanged += Object_PropertyChanged,
                                     c => c.CollectionChanged += CollectionChanged, co => co.ListenChanges());

            ApplyOnListenableReferencedObjects(list);
        }

        private void UnlistenToCSharpChanges()
        {
            var list = new JSCBridgeListenableVisitor(n => n.PropertyChanged -= Object_PropertyChanged,
                           c => c.CollectionChanged -= CollectionChanged, co => co.UnListenChanges());

            ApplyOnListenableReferencedObjects(list);
        }

        public IJSCSGlue JSValueRoot { get { return _Root; } }

        private async Task InjectInHTLMSession(IJSCSGlue iroot, bool isroot = false)
        {
            if ((iroot == null) || (iroot.Type == JSCSGlueType.Basic))
                return;

            if ((iroot.Type == JSCSGlueType.Object) && (iroot.JSValue.IsNull))
                return;

            var jvm = new JavascriptMapper(iroot as IJSObservableBridge, this);
            var res = _SessionInjector.Map(iroot.JSValue, jvm, (iroot.CValue != null));

            await jvm.UpdateTask;

            if (isroot)
                await _SessionInjector.RegisterInSession(res);
        }

        public void OnJavaScriptObjectChanges(IJavascriptObject objectchanged, string PropertyName, IJavascriptObject newValue)
        {
            try
            {
                var res = GetFromJavascript(objectchanged) as JSGenericObject;
                if (res == null)
                    return;

                INotifyPropertyChanged inc = (!_IsListening) ? null : res.CValue as INotifyPropertyChanged;
                if (inc != null) inc.PropertyChanged -= Object_PropertyChanged;
                res.UpdateCSharpProperty(PropertyName, this, newValue);
                if (inc != null) inc.PropertyChanged += Object_PropertyChanged;
            }
            catch (Exception e)
            {
                ExceptionHelper.Log(string.Format("Unable to update ViewModel from View, exception raised: {0}", e));
            }
        }

        public void OnJavaScriptCollectionChanges(IJavascriptObject collectionchanged, IJavascriptObject[] value, IJavascriptObject[] status, IJavascriptObject[] index)
        {
            try
            {
                var res = GetFromJavascript(collectionchanged) as JSArray;
                if (res == null) return;

                CollectionChanges cc = res.GetChanger(value, status, index, this);

                using (ReListen(null))
                {
                    INotifyCollectionChanged inc = res.CValue as INotifyCollectionChanged;
                    if (inc != null) inc.CollectionChanged -= CollectionChanged;
                    res.UpdateEventArgsFromJavascript(cc);
                    if (inc != null) inc.CollectionChanged += CollectionChanged;
                }
            }
            catch (Exception e)
            {
                ExceptionHelper.Log(string.Format("Unable to update ViewModel from View, exception raised: {0}", e));
            }
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string pn = e.PropertyName;

            PropertyInfo propertyInfo = sender.GetType().GetProperty(pn, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
                return;

            JSGenericObject currentfather = _FromCSharp[sender] as JSGenericObject;

            object nv = propertyInfo.GetValue(sender, null);
            IJSCSGlue oldbridgedchild = currentfather.Attributes[pn];

            if (Object.Equals(nv, oldbridgedchild.CValue)) return;

            IJSCSGlue newbridgedchild = _JSObjectBuilder.Map(nv);

            RegisterAndDo(newbridgedchild, () => currentfather.Reroot(pn, newbridgedchild));
        }

        public void RegisterInSession(object nv, Action<IJSCSGlue> Continue)
        {
            IJSCSGlue newbridgedchild = _JSObjectBuilder.Map(nv);
            RegisterAndDo(newbridgedchild, () => { _UnrootedEntities.Add(newbridgedchild); Continue(newbridgedchild); });
        }

        #region Relisten

        private class ReListener : IDisposable
        {
            private HashSet<INotifyPropertyChanged> _OldObject = new HashSet<INotifyPropertyChanged>();
            private HashSet<INotifyCollectionChanged> _OldCollections = new HashSet<INotifyCollectionChanged>();
            private HashSet<JSCommand> _OldCommands = new HashSet<JSCommand>();

            private int _Count = 1;

            private BidirectionalMapper _BidirectionalMapper;
            public ReListener(BidirectionalMapper iBidirectionalMapper)
            {
                _BidirectionalMapper = iBidirectionalMapper;
                var list = new JSCBridgeListenableVisitor((e) => _OldObject.Add(e),
                    (e) => _OldCollections.Add(e), e => _OldCommands.Add(e));

                _BidirectionalMapper.ApplyOnListenableReferencedObjects(list);
            }

            public void AddRef()
            {
                _Count++;
            }

            public void Dispose()
            {
                if (--_Count == 0)
                {
                    Clean();
                }
            }

            private void Clean()
            {
                var newObject = new HashSet<INotifyPropertyChanged>();
                var new_Collections = new HashSet<INotifyCollectionChanged>();
                var new_Commands = new HashSet<JSCommand>();

                var list = new JSCBridgeListenableVisitor((e) => newObject.Add(e),
                                (e) => new_Collections.Add(e), e => new_Commands.Add(e));


                _BidirectionalMapper.ApplyOnListenableReferencedObjects(list);

                _OldObject.Where(o => !newObject.Contains(o)).ForEach(o => o.PropertyChanged -= _BidirectionalMapper.Object_PropertyChanged);
                newObject.Where(o => !_OldObject.Contains(o)).ForEach(o => o.PropertyChanged += _BidirectionalMapper.Object_PropertyChanged);

                _OldCollections.Where(o => !new_Collections.Contains(o)).ForEach(o => o.CollectionChanged -= _BidirectionalMapper.CollectionChanged);
                new_Collections.Where(o => !_OldCollections.Contains(o)).ForEach(o => o.CollectionChanged += _BidirectionalMapper.CollectionChanged);

                _OldCommands.Where(o => !new_Commands.Contains(o)).ForEach(o => o.UnListenChanges());
                new_Commands.Where(o => !_OldCommands.Contains(o)).ForEach(o => o.ListenChanges());

                _BidirectionalMapper._ReListen = null;
            }
        }

        private ReListener _ReListen = null;
        private IDisposable ReListen(IJSCSGlue ivalue)
        {
            if (_ReListen != null)
                _ReListen.AddRef();
            else
                _ReListen = new ReListener(this);

            return _ReListen;
        }

        #endregion

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _IWebView.RunAsync(() =>
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
            var idisp = ReListen(ivalue);

            await InjectInHTLMSession(ivalue);
            await _IWebView.RunAsync(() =>
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

            if (_SessionInjector != null)
            {
                _SessionInjector.Dispose();
                _SessionInjector = null;
            }

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
            IJSCSGlue res = null;
            _FromCSharp.TryGetValue(key, out res);
            return res;
        }

        public IJSCSGlue GetCached(IJavascriptObject globalkey)
        {
            return _IWebView.Evaluate(() =>
                {
                    if (!globalkey.HasRelevantId())
                        return null;

                    IJSCSGlue res = null;
                    _FromJavascript_Global.TryGetValue(globalkey.GetID(), out res);
                    return res;
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
            bool converted = _IWebView.Converter.GetSimpleValue(globalkey, out targetvalue, iTargetType);
            if ((!converted) && (!globalkey.IsNull) && (!globalkey.IsUndefined))
                throw ExceptionHelper.Get(string.Format("Unable to convert javascript object: {0}", globalkey));

            return new JSBasicObject(globalkey, targetvalue);
        }

        private IJSCSGlue GetCachedLocal(IJavascriptObject localkey)
        {
            if (!localkey.HasRelevantId())
                return null;

            IJSCSGlue res = null;
            _FromJavascript_Local.TryGetValue(localkey.GetID(), out res);
            return res;
        }
    }
}
