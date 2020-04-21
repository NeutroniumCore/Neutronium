using System;
using System.Windows;
using System.Windows.Threading;
using Neutronium.Example.ViewModel;

namespace Example.ChromiumFX.Vue.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Skill _FirstSkill;
        private readonly DispatcherTimer _UpdateTimer;
        private readonly DispatcherTimer _GarbageCollectorTimer;

        public MainWindow()
        {
            this.InitializeComponent();

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

            DataContext = datacontext;

            _UpdateTimer = new DispatcherTimer 
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _UpdateTimer.Tick += (o, e) => Update(datacontext);
            _UpdateTimer.Start();

            _GarbageCollectorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(2000)
            };
            _GarbageCollectorTimer.Tick += (o, e) =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            };
            _GarbageCollectorTimer.Start();
        }

        private static void Update(Person person) => person.Count += 1;

        protected override void OnClosed(EventArgs e)
        {
            this.wcBrowser.Dispose();
            base.OnClosed(e);
        }
    }
}
