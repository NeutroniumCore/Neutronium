using System;
using FluentAssertions;
using NSubstitute;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace IntegratedTest.Tests.Windowless
{
    public abstract class Test_WebView_Dispatcher : IntegratedTestBase
    {
        protected Test_WebView_Dispatcher(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
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
