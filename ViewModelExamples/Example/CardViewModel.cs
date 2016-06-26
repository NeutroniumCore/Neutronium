using MVVM.Component;
using System.Collections.ObjectModel;
using MVVM.Component.Relay;

namespace MVVM.ViewModel.Example
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

        public ISimpleCommand Insert { get; }

        public ISimpleCommand Remove { get; }
    }
}
