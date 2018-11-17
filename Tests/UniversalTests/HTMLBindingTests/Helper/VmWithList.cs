using System.Collections;
using System.Collections.ObjectModel;
using Neutronium.Core.Test.Helper;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class VmWithList<T> : ViewModelTestBase
    {
        public VmWithList()
        {
            List = new ObservableCollection<T>();
        }
        public ObservableCollection<T> List { get; }
    }

    public class VmWithList : ViewModelTestBase
    {
        public VmWithList()
        {
            List = new ArrayList();
        }
        public ArrayList List { get; }
    }
}
