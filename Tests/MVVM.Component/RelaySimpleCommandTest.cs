using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NSubstitute;
using Xunit;
using FluentAssertions;

using MVVM.Component.Infra;

namespace MVVM.Cef.Glue.Test
{
    public class TaskHelperTest
    {
        [Fact]
        public void SuccessShoulbBeSuccess()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            tcs.SetResult(1);

            var target = tcs.Task.Convert();
            target.Result.Should().Be(1);
        }


        [Fact]
        public void CancelShoulbBeCancel()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            tcs.SetCanceled();

            var target = tcs.Task.Convert();
            target.IsCanceled.Should().BeTrue();
        }


        [Fact]
        public void ExceptionShoulbBeException()
        {
            var ex = new Exception();
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            tcs.SetException(ex);

            var target = tcs.Task.Convert();
            target.IsFaulted.Should().BeTrue();
            target.Exception.Flatten().InnerException.Should().Be(ex);
        }
      
    }
}
