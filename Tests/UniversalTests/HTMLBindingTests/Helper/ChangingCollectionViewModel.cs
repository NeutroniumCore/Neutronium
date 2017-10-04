using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Neutronium.Example.ViewModel.Infra;
using Neutronium.MVVMComponents.Relay;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class ChangingCollectionViewModel : ViewModelBase
    {
        public ChangingCollectionViewModel()
        {
            _Items = new ObservableCollection<int>(new int[]{1,2,3,4,5});

            Replace = new RelaySimpleCommand(DoReplace);
            Add = new RelaySimpleCommand(DoAdd);
        }

        private void DoReplace()
        {
            Random rnd = new Random();
            Items = new ObservableCollection<int>();
        }

        private void DoAdd()
        {
            Items.Add(89);
        }

        public ICommand Replace { get; private set; }

        public ICommand Add { get; private set; }

        private ObservableCollection<int> _Items;
        public ObservableCollection<int> Items
        {
            get { return _Items; }
            set { Set(ref _Items, value, "Items"); }
        }
    }
}
