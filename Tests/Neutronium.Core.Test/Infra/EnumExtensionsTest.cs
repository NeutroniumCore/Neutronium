using System.ComponentModel;
using FluentAssertions;
using Neutronium.Core.Infra;
using Xunit;

namespace Neutronium.Core.Test.Infra
{
    public class EnumExtensionsTest
    {
        private enum Test
        {
            Tested
        };

        [Fact]
        public void GetDescription_Has_FallBack()
        {
            var vi = Test.Tested;
            vi.GetDescription().Should().Be("Tested");
        }

        private enum Ex
        {
            [Description("Cute")]
            ex1 = 8,

            [Description("Cute2")]
            ex2 = 16
        };

        [Fact]
        public void GetDescription_Uses_Description_Attribute()
        {
            var vi = Ex.ex1;
            vi.GetDescription().Should().Be("Cute");
        }

        [Fact]
        public void GetDescription_Uses_ToString()
        {
            var vi = Ex.ex1 | Ex.ex2;
            vi.GetDescription().Should().Be("24");
        }
    }
}
