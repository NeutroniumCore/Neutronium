using Neutronium.Example.ViewModel.Infra;

namespace Neutronium.Example.ViewModel
{
    public class Skill : ViewModelBase
    {
        private string _Type;
        public string Type
        {
            get { return _Type; }
            set
            {
                Set(ref _Type, value, "Type");
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                Set(ref _Name, value, "Name");
            }
        }
    }
}
