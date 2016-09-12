using System;
using Neutronium.Example.ViewModel.Infra;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.Example.ViewModel
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
