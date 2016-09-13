using System;
using FluentAssertions;
using NSubstitute;
using Tests.Infra.WebBrowserEngineTesterHelper.Context;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Universal.WebBrowserEnginesTests
{
    public abstract class WebViewDispatcher_Tests : TestBase
    {
        protected WebViewDispatcher_Tests(IBasicWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }

        [Fact]
        public void WebViewRun_CallAction()
        {
            using (Tester())
            {
                Action Do = Substitute.For<Action>();

                DoSafe(Do);

                Do.Received(1).Invoke();
            }
        }

        [Fact]
        public void WebViewRun_DoNotSwallowException()
        {
            using (Tester())
            {
                Action action = Substitute.For<Action>();
                action.When(d => d()).Do(_ => { throw new Exception(); });

                Action wf = () => DoSafe(action);

                wf.ShouldThrow<Exception>();

                action.Received(1).Invoke();
            }
        }
    }
}
