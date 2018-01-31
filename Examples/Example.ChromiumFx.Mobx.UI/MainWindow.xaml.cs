using Neutronium.Example.ViewModel;
using System;
using System.Windows;

namespace Example.ChromiumFx.Mobx.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool Debug => true;
        public Uri Uri => new Uri("pack://application:,,,/View/mainview/build/index.html");

        public MainWindow()
        {
            Initialized += MainWindow_Initialized;
            InitializeComponent();
        }

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            var datacontext = new Person()
            {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new Local() { City = "Florianopolis", Region = "SC" },
                PersonalState = PersonalState.Married
            };

            var firstSkill = new Skill() { Name = "Langage", Type = "French" };
            datacontext.Skills.Add(firstSkill);
            datacontext.Skills.Add(new Skill() { Name = "Info", Type = "C++" });

            DataContext = datacontext;
        }
    }
}
