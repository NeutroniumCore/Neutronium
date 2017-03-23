using FluentAssertions;
using Internal.Test.Namespace;
using Neutronium.Core.Navigation.Routing;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;


namespace Internal.Test.Namespace
{
    public class ClassTeste { }
}


namespace Neutronium.Core.Test.Navigation.Routing
{
    public class TypeProviderTest 
    {
        public interface IInterface { }

        public interface IInterface2 { }

        public class Class1: IInterface { }

        public class Class2 : IInterface, IInterface2 { }

        public class Class3 : IInterface { }

        public class Class2WithFunnyName { }

        public class WithFunnyNameStarting { }

        private IEnumerable<Type> TestAssemblyTypes => typeof(TypeProviderTest).Assembly.GetTypes();

        [Fact]
        public void GetTypesFromSameAssembly_object_returns_types_of_assembly()
        {
            var typeProvider = this.GetTypesFromSameAssembly();
            typeProvider.Types.Should().BeEquivalentTo(TestAssemblyTypes);
        }

        [Fact]
        public void GetTypesFromSameAssembly_type_returns_types_of_assembly()
        {
            var typeProvider = GetType().GetTypesFromSameAssembly();
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
            var typeProvider = this.GetTypesFromSameAssembly().Implementing<IInterface>();
            typeProvider.Types.Should().BeEquivalentTo(typeof(Class1), typeof(Class2), typeof(Class3));
        }

        [Fact]
        public void Except_filters_type()
        {
            var typeProvider = this.GetTypesFromSameAssembly().Implementing<IInterface>().Except(typeof(Class1), typeof(Class2));
            typeProvider.Types.Should().BeEquivalentTo(typeof(Class3));
        }

        [Fact]
        public void Implementing_filters_type_by_inheritance_case_2()
        {
            var typeProvider = this.GetTypesFromSameAssembly().Implementing<IInterface2>();
            typeProvider.Types.Should().BeEquivalentTo(typeof(Class2));
        }

        [Fact]
        public void WithNameEndingWith_filters_type_by_name()
        {
            var typeProvider = this.GetTypesFromSameAssembly().WithNameEndingWith("FunnyName");
            typeProvider.Types.Should().BeEquivalentTo(typeof(Class2WithFunnyName));
        }

        [Fact]
        public void InNamespace_filters_type_by_namespace()
        {
            var typeProvider = this.GetTypesFromSameAssembly().InNamespace("Internal.Test.Namespace");
            typeProvider.Types.Should().BeEquivalentTo(typeof(ClassTeste));
        }
    }
}
