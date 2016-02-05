using MVVM.Component;
using System;

namespace MVVM.ViewModel.Example
{
    public class FakeFactory<Tin,Tout> : ViewModelBase
    {
        public FakeFactory(Func<Tin, Tout> iFact)
        {
            CreateObject = RelayResultCommand.Create(iFact);
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
