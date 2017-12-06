using System;
using Neutronium.Example.ViewModel.Infra;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.Core.Test.Helper
{
    public class FakeFactory<TIn, TOut> : ViewModelBase
    {
        public FakeFactory(Func<TIn, TOut> factory)
        {
            CreateObject = RelayResultCommand.Create(factory);
        }

        public IResultCommand<TIn, TOut> CreateObject { get; set; }

        private string _Name = null;
        public string Name
        {
            get { return _Name; }
            set { Set(ref _Name, value, "Name"); }
        }
    }

    public class FakeFactory<TOut> : ViewModelBase
    {
        public FakeFactory(Func<TOut> factory)
        {
            CreateObject = RelayResultCommand.Create(factory);
        }

        public IResultCommand<TOut> CreateObject { get; set; }

        private string _Name = null;
        public string Name
        {
            get { return _Name; }
            set { Set(ref _Name, value, "Name"); }
        }
    }
}
