using FsCheck;
using FsCheck.Xunit;
using Neutronium.Core.Infra;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Test.Infra
{
    public class SlicerTest
    {
        private readonly int _MaxCount = 5;

        [Property]
        public Property Slice_returns_original_elements()
        {
            return Prop.ForAll<List<int>>(array =>
            {
                var slicer = new Slicer<int>(array, _MaxCount);
                var slices = slicer.Slice();

                return array.SequenceEqual(slices.SelectMany(sl => sl));
            });
        }

        [Property]
        public Property Slice_returns_collection_with_maxCount_elements()
        {
            return Prop.ForAll<List<int>>(array =>
            {
                var slicer = new Slicer<int>(array, _MaxCount);
                var slices = slicer.Slice();

                return slices.All(sl => sl.Length <= _MaxCount);
            });
        }

        [Property]
        public Property Slice_returns_first_collections_with_maxCount_elements()
        {
            return Prop.ForAll<List<int>>(array =>
            {
                var slicer = new Slicer<int>(array, _MaxCount);
                var slices = slicer.Slice();

                return slices.Reverse().Skip(1).All(sl => sl.Length == _MaxCount);
            });
        }
    }
}
