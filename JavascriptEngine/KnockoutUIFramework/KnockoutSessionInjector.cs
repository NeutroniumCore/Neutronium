using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Extension;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;
using UIFramework.Uttils;

namespace KnockoutUIFramework
{
    internal class KnockoutSessionInjector : IJavascriptSessionInjector
    {
        private readonly IWebView _WebView;
        private readonly Queue<IJavascriptObjectMapper> _JavascriptMapper = new Queue<IJavascriptObjectMapper>();
        private IJavascriptObject _Listener;
        private IJavascriptObjectMapper _Current;
        private IJavascriptObject _Mapper;
        private bool _PullNextMapper = true;
        private IJavascriptObject _Ko;

        public KnockoutSessionInjector(IWebView webView, IJavascriptChangesObserver javascriptListener)
        {
            _WebView = webView;

            var builder = new BinderBuilder(_WebView, javascriptListener);
            _Listener = builder.BuildListener();
        }

        public void Dispose()
        {
            if (_Listener == null)
                return;

            _Listener.Dispose();
            _Listener = null;
        }

        public IJavascriptObject Inject(IJavascriptObject ihybridobject, IJavascriptObjectMapper ijvm)
        {
            return _WebView.Evaluate(() => UnsafeInject(ihybridobject, ijvm));
        }

        public IJavascriptObject UnsafeInject(IJavascriptObject ihybridobject, IJavascriptObjectMapper ijvm) 
        {
            var res = GetKo().Invoke("MapToObservable", _WebView, ihybridobject, GetMapper(ijvm), _Listener);
            return (res == null || res.IsUndefined) ? null : res;
        }

        private IJavascriptObject GetKo()
        {
            if (_Ko != null)
                return _Ko;

            _Ko = _WebView.GetGlobal().GetValue("ko");
            if ((_Ko == null) || (!_Ko.IsObject))
                throw ExceptionHelper.Get("ko object not found! You should add a link to knockout.js script to the HML document!");

            _Ko.Bind("log", _WebView, (e) => ExceptionHelper.Log(string.Join(" - ", e.Select(s => (s.GetStringValue().Replace("\n", " "))))));

            return _Ko;
        }

        private IJavascriptObject GetMapper(IJavascriptObjectMapper iMapperListener)
        {
            _JavascriptMapper.Enqueue(iMapperListener);

            if (_Mapper != null)
                return _Mapper;

            _Mapper = _WebView.Factory.CreateObject(false);

            _Mapper.Bind("Register", _WebView, (e) =>
            {
                if (_PullNextMapper)
                {
                    _Current = _JavascriptMapper.Dequeue();
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

            _Mapper.Bind("End", _WebView, (e) =>
            {
                if (_PullNextMapper)
                    _Current = _JavascriptMapper.Dequeue();

                if (_Current != null)
                    _Current.EndMapping(e[0]);
                _Current = null;
                _PullNextMapper = true;
            });

            return _Mapper;
        }

        public Task RegisterMainViewModel(IJavascriptObject jsObject)
        {
            return _WebView.RunAsync(() =>
                {  
                    var ko = GetKo();
                    ko.Invoke("register", _WebView, jsObject);
                    ko.Invoke("applyBindings", _WebView, jsObject);
                });
        }
    }
}
