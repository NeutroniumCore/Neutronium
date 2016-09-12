using Xunit;
using FluentAssertions;
using Neutronium.Core.Infra.VM;

namespace MVVM.HTML.Core.Test
{
    public class NotifyPropertyChangedBaseTest
    {
        public class vm : NotifyPropertyChangedBase
        {
            private int _Value;
            public int Value { get { return _Value; } set { Set(ref _Value, value, "Value"); } }
        }

        [Fact]
        public void Test_NotifyPropertyChangedBase()
        {
            var vm = new vm();

            vm.MonitorEvents();

            vm.Value.Should().Be(0);
            vm.Value = 0;
            vm.Value.Should().Be(0);
            vm.ShouldNotRaisePropertyChangeFor(t => t.Value);

            vm.Value = 2;
            vm.Value.Should().Be(2);
            vm.ShouldRaisePropertyChangeFor(t => t.Value);
        }
    }
}
