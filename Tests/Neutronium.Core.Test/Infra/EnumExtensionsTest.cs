using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
            var value = Test.Tested;
            value.GetDescription().Should().Be("Tested");
        }

        public enum Ex
        {
            [Description("Cute")]
            ex1 = 8,

            [Description("Cute2")]
            ex2 = 16
        };


        [Theory]
        [InlineData(Ex.ex1, "Cute")]
        [InlineData(Ex.ex2, "Cute2")]
        public void GetDescription_Uses_Description_Attribute(Ex value, string expected)
        {
            value.GetDescription().Should().Be(expected);
        }

        [Fact]
        public void GetDescription_Uses_ToString_When_Not_Field_And_Not_Defined()
        {
            var value = Ex.ex1 | Ex.ex2;
            value.GetDescription().Should().Be("24");
        }

        [Flags]
        public enum Number
        {
            [Description("One")]
            Un = 1,

            [Description("Two")]
            Deux = 2,

            [Description("Four")]
            Quatre = 4,

            [Description("Six")]
            Six = 6
        };

        [Theory]
        [InlineData((Number)3, "One Two")]
        [InlineData((Number)5, "One Four")]
        [InlineData((Number)7, "One Six")]
        public void GetDescription_Combines_Description_Attribute_On_BitFlag(Number @enum, string expected)
        {
            @enum.GetDescription().Should().Be(expected);
        }

        [Fact]
        public void GetDescription_Uses_Attribute_On_Priority_On_BitFlag()
        {
            var value = Number.Six;
            value.GetDescription().Should().Be("Six");
        }

        [Fact]
        public void GetDescription_Falls_Back_On_To_String()
        {
            var value = (Number)8;
            value.GetDescription().Should().Be("8");
        }

        public enum Localized
        {
            [Display(ResourceType = typeof(Resource), Description = "One")]
            Uno,

            [Display(ResourceType = typeof(Resource), Description = "Two")]
            Dos,

            [Display(ResourceType = typeof(Localized), Description = "Two")]
            ProblemWithType,

            [Display(ResourceType = typeof(Resource), Description = "Unknown")]
            ProblemWithKey
        }

        [Theory]
        [InlineData(Localized.Uno, "Un")]
        [InlineData(Localized.Dos, "Deux")]
        public void GetDescription_Uses_DisplayAttribute(Localized value, string expected)
        {
            value.GetDescription().Should().Be(expected);
        }

        [Theory]
        [InlineData(Localized.ProblemWithType, "ProblemWithType")]
        [InlineData(Localized.ProblemWithKey, "ProblemWithKey")]
        public void GetDescription_Uses_Fallabck_When_Problem_With_DisplayAttribute(Localized value, string expected)
        {
            value.GetDescription().Should().Be(expected);
        }
    }
}
