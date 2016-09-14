using System;
using Neutronium.Example.ViewModel.Infra;

namespace Neutronium.Example.ViewModel
{
    public class DateInformation : ViewModelBase
    {
        private DateTime _Date;
        public DateTime Date
        {
            get { return _Date; }
            set
            {
                Set(ref _Date, value, "Date");
            }
        }
    }
}
