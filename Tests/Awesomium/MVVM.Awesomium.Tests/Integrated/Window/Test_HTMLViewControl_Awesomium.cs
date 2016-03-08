using IntegratedTest.WPF;
using IntegratedTest.WPF.Infra;
using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Tests.Integrated.Window 
{
    [CollectionDefinition("Awesomium Window Integrated")]
    public class Test_HTMLViewControl_Awesomium : Test_HTMLViewControl
    {
        public Test_HTMLViewControl_Awesomium(AwesomiumWindowTestEnvironment context, WpfThread wpfThread)
            : base(context, wpfThread) 
        {
        }
    }
}
