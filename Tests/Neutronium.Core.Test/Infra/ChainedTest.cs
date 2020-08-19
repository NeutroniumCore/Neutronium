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

        [Theory, AutoData]
        public void MapFilter_maps_collection(List<int> collectionOfInt)
        {
            int Transform(int value) => value * 2;
            var first = FromEnumerable(collectionOfInt);
            var expected = collectionOfInt.Select(Transform);

            var res = first.MapFilter(Transform);

            ToEnumerable(res).Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public void MapFilter_filter_collection(List<int> collectionOfInt)
        {
            int Transform(int value) => value;
            bool Filter(int value) => value % 2 == 0;

            var first = FromEnumerable(collectionOfInt);
            var expected = collectionOfInt.Where(Filter);

            var res = first.MapFilter(Transform, Filter);

            ToEnumerable(res).Should().BeEquivalentTo(expected);
        }

        [Theory, AutoData]
        public void MapFilter_returns_null_when_there_is_no_element(List<int> collectionOfInt)
        {
            int Transform(int value) => value;
            bool Filter(int value) => false;
            var first = FromEnumerable(collectionOfInt);

            var res = first.MapFilter(Transform, Filter);

            res.Should().BeNull();
        }

        private static Chained<T> FromEnumerable<T>(IEnumerable<T> collection)
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

        private static IEnumerable<T> ToEnumerable<T>(Chained<T> collection)
        {
            var current = collection;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }
    }
}
