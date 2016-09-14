using System.Windows.Input;
using Neutronium.Example.ViewModel.Infra;

namespace Neutronium.Example.ViewModel
{
    public class Couple : ViewModelBase
    {
        public Couple()
        {
            MakeSelf = new RelayCommand(_ => DoMakeSelf());
            Duplicate = new RelayCommand(_ => DoDuplicate());   
        }

        private void DoMakeSelf()
        {
            var t = new ForNavigation.Person(){Couple=this};
            Two = t;         
        }

        private void DoDuplicate()
        {
            Two = One;
        }

        private Person _Person;
        public Person One
        {
            get { return _Person; }
            set
            {
                Set(ref _Person, value, "One");
            }
        }

        private Person _Person2;
        public Person Two
        {
            get { return _Person2; }
            set
            {
                Set(ref _Person2, value, "Two");
            }
        }

        public ICommand MakeSelf { get; }
        public ICommand Duplicate { get; }
    }
}
