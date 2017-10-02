using System.Collections.ObjectModel;
using Neutronium.Example.ViewModel.Infra;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.Example.ViewModel
{

    public class CardViewModel : ViewModelBase
    {
        public CardViewModel()
        {
            Insert = new RelaySimpleCommand<ItemViewModel>((it) => _Items.Insert(_Items.IndexOf(it),new ItemViewModel()));
            Remove = new RelaySimpleCommand<ItemViewModel>((it) => _Items.Remove(it));
        }

        private ObservableCollection<ItemViewModel> _Items= new ObservableCollection<ItemViewModel>();
        public ObservableCollection<ItemViewModel> Items
        {
            get { return _Items; }
        }

        public ISimpleCommand<ItemViewModel> Insert { get; }

        public ISimpleCommand<ItemViewModel> Remove { get; }
    }
}
