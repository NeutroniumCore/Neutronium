using FluentAssertions;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Xunit;

namespace Neutronium.Core.Test.WebBrowserEngine.JavascriptObject
{
    public class JavascriptNamerTest
    {
        [Theory]
        [InlineData("a", "'a'")]
        [InlineData("cat", "'cat'")]
        [InlineData(@"\", @"'\\'")]
        [InlineData("\n", @"'\n'")]
        [InlineData("\n", @"'\n'")]
        [InlineData("\r", @"'\r'")]
        [InlineData("'", @"'\''")]
        public void GetCreateExpression_Returns_Correct_Information(string value, string expected)
        {
            var actual = JavascriptNamer.GetCreateExpression(value);
            actual.Should().Be(expected);
        }
    }
}
