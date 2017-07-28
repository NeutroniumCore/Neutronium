using System;
using Tests.CefGlue.WebBrowserEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CefGlue.WebBrowserEngineTests
{
    [Collection("CefGlue Context")]
    public class CefGlue_JavascriptFactoryBulk_Tests : JavascriptFactoryBulk_Tests
    {
        public CefGlue_JavascriptFactoryBulk_Tests(CefGlueContext testEnvironment, ITestOutputHelper output)
                        : base(testEnvironment, output)
        {
        }

        protected override bool SupportStringEmpty => false;
    }
}
