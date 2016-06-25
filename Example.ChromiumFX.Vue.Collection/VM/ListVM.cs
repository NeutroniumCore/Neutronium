using MVVM.ViewModel;
using MVVM.ViewModel.Example;
using System.Collections.ObjectModel;
using MVVM.HTML.Core.Infra;
using System.Windows.Input;
using MVVM.ViewModel.Infra;

namespace Example.ChromiumFX.Vue.Collection.VM
{
    public class ListVM : ViewModelBase
    {
        private int _Count;
        public ObservableCollection<Person> Persons { get; } = new ObservableCollection<Person>();

        public ListVM(params Person[] persons)
        {
            persons.ForEach(p => Persons.Add(p));
            Add = new RelayCommand(DoAdd);
        }

        private void DoAdd()
        {
            Persons.Add(new Person() { Name = $"Person{_Count++}" });
        }

        public ICommand Add { get; set; }
    }
}
