using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using MoreCollection.Extensions;
using Neutronium.Core.Infra;
using Xunit;

namespace Neutronium.Core.Test.Infra
{
    public class ChainedTest
    {
        [Fact]
        public void Current_copies_value()
        {
            var current = new object();
            var res = new Chained<object>(current, null);
            res.Value.Should().Be(current);
        }

        [Fact]
        public void Next_originally_is_null()
        {
            var res = new Chained<object>(new object(), null);
            res.Next.Should().BeNull();
        }

        [Fact]
        public void Next_is_set_when_creating_new_chained()
        {
            var previous = new Chained<object>(new object(), null);
            var res = new Chained<object>(new object(), previous);
            previous.Next.Should().Be(res);
        }

        [Theory, AutoData]
        public void ForEach_Enumerates_collection(List<object> collection)
        {
            var first = FromEnumerable(collection);
            var visited = new List<object>();

            first?.ForEach(ob => visited.Add(ob));

            visited.Should().BeEquivalentTo(collection);
        }

        private class Number
        {
            public int Value { get; set; }
        }

        [Theory, AutoData]
        public void Reduce_reduces_collection(List<int> collectionOfInt)
        {
            var collection = collectionOfInt.Select(c => new Number {Value = c}).ToList();
            var first = FromEnumerable(collection);
            var expected = collectionOfInt.Aggregate(89, (ag, v) => ag - v);

            var value = first.Reduce<int>(ob => ob.Value, (ag, v) => ag - v, 89);

            value.Should().Be(expected);
        }

        private static Chained<T> FromEnumerable<T>(IEnumerable<T> collection) where T : class
        {
            var first = default(Chained<T>);
            var last = default(Chained<T>);
            collection.ForEach(item =>
            {
                last = new Chained<T>(item, last);
                first = first ?? last;
            });
            return first;
        }
    }
}
