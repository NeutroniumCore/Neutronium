using System;
using System.Windows;
using Neutronium.Example.ViewModel;

namespace Example.ChromiumFX.Vue.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            var datacontext = new Person() {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new Local() { City = "Florianopolis", Region = "SC" },
                PersonalState = PersonalState.Married
            };

            _FirstSkill = new Skill() { Name = "Langage", Type = "French" };

            datacontext.Skills.Add(_FirstSkill);
            datacontext.Skills.Add(new Skill() { Name = "Info", Type = "C++" });

            DataContext = datacontext;
            _Person = datacontext;
        }

        private Skill _FirstSkill;
        private Person _Person;


        protected override void OnClosed(EventArgs e)
        {
            this.wcBrowser.Dispose();
            base.OnClosed(e);
        }
    }
}
