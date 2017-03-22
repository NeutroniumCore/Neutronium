using FluentAssertions;
using Neutronium.Core.Navigation.Routing;
using Xunit;

namespace Neutronium.Core.Test.Navigation.Routing
{
    public class TypeProviderTest 
    {
        public interface IInterface { }

        public class Class1: IInterface { }

        public class Class2 : IInterface { }

        [Fact]
        public void Implementing_filter_type_by_inheritance() 
        {
            var typeProvider = this.GetAllTypesFromAssembly().Implementing<IInterface>();
            typeProvider.Types.Should().BeEquivalentTo(typeof(Class1), typeof(Class2));
        } 
    }
}
