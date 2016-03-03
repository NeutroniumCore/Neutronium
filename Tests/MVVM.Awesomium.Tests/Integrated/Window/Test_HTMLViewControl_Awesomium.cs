using IntegratedTest.WPF;
using IntegratedTest.WPF.Infra;
using MVVM.Awesomium.Tests.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Awesomium.Tests.Integrated.Window
{
    public class Test_HTMLViewControl_Awesomium : Test_HTMLViewControl
    {
        protected override WindowTestEnvironment GetEnvironment()
        {
            return AwesomiumTestHelper.GetWindowEnvironment();
        }
    }
}
