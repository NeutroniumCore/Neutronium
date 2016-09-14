using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Neutronium.Example.ViewModel.Infra;

namespace Example.CefGlue.Ko.CollectionTest.ViewModel
{
    public class ChangingCollectionViewModel : ViewModelBase
    {
        public ChangingCollectionViewModel()
        {
            _Items = new ObservableCollection<int>(new int[] { 1, 2, 3, 4, 5 });

            Replace = new RelayCommand(DoReplace);
            Add = new RelayCommand(DoAdd);
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

        public ICommand Replace { get; }

        public ICommand Add { get; }

        private ObservableCollection<int> _Items;
        public ObservableCollection<int> Items
        {
            get { return _Items; }
            set { Set(ref _Items, value, "Items"); }
        }
    }

}
