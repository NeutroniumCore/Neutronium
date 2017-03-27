using FluentAssertions;
using Neutronium.WPF.Utils;
using System;
using Xunit;

namespace Neutronium.WPF.Test.Utils
{
    public class VersionHelperTest
    {
        [Theory]
        [InlineData(1, 2, 3, "1.2.3")]
        [InlineData(0, 5, 0, "0.5.0")]
        [InlineData(0, 5, 1, "0.5.1")]
        public void GetVersionDisplayName_ShouldReturn_CorrectValue(int major, int minor, int build, string expected)
        {
            var version = new Version(major, minor, build);

            VersionHelper.GetDisplayName(version).Should().Be(expected);
        }
    }
}
