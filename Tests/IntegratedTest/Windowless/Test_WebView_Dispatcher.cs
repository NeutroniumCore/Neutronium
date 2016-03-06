using System;
using NSubstitute;
using Xunit;
using FluentAssertions;

namespace IntegratedTest.Windowless
{
    public abstract class Test_WebView_Dispatcher : MVVMCefCore_Test_Base
    {
        public Test_WebView_Dispatcher(IWindowLessHTMLEngineProvider testEnvironment): base(testEnvironment)
        {
        }

        [Fact]
        public void Test_CefTask_Action_Should_Call_Action()
        {
            using (Tester())
            {
                Action Do = Substitute.For<Action>();

                DoSafe(Do);

                Do.Received(1).Invoke();
            }
        }

        [Fact]
        public void Test_CefTask_Action_Should_Not_Swallow_Exception()
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
