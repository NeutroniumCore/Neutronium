using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Xilium.CefGlue;

using MVVM.CEFGlue.CefGlueHelper;
using MVVM.CEFGlue.Exceptions;
using System.Threading.Tasks;

namespace MVVM.CEFGlue.HTMLBinding
{
    internal class JavascriptSessionInjector : IDisposable
    {
        private CefV8CompleteContext _CefV8Context;
        private IJSOBuilder _GlobalBuilder;
        private CefV8Value _Listener;
        private IJavascriptListener _IJavascriptListener;


        internal JavascriptSessionInjector(CefV8CompleteContext iWebView, IJSOBuilder iGlobalBuilder, IJavascriptListener iJavascriptListener)
        {
            _CefV8Context = iWebView;
            _GlobalBuilder = iGlobalBuilder;
            _IJavascriptListener = iJavascriptListener;


            if (_IJavascriptListener != null)
            {
                _Listener = _GlobalBuilder.CreateJSO();
                _Listener.Bind("TrackChanges",_CefV8Context, (c, o, e) => _IJavascriptListener.OnJavaScriptObjectChanges(e[0], e[1].GetStringValue(), e[2]));
                _Listener.Bind("TrackCollectionChanges", _CefV8Context, (c, o, e) => _IJavascriptListener.OnJavaScriptCollectionChanges(e[0], e[1].GetArrayElements(), e[2].GetArrayElements(), e[3].GetArrayElements()));
            }
            else
                _Listener = _GlobalBuilder.CreateJSO();
        }

        private Queue<IJavascriptMapper> _IJavascriptMapper = new Queue<IJavascriptMapper>();
        private IJavascriptMapper _Current;
        private bool _PullNextMapper = true;
        private CefV8Value _Mapper;

        private CefV8Value GetMapper(IJavascriptMapper iMapperListener)
        {
            _IJavascriptMapper.Enqueue(iMapperListener);
    
            if (_Mapper != null)
                return _Mapper;

            _Mapper = _GlobalBuilder.CreateJSO();

            _Mapper.Bind("Register", _CefV8Context, (c, o, e) =>
            {
                if (_PullNextMapper)
                { 
                    _Current = _IJavascriptMapper.Dequeue();
                    _PullNextMapper = false;
                }

                if (_Current == null)
                    return;

                int count = e.Length;
                CefV8Value registered = e[0];

                switch (count)
                {
                    case 1:
                        _Current.RegisterFirst(registered);
                        break;

                    case 3:
                        _Current.RegisterMapping(e[1], e[2].GetStringValue(), registered);
                        break;

                    case 4:
                        _Current.RegisterCollectionMapping(e[1], e[2].GetStringValue(), e[3].GetIntValue(), registered);
                        break;
                }
             });

            _Mapper.Bind("End", _CefV8Context, (c, o, e) =>
                {
                    if (_PullNextMapper)
                        _Current = _IJavascriptMapper.Dequeue();

                    if (_Current!=null)
                        _Current.End(e[0]);
                    _Current = null;
                    _PullNextMapper = true;
                });

            return _Mapper;
        }

        private CefV8Value _Ko;
        private CefV8Value GetKo()
        {
            if (_Ko == null)
            {
                _Ko = _CefV8Context.Context.GetGlobal().GetValue("ko");
                if ((_Ko==null) || (!_Ko.IsObject))
                    throw ExceptionHelper.NoKo();
            }

            return _Ko;
        }


        public CefV8Value Map(CefV8Value ihybridobject, IJavascriptMapper ijvm, bool checknullvalue = true)
        {
            return _CefV8Context.Evaluate(() =>
                {

                    CefV8Value res = GetKo().Invoke("MapToObservable", _CefV8Context,ihybridobject, GetMapper(ijvm), _Listener);
                    if ((res == null) && checknullvalue)
                    {
                        throw ExceptionHelper.NoKo();
                    }
                    return res;
                });
        }

        public Task RegisterInSession(CefV8Value iJSObject)
        {
            var ko = GetKo();

            return _CefV8Context.RunAsync(() =>
                {
                    ko.Bind("log", _CefV8Context, (c, o, e) => ExceptionHelper.Log(string.Join(" - ", e.Select(s => (s.GetStringValue().Replace("\n", " "))))));
                    ko.Invoke("register", _CefV8Context, iJSObject);
                    ko.Invoke("applyBindings", _CefV8Context, iJSObject);
                });
        }

        public void Dispose()
        {
            _CefV8Context.Run(() =>
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
