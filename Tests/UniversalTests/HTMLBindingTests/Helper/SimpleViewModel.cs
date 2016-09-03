using MVVM.ViewModel;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    internal class SimpleViewModel : ViewModelBase 
    {
        private string _Name;
        public string Name 
        {
            get { return _Name; }
            set { Set(ref _Name, value, "Name"); }
        }
    }
}
