using FsCheck;
using FsCheck.Xunit;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Test.Binding.Builder
{
    public class EntityDescriptorSpliterTests
    {
        public class MyGenerators
        {
            public static Arbitrary<EntityDescriptor<int>> ChildDescriptions()
            {
                var generatorIJSCSGlue = Gen.Constant(0).Select(index => NSubstitute.Substitute.For<IJSCSGlue>());

                var generatorChildDescription = Gen.zip(generatorIJSCSGlue, Gen.Choose(0, 100))
                                                    .Select(tuple => new ChildDescription<int>(tuple.Item2, tuple.Item1));

                var generator = Gen.zip(generatorIJSCSGlue, Gen.NonEmptyListOf(generatorChildDescription))
                                    .Select(tuple => new EntityDescriptor<int>(tuple.Item1, tuple.Item2));

                return Arb.From(generator);
            }
        }

        private EntityDescriptorSpliter<int> _Spliter;
        private const int _Limit = 5;

        public EntityDescriptorSpliterTests()
        {
            Arb.Register<MyGenerators>();
            _Spliter = new EntityDescriptorSpliter<int> { MaxCount = _Limit }; 
        }

        [Property]
        public Property Split_Returns_Ordered_All_Original_ChildrenDescriptor()
        {
            return Prop.ForAll<List<EntityDescriptor<int>>>((updates) => {
                var res = _Spliter.SplitParameters(updates);

                var elements = res.SelectMany(item => item).SelectMany(desc => desc.ChildrenDescription);

                return elements.SequenceEqual(updates.SelectMany(desc => desc.ChildrenDescription))
                            .Classify(updates.Count <= _Limit, "Single")
                            .Classify(updates.Count > _Limit && updates.Count<= _Limit * 2, "Splitted")
                            .Classify(updates.Count > _Limit * 2 && updates.Count <= _Limit * 3, "Double Splitted")
                            .Classify(updates.Count > _Limit * 3, "Triple Splitted and more");
            });
        }

        [Property]
        public Property Split_keeps_Father_Child_Relationship()
        {
            return Prop.ForAll<List<EntityDescriptor<int>>>((updates) => {
                var res = _Spliter.SplitParameters(updates);

                var elements = res.SelectMany(item => item)
                                    .GroupBy(desc => desc.Father)
                                    .Select(grouping => new EntityDescriptor<int>(grouping.Key, grouping.SelectMany(g => g.ChildrenDescription)));

                return elements.SequenceEqual(updates)
                            .Classify(updates.Count <= _Limit, "Single")
                            .Classify(updates.Count > _Limit && updates.Count <= _Limit * 2, "Splitted")
                            .Classify(updates.Count > _Limit * 2 && updates.Count <= _Limit * 3, "Double Splitted")
                            .Classify(updates.Count > _Limit * 3, "Triple Splitted and more");
            });
        }

        [Property]
        public Property Split_Limit_Size_Of_Returned_Data()
        {
            return Prop.ForAll<List<EntityDescriptor<int>>>((updates) => {
                var res = _Spliter.SplitParameters(updates);

                var sizes = res.Select(coll => coll.Select(item => item.Father)
                                   .Concat(coll.SelectMany(item => item.ChildrenDescription).Select(item => item.Child))
                                   .Count());

                return sizes.All(size => size<_Limit)
                            .Classify(updates.Count <= _Limit, "Single")
                            .Classify(updates.Count > _Limit && updates.Count <= _Limit * 2, "Splitted")
                            .Classify(updates.Count > _Limit * 2 && updates.Count <= _Limit * 3, "Double Splitted")
                            .Classify(updates.Count > _Limit * 3, "Triple Splitted and more");
            });
        }

        [Property]
        public Property Split_Optimize_Size_Of_Returned_Data()
        {
            return Prop.ForAll<List<EntityDescriptor<int>>>((updates) => {
                var res = _Spliter.SplitParameters(updates);

                var sizes = res.Select(coll => coll.Select(item => item.Father)
                                   .Concat(coll.SelectMany(item => item.ChildrenDescription).Select(item => item.Child))
                                   .Count());

                var maxSize = _Limit - 1;

                return sizes.Reverse().Skip(1).All(size => size == maxSize || size == maxSize -1 )
                            .Classify(updates.Count <= _Limit, "Single")
                            .Classify(updates.Count > _Limit && updates.Count <= _Limit * 2, "Splitted")
                            .Classify(updates.Count > _Limit * 2 && updates.Count <= _Limit * 3, "Double Splitted")
                            .Classify(updates.Count > _Limit * 3, "Triple Splitted and more");
            });
        }
    }
}
