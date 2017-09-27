using System.ComponentModel;
using FluentAssertions;
using Neutronium.Core.Infra;
using Xunit;

namespace Neutronium.Core.Test.Infra
{
    public class EnumExtensionsTest
    {
        enum Test { Tested };

        [Fact]
        public void Test_GetDescription_FallBack()
        {
            Test vi = Test.Tested;
            vi.GetDescription().Should().Be("Tested");
        }

        enum Ex { [Description("Cute")] ex1 = 8, [Description("Cute2")] ex2 = 16 };

        [Fact]
        public void Test_GetDescription_Description()
        {
            Ex vi = Ex.ex1;
            vi.GetDescription().Should().Be("Cute");
        }

        [Fact]
        public void Test_GetDescription_Or()
        {
            Ex vi = Ex.ex1 | Ex.ex2;
            vi.GetDescription().Should().Be("24");
        }
    }
}
