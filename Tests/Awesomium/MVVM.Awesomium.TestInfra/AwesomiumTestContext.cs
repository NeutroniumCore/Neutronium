using KnockoutUIFramework.Test.IntegratedInfra;
using MVVM.HTML.Core.Infra;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;

namespace MVVM.Awesomium.TestInfra 
{
    public class AwesomiumTestContext : IWindowLessHTMLEngineProvider 
    {
        private AwesomiumWindowlessHTMLEngineFactory _AwesomiumWindowlessHTMLEngineFactory;

        private AwesomiumWindowlessHTMLEngineFactory GetWindowLessEngine() 
        {
            if (_AwesomiumWindowlessHTMLEngineFactory != null)
                return _AwesomiumWindowlessHTMLEngineFactory;

            return _AwesomiumWindowlessHTMLEngineFactory = new AwesomiumWindowlessHTMLEngineFactory();
        }

        public IWindowlessIntegratedContextBuilder GetWindowlessEnvironment() 
        {
            return new WindowlessIntegratedTestEnvironment()
            {
                WindowlessJavascriptEngineBuilder = () => GetWindowLessEngine().CreateWindowlessJavascriptEngine(),
                FrameworkTestContext = KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext(),
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
