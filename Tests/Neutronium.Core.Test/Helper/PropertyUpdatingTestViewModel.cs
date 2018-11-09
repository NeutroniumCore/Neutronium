namespace Neutronium.Core.Test.Helper
{
    public class PropertyUpdatingTestViewModel : ViewModelTestBase
    {
        private string _Property1 = "1";
        public string Property1
        {
            get => _Property1;
            set
            {
                if (Set(ref _Property1, value))
                    Property2 = _Property1;
            }
        }

        private string _Property2 = "2";
        public string Property2
        {
            get => _Property2;
            set
            {
                if (Set(ref _Property2, value))
                    Property1 = _Property2;
            }
        }
    }
}
