using FluentAssertions;
using Neutronium.Core.Navigation.Routing;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Neutronium.Core.Test.Navigation.Routing
{
    public class TypeProviderTest 
    {
        public interface IInterface { }

        public interface IInterface2 { }

        public class Class1: IInterface { }

        public class Class2 : IInterface, IInterface2 { }

        private static IEnumerable<Type> TestAssemblyTypes => typeof(TypeProviderTest).Assembly.GetTypes();

        [Fact]
        public void GetAllTypesFromAssembly_object_returns_types_of_assembly()
        {
            var typeProvider = this.GetAllTypesFromAssembly();
            typeProvider.Types.Should().BeEquivalentTo(TestAssemblyTypes);
        }

        [Fact]
        public void GetAllTypesFromAssembly_type_returns_types_of_assembly()
        {
            var typeProvider = GetType().GetAllTypesFromAssembly();
            typeProvider.Types.Should().BeEquivalentTo(TestAssemblyTypes);
        }

        [Fact]
        public void GetAllTypes_returns_types_of_assembly()
        {
            var typeProvider = Assembly.GetExecutingAssembly().GetAllTypes();
            typeProvider.Types.Should().BeEquivalentTo(TestAssemblyTypes);
        }

        [Fact]
        public void Implementing_filters_type_by_inheritance() 
        {
            var typeProvider = this.GetAllTypesFromAssembly().Implementing<IInterface>();
            typeProvider.Types.Should().BeEquivalentTo(typeof(Class1), typeof(Class2));
        }

        [Fact]
        public void Implementing_filters_type_by_inheritance_case_2()
        {
            var typeProvider = this.GetAllTypesFromAssembly().Implementing<IInterface2>();
            typeProvider.Types.Should().BeEquivalentTo(typeof(Class2));
        }
    }
}
