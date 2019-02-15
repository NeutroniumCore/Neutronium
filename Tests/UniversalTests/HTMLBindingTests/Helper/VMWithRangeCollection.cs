using System.Collections.ObjectModel;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class VmWithRangeCollection
    {
        public ObservableRangeCollection<int> List { get;  } = new ObservableRangeCollection<int>();
    }
}
