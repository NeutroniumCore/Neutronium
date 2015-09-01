using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;

using MVVM.ViewModel.Example;

namespace MVVM.Cef.Glue.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
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
            //this.wcBrowser.Dispose();
            base.OnClosed(e);
        }

    }
}
