using Neutronium.Example.ViewModel.Infra;

namespace Neutronium.Example.ViewModel
{
    public class ItemViewModel : ViewModelBase
    {
        private decimal _Price;
        public decimal Price
        {
            get { return _Price; }
            set { Set(ref _Price, value, "Price"); }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { Set(ref _Name, value, "Name"); }
        }
    }
}
