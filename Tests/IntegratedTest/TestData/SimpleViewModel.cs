using MVVM.ViewModel;

namespace IntegratedTest.TestData 
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
