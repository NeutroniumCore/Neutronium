using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Neutronium.Core.Test.Helper
{
    public class BasicListTestViewModel : ViewModelTestBase
    {
        public IList<BasicTestViewModel> Children { get; } = new ObservableCollection<BasicTestViewModel>();
    }

}
