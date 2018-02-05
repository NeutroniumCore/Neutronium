using Neutronium.Example.ViewModel;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Example.ChromiumFx.Mobx.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool Debug => App.MainApplication.Debug;
        public Uri Uri => App.MainApplication.Uri;

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
            var timer = new DispatcherTimer 
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += (o, _) => Update(datacontext);
            timer.Start();

            DataContext = datacontext;
        }

        private static void Update(Person person) => person.Count += 1;
    }
}
