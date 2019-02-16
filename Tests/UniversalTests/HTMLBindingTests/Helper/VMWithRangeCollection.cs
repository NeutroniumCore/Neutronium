using System.Collections.ObjectModel;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class VmWithRangeCollection<T>
    {
        public ObservableRangeCollection<T> List { get;  } = new ObservableRangeCollection<T>();
    }
}
