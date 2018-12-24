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
            get => _Localization;
            private set => Set(ref _Localization, value, nameof(Localization));
        }

        private string _Language;
        public string Language
        {
            get => _Language;
            set
            {
                if (Set(ref _Language, value, nameof(Language)))
                {
                    UpdateLanguage();
                }           
            }
        }

        private void UpdateLanguage()
        {
            var rs = Resource.ResourceManager.GetResourceSet(new CultureInfo(_Language), true, true);
            Localization = rs.Cast<DictionaryEntry>().ToDictionary(dicEntry => (string)dicEntry.Key, dicEntry => (string)dicEntry.Value);            
        }

        public string[] Languages => new[] {"en-US", "pt-BR", "fr-FR"};

        public MainViewModel()
        {
            Language = "en-US";
        }
    }
}
