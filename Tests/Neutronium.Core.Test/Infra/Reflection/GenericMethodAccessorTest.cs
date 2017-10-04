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
        private readonly GenericMethodAccessor _GenericMethodAccessor;

        public GenericMethodAccessorTest()
        {
            _GenericMethodAccessor = GenericMethodAccessor.Get<GenericMethodAccessorTest>(nameof(Test));
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(GenericMethodAccessor))]
        [InlineData(typeof(object))]
        public void Get_returns_valid_GenericMethodAccessor(Type genericType)
        {
            var method = _GenericMethodAccessor.Build<Type>(genericType);
            var res = method.Invoke();
            res.Should().Be(genericType);
        }

        private static Type Test<T>()
        {
            return typeof(T);
        }
    }
}
