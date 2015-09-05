using MVVM.ViewModel.Example;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MVVM_Awesonium_UI
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
