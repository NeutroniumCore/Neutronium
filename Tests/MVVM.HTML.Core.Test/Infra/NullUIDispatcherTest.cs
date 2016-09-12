using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Neutronium.Core.Infra;

namespace MVVM.HTML.Core.Test
{
    public class NullUIDispatcherTest
    {
        private readonly NullUIDispatcher _NullUIDispatcher;
        public NullUIDispatcherTest()
        {
            _NullUIDispatcher = new NullUIDispatcher();
        }

        [Fact]
        public void Test_NullUIDispatcher_Evaluate()
        {
            int res = _NullUIDispatcher.Evaluate(() => 3);
            res.Should().Be(3);
        }

        [Fact]
        public async Task Test_NullUIDispatcher_EvaluateAsync()
        {
            int res = await _NullUIDispatcher.EvaluateAsync(() => 3);
            res.Should().Be(3);
        }
    }
}
