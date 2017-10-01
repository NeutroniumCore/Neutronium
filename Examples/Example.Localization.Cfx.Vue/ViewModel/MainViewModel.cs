using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Neutronium.Example.ViewModel.Infra;

namespace Example.Localization.Cfx.Vue.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private Dictionary<string, string> _Localization;
        public Dictionary<string, string> Localization
        {
            get { return _Localization; }
            private set { Set(ref _Localization, value, nameof(Localization)); }
        }

        private string _Langage;
        public string Langage
        {
            get { return _Langage; }
            set
            {
                if (Set(ref _Langage, value, nameof(Langage)))
                {
                    UpdateLangage();
                }           
            }
        }

        private void UpdateLangage()
        {
            var rs = Resource.ResourceManager.GetResourceSet(new CultureInfo(_Langage), true, true);
            Localization = rs.Cast<DictionaryEntry>().ToDictionary(dicEntry => (string)dicEntry.Key, dicEntry => (string)dicEntry.Value);            
        }

        public string[] Langages => new[] {"en-US", "pt-BR", "fr-FR"};

        public MainViewModel()
        {
            Langage = "en-US";
        }
    }
}
