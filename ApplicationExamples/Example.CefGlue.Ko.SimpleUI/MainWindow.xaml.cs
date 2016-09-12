using System;
using System.Windows;
using Neutronium.Example.ViewModel;

namespace Example.CefGlue.Ko.SimpleUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Skill _FirstSkill;
        private Person _Person;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = new Person()
                {
                    Name = "O Monstro",
                    LastName = "Desmaisons",
                    Local = new Local() { City = "Florianopolis", Region = "SC" },
                    PersonalState = PersonalState.Married
                };

            _FirstSkill = new Skill() { Name = "Langage", Type = "French" };

            datacontext.Skills.Add(_FirstSkill);
            datacontext.Skills.Add(new Skill() { Name = "Info", Type = "C++" });

            Window w = sender as Window;
            w.DataContext = datacontext;
            _Person = datacontext;
        }

        protected override void OnClosed(EventArgs e)
        {
            this.wcBrowser.Dispose();
            base.OnClosed(e);
        }
    }
}
