using System;

namespace MVVM.ViewModel.Example
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
