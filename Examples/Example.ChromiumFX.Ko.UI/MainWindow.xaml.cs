using Neutronium.Example.ViewModel;

namespace Example.ChromiumFX.Ko.UI 
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow  
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

            var firstSkill = new Skill() { Name = "Langage", Type = "French" };

            datacontext.Skills.Add(firstSkill);
            datacontext.Skills.Add(new Skill() { Name = "Info", Type = "C++" });

            DataContext = datacontext;
        }
    }
}
