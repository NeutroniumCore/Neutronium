using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NSubstitute;
using Xunit;
using FluentAssertions;

using MVVM.Cef.Glue.CefSession;
using System.Windows;
using System.Threading;
using MVVM.Cef.Glue.CefGlueHelper;

namespace MVVM.Cef.Glue.Test
{
    public class Test_CefTask_Action : MVVMCefGlue_Test_Base
    {
       
        [Fact]
        public void Test_CefTask_Action_Should_Call_Action()
        {
            using (var disp = Tester())
            {
                Action Do = Substitute.For<Action>();

                DoSafe(Do);

                Do.Received(1).Invoke();
            }
        }


        [Fact]
        public void Test_CefTask_Action_Should_Swallow_Exception()
        {
            using (var disp = Tester())
            {
                Action Do = Substitute.For<Action>();
                Do.When(d => d()).Do(_ => { throw new Exception(); });

                Action wf = () =>  DoSafe(Do);

                wf.ShouldThrow<Exception>();
               
                Do.Received(1).Invoke();
            }
        }
    }
}
