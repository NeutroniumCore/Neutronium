using FluentAssertions;
using Neutronium.Core.Infra.VM;
using Xunit;

namespace Neutronium.Core.Test.Infra.VM
{
    public class NotifyPropertyChangedBaseTest
    {
        public class Vm : NotifyPropertyChangedBase
        {
            private int _Value;
            public int Value { get { return _Value; } set { Set(ref _Value, value, "Value"); } }
        }

        [Fact]
        public void Test_NotifyPropertyChangedBase()
        {
            var vm = new Vm();

            using (var monitor = vm.Monitor())
            {
                vm.Value.Should().Be(0);
                vm.Value = 0;
                vm.Value.Should().Be(0);
                monitor.Should().NotRaisePropertyChangeFor(t => t.Value);

                vm.Value = 2;
                vm.Value.Should().Be(2);
                monitor.Should().RaisePropertyChangeFor(t => t.Value);
            }
        }
    }
}
