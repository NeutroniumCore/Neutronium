using Neutronium.Example.ViewModel.Infra;
using System.Windows.Input;
using System.Threading.Tasks;
using System;

namespace Neutronium.Example.ViewModel.Counter
{
    public class CounterViewModel : ViewModelBase
    {
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

        public ICommand Count { get; }

        public IProgress<int> Progess { get; set; }

        public CounterViewModel()
        {
            Count = new RelayCommand(DoCount);
        }

        private void DoCount()
        {
            Task.Run(() =>
            {
                for(var i=0; i<10000; i++)
                {
                    Progess?.Report(i);
                }
            });
        }
    }
}
