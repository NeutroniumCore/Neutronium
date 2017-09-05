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
            public static Arbitrary<ObjectDescriptor> ChildDescriptions()
            {
                var generatorIjsCsGlue = Gen.Constant(0).Select(index => NSubstitute.Substitute.For<IJsCsGlue>());

                var generatorChildDescription = Gen.zip(generatorIjsCsGlue, Arb.Default.String().Generator)
                                                    .Select(tuple => new AttributeDescription(tuple.Item2, tuple.Item1));

                var generator = Gen.zip(generatorIjsCsGlue, Gen.NonEmptyListOf(generatorChildDescription))
                                    .Select(tuple => new ObjectDescriptor(tuple.Item1, tuple.Item2.ToArray()));

                return Arb.From(generator);
            }
        }

        private readonly EntityDescriptorSpliter _Spliter;
        private const int Limit = 5;

        public EntityDescriptorSpliterTests()
        {
            Arb.Register<MyGenerators>();
            _Spliter = new EntityDescriptorSpliter { MaxCount = Limit }; 
        }

        [Property]
        public Property Split_Returns_Ordered_All_Original_ChildrenDescriptor()
        {
            return Prop.ForAll<List<ObjectDescriptor>>((updates) => {
                var res = _Spliter.SplitParameters(updates);

                var elements = res.SelectMany(item => item).SelectMany(desc => desc.ChildrenDescription);

                return elements.SequenceEqual(updates.SelectMany(desc => desc.ChildrenDescription))
                            .Classify(updates.Count <= Limit, "Single")
                            .Classify(updates.Count > Limit && updates.Count<= Limit * 2, "Splitted")
                            .Classify(updates.Count > Limit * 2 && updates.Count <= Limit * 3, "Double Splitted")
                            .Classify(updates.Count > Limit * 3, "Triple Splitted and more");
            });
        }

        [Property]
        public Property Split_keeps_Father_Child_Relationship()
        {
            return Prop.ForAll<List<ObjectDescriptor>>((updates) => {
                var res = _Spliter.SplitParameters(updates);

                var elements = res.SelectMany(item => item)
                                    .GroupBy(desc => desc.Father)
                                    .Select(grouping => new ObjectDescriptor(grouping.Key, grouping.SelectMany(g => g.ChildrenDescription).ToArray()));

                return elements.SequenceEqual(updates)
                            .Classify(updates.Count <= Limit, "Single")
                            .Classify(updates.Count > Limit && updates.Count <= Limit * 2, "Splitted")
                            .Classify(updates.Count > Limit * 2 && updates.Count <= Limit * 3, "Double Splitted")
                            .Classify(updates.Count > Limit * 3, "Triple Splitted and more");
            });
        }

        [Property]
        public Property Split_Limit_Size_Of_Returned_Data()
        {
            return Prop.ForAll<List<ObjectDescriptor>>((updates) => {
                var res = _Spliter.SplitParameters(updates);

                var sizes = res.Select(coll => coll.Select(item => item.Father)
                                   .Concat(coll.SelectMany(item => item.ChildrenDescription).Select(item => item.Glue))
                                   .Count());

                return sizes.All(size => size<Limit)
                            .Classify(updates.Count <= Limit, "Single")
                            .Classify(updates.Count > Limit && updates.Count <= Limit * 2, "Splitted")
                            .Classify(updates.Count > Limit * 2 && updates.Count <= Limit * 3, "Double Splitted")
                            .Classify(updates.Count > Limit * 3, "Triple Splitted and more");
            });
        }

        [Property]
        public Property Split_Optimize_Size_Of_Returned_Data()
        {
            return Prop.ForAll<List<ObjectDescriptor>>((updates) => {
                var res = _Spliter.SplitParameters(updates);

                var sizes = res.Select(coll => coll.Select(item => item.Father)
                                   .Concat(coll.SelectMany(item => item.ChildrenDescription).Select(item => item.Glue))
                                   .Count());

                var maxSize = Limit - 1;

                return sizes.Reverse().Skip(1).All(size => size == maxSize || size == maxSize -1 )
                            .Classify(updates.Count <= Limit, "Single")
                            .Classify(updates.Count > Limit && updates.Count <= Limit * 2, "Splitted")
                            .Classify(updates.Count > Limit * 2 && updates.Count <= Limit * 3, "Double Splitted")
                            .Classify(updates.Count > Limit * 3, "Triple Splitted and more");
            });
        }
    }
}
