using MVVM.Component;
using System;

namespace MVVM.ViewModel.Example
{
    public class Factory : ViewModelBase
    {
        public Factory()
        {
            CreateObject = new RelayResultCommand<string, Person>(n =>Fact(n));
        }

        private Person  Fact(string n)
        {
            if (n == null)
                throw new NullReferenceException();

            return new Person() { LastName = n + "99" };
        }

        public IResultCommand CreateObject { get; set; }

        private string _Name=null;
        public string Name
        {
            get { return _Name; }
            set { Set(ref _Name, value, "Name"); }
        }
    }
}
