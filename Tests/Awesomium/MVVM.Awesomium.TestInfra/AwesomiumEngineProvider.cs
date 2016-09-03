using MVVM.HTML.Core.Infra;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;

namespace Tests.Awesomium.Infra
{
    public class AwesomiumEngineProvider : IBasicWindowLessHTMLEngineProvider
    {
        private AwesomiumWindowlessHTMLEngineFactory _AwesomiumWindowlessHTMLEngineFactory;
        private readonly ITestHtmlProvider _HtmlProvider;

        public AwesomiumEngineProvider(ITestHtmlProvider htmlProvider) 
        {
            _HtmlProvider = htmlProvider;
        }

        private AwesomiumWindowlessHTMLEngineFactory GetWindowLessEngine() 
        {
            if (_AwesomiumWindowlessHTMLEngineFactory != null)
                return _AwesomiumWindowlessHTMLEngineFactory;

            return _AwesomiumWindowlessHTMLEngineFactory = new AwesomiumWindowlessHTMLEngineFactory();
        }

        public IWindowlessHTMLEngineBuilder GetWindowlessEnvironment() 
        {
            return new WindowlessIntegratedTestEnvironment()
            {
                WindowlessJavascriptEngineBuilder = () => GetWindowLessEngine().CreateWindowlessJavascriptEngine(),
                HtmlProvider = _HtmlProvider,
                TestUIDispacther = new NullUIDispatcher()
            };
        }

        public void Dispose() 
        {
            if (_AwesomiumWindowlessHTMLEngineFactory != null)
            {
                _AwesomiumWindowlessHTMLEngineFactory.Dispose();
                _AwesomiumWindowlessHTMLEngineFactory = null;
            }
        }
    }
}
