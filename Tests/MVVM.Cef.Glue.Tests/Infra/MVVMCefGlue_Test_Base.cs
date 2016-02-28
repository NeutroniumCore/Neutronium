using IntegratedTest;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHelper;
using MVVM.Cef.Glue.Test.Generic;
using MVVM.Cef.Glue.Test.Infra;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.Cef.Glue.Test.Core
{
    public abstract class MVVMCefGlue_Test_Base : MVVMCefCore_Test_Base
    {
        public MVVMCefGlue_Test_Base(): base(CefTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}
