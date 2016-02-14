using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Binding.Extension;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    internal class KnockoutSessionInjector : IJavascriptSessionInjector
    {
        private readonly IWebView _IWebView;
        private readonly IJavascriptChangesObserver _IJavascriptListener;
        private readonly Queue<IJavascriptObjectMapper> _IJavascriptMapper = new Queue<IJavascriptObjectMapper>();
        private IJavascriptObject _Listener;
        private IJavascriptObjectMapper _Current;
        private IJavascriptObject _Mapper;
        private bool _PullNextMapper = true;

        internal KnockoutSessionInjector(IWebView iWebView, IJavascriptChangesObserver iJavascriptListener)
        {
            _IWebView = iWebView;
            _IJavascriptListener = iJavascriptListener;

            _IWebView.Run(() =>
                {
                    _Listener = _IWebView.Factory.CreateObject(false);

                    if (_IJavascriptListener != null)
                    {
                        _Listener.Bind("TrackChanges", _IWebView, (e) => _IJavascriptListener.OnJavaScriptObjectChanges(e[0], e[1].GetStringValue(), e[2]));
                        _Listener.Bind("TrackCollectionChanges", _IWebView, JavascriptColectionChanged);
                    }
                });
        }

        private void JavascriptColectionChanged(IJavascriptObject[] arguments)
        {
            var values = arguments[1].GetArrayElements();
            var types = arguments[2].GetArrayElements();
            var indexes = arguments[3].GetArrayElements();
            var collectionChange = new JavascriptCollectionChanges(arguments[0], values.Zip(types, indexes,
                                            (v, t, i) => new IndividualJavascriptCollectionChange(
                                                t.GetStringValue() == "added" ? CollectionChangeType.Add : CollectionChangeType.Remove,
                                                i.GetIntValue(), v)));
       
            _IJavascriptListener.OnJavaScriptCollectionChanges(collectionChange);
        }

        private IJavascriptObject GetMapper(IJavascriptObjectMapper iMapperListener)
        {
            _IJavascriptMapper.Enqueue(iMapperListener);
    
            if (_Mapper != null)
                return _Mapper;

            _Mapper = _IWebView.Factory.CreateObject(false);

            _Mapper.Bind("Register", _IWebView, (e) =>
            {
                if (_PullNextMapper)
                { 
                    _Current = _IJavascriptMapper.Dequeue();
                    _PullNextMapper = false;
                }

                if (_Current == null)
                    return;

                int count = e.Length;
                var registered = e[0];

                switch (count)
                {
                    case 1:
                        _Current.MapFirst(registered);
                        break;

                    case 3:
                        _Current.Map(e[1], e[2].GetStringValue(), registered);
                        break;

                    case 4:
                        _Current.MapCollection(e[1], e[2].GetStringValue(), e[3].GetIntValue(), registered);
                        break;
                }
             });

            _Mapper.Bind("End", _IWebView, (e) =>
                {
                    if (_PullNextMapper)
                        _Current = _IJavascriptMapper.Dequeue();

                    if (_Current!=null)
                        _Current.EndMapping(e[0]);
                    _Current = null;
                    _PullNextMapper = true;
                });

            return _Mapper;
        }

        private IJavascriptObject _Ko;
        private IJavascriptObject GetKo()
        {
            if (_Ko == null)
            {
                _Ko = _IWebView.GetGlobal().GetValue("ko");
                if ((_Ko==null) || (!_Ko.IsObject))
                    throw ExceptionHelper.NoKo();
            }

            return _Ko;
        }

        public IJavascriptObject Inject(IJavascriptObject ihybridobject, IJavascriptObjectMapper ijvm)
        {
            return _IWebView.Evaluate(() =>
                {
                    return GetKo().Invoke("MapToObservable", _IWebView, ihybridobject, GetMapper(ijvm), _Listener);
                });
        }

        public Task RegisterMainViewModel(IJavascriptObject iJSObject)
        {
            var ko = GetKo();

            return _IWebView.RunAsync(() =>
                {
                    ko.Bind("log", _IWebView, (e) => ExceptionHelper.Log(string.Join(" - ", e.Select(s => (s.GetStringValue().Replace("\n", " "))))));
                    ko.Invoke("register", _IWebView, iJSObject);
                    ko.Invoke("applyBindings", _IWebView, iJSObject);
                });
        }

        public void Dispose()
        {
            _IWebView.Run(() =>
            {
                if (_Listener == null)
                    return;

                _Listener.Dispose();
                _Listener = null;
            });
        }
    }
}
