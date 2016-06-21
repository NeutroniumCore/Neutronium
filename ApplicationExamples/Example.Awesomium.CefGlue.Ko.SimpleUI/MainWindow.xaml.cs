using System;
using System.Windows;
using MVVM.ViewModel.Example;

namespace Example.Awesomium.CefGlue.Ko.SimpleUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var datacontext = new Person()
            {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new Local() { City = "Florianopolis", Region = "SC" },
                PersonalState = PersonalState.Married
            };

            var _FirstSkill = new Skill() { Name = "Langage", Type = "French" };

            datacontext.Skills.Add(_FirstSkill);
            datacontext.Skills.Add(new Skill() { Name = "Info", Type = "C++" });

             DataContext = datacontext;
            //_Person = datacontext;
        }

        protected override void OnClosed(EventArgs e)
        {
            this.wcBrowser.Dispose();
            this.wcBrowser2.Dispose();
            base.OnClosed(e);
        }
    }
}
