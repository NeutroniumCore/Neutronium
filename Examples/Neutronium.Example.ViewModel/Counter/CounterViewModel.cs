using Neutronium.Example.ViewModel.Infra;
using System.Windows.Input;
using System.Threading.Tasks;
using System;
using System.Linq;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.Example.ViewModel.Counter
{
    public class CounterViewModel : ViewModelBase
    {
        private const int _Leaves = 30;
        private const int _Position = 3;
        public int CounterRead { get; private set; }

        private int _Counter;
        public int Counter
        {
            get
            {
                CounterRead++;
                return _Counter;
            }
            set
            {
                Set(ref _Counter, value, "Counter");
            }
        }

        private FakeViewModel _Big;
        public FakeViewModel Big
        {
            get { return _Big; }
            private set
            {
                Set(ref _Big, value, "Big");
            }
        }

        private string _State = "Done";
        public string State
        {
            get { return _State; }
            set { Set(ref _State, value, "State"); }
        }

        public ICommand Count { get; }
        public ICommand BuildBigModel { get; }

        public IProgress<int> Progess { get; set; }

        public CounterViewModel()
        {
            Count = new RelaySimpleCommand(DoCount);
            BuildBigModel = new RelaySimpleCommand(DoBuildBigModel);
        }

        private void DoBuildBigModel()
        {
            Big = BuildBigVm(_Leaves, _Position);
        }

        protected FakeViewModel BuildBigVm(int leaves = 50, int position = 3)
        {
            var children = position == 0 ? null : Enumerable.Range(0, leaves).Select(i => BuildBigVm(leaves, position - 1));
            return new FakeViewModel(children);
        }

        private async void DoCount()
        {
            State = "Running";
            var init = _Counter;
            await Task.Run(() =>
            {
                for (var i = 0; i < 10000; i++)
                {
                    Progess?.Report(i + init);
                }
            });
            State = "Done";
        }
    }
}
