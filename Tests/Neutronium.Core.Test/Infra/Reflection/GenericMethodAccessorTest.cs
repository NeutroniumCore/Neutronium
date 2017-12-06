using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Neutronium.Core.Infra.Reflection;
using Xunit;

namespace Neutronium.Core.Test.Infra.Reflection
{
    public class GenericMethodAccessorTest
    {
        private readonly GenericMethodAccessor _GenericMethodAccessorSimple;
        private readonly GenericMethodAccessor _GenericMethodAccessorDouble;

        public GenericMethodAccessorTest()
        {
            _GenericMethodAccessorSimple = GenericMethodAccessor.Get<GenericMethodAccessorTest>(nameof(Test));
            _GenericMethodAccessorDouble = GenericMethodAccessor.Get<GenericMethodAccessorTest>(nameof(Test2));
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(GenericMethodAccessor))]
        [InlineData(typeof(object))]
        public void Build_With_One_Generic_returns_valid_GenericMethodAccessor(Type genericType)
        {
            var method = _GenericMethodAccessorSimple.Build<Type>(genericType);
            var res = method.Invoke();
            res.Should().Be(genericType);
        }

        [Theory]
        [InlineData(typeof(int), typeof(string))]
        [InlineData(typeof(string), typeof(int))]
        [InlineData(typeof(GenericMethodAccessor), typeof(List<string>))]
        [InlineData(typeof(object), typeof(object))]
        public void Build_With_Two_Generics_returns_valid_GenericMethodAccessor(Type genericType, Type genericType2)
        {
            var expected = Tuple.Create(genericType, genericType2);
            var method = _GenericMethodAccessorDouble.Build<Tuple<Type, Type>>(genericType, genericType2);
            var res = method.Invoke();
            res.Should().Be(expected);
        }

        private static Type Test<T>()
        {
            return typeof(T);
        }

        private static Tuple<Type, Type> Test2<T1, T2>()
        {
            return Tuple.Create(typeof(T1), typeof(T2));
        }
    }
}
