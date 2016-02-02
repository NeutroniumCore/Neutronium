using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Infra;

namespace MVVM.HTML.Core.HTMLBinding
{
    internal class KnockoutSessionInjector : IJavascriptSessionInjector
    {
        private readonly IWebView _IWebView;
        private readonly IJavascriptChangesListener _IJavascriptListener;
        private readonly Queue<IJavascriptMapper> _IJavascriptMapper = new Queue<IJavascriptMapper>();
        private IJavascriptObject _Listener;
        private IJavascriptMapper _Current;
        private IJavascriptObject _Mapper;
        private bool _PullNextMapper = true;

        internal KnockoutSessionInjector(IWebView iWebView, IJavascriptChangesListener iJavascriptListener)
        {
            _IWebView = iWebView;
            _IJavascriptListener = iJavascriptListener;

            _IWebView.Run(() =>
                {
                    _Listener = _IWebView.Factory.CreateObject(false);

                    if (_IJavascriptListener != null)
                    {
                        _Listener.Bind("TrackChanges", _IWebView, (c, o, e) => _IJavascriptListener.OnJavaScriptObjectChanges(e[0], e[1].GetStringValue(), e[2]));
                        _Listener.Bind("TrackCollectionChanges", _IWebView, (c, o, e) => JavascriptColectionChanged(e));
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

        private IJavascriptObject GetMapper(IJavascriptMapper iMapperListener)
        {
            _IJavascriptMapper.Enqueue(iMapperListener);
    
            if (_Mapper != null)
                return _Mapper;

            _Mapper = _IWebView.Factory.CreateObject(false);

            _Mapper.Bind("Register", _IWebView, (c, o, e) =>
            {
                if (_PullNextMapper)
                { 
                    _Current = _IJavascriptMapper.Dequeue();
                    _PullNextMapper = false;
                }

                if (_Current == null)
                    return;

                int count = e.Length;
                IJavascriptObject registered = e[0];

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

            _Mapper.Bind("End", _IWebView, (c, o, e) =>
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


        public IJavascriptObject Inject(IJavascriptObject ihybridobject, IJavascriptMapper ijvm, bool checknullvalue = true)
        {
            return _IWebView.Evaluate(() =>
                {

                    IJavascriptObject res = GetKo().Invoke("MapToObservable", _IWebView, ihybridobject, GetMapper(ijvm), _Listener);
                    if (( (res == null) || (res.IsUndefined)) && checknullvalue)
                    {
                        throw ExceptionHelper.NoKo();
                    }
                    return res;
                });
        }

        public Task RegisterMainViewModel(IJavascriptObject iJSObject)
        {
            var ko = GetKo();

            return _IWebView.RunAsync(() =>
                {
                    ko.Bind("log", _IWebView, (c, o, e) => ExceptionHelper.Log(string.Join(" - ", e.Select(s => (s.GetStringValue().Replace("\n", " "))))));
                    ko.Invoke("register", _IWebView, iJSObject);
                    ko.Invoke("applyBindings", _IWebView, iJSObject);
                });
        }

        public void Dispose()
        {
            _IWebView.Run(() =>
            {
                if (_Listener != null)
                {
                    _Listener.Dispose();
                    _Listener = null;
                }
            });
        }
    }
}
