using CefGlue.TestInfra;
using IntegratedTest.Windowless;
using Xunit;

namespace CefGlue.Windowless.Integrated.Tests
{
    [Collection("Cef Windowless Integrated")]
    public class Test_CefTask_Action_Cef : Test_CefTask_Action
    {
        public Test_CefTask_Action_Cef(CefGlueWindowlessSharedJavascriptEngineFactory context) : 
            base(context)
        {
        }
    }
}
