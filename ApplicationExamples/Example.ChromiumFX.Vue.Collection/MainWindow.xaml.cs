using System;
using System.Windows;
using Neutronium.Example.ViewModel;

namespace Example.ChromiumFX.Vue.Collection
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = new SkillsViewModel();
            datacontext.Skills.Add(new Skill() { Name = "Vue.js", Type = "Info" });
            datacontext.SelectedSkills.Add(datacontext.Skills[0]);
            datacontext.MainSkill = datacontext.Skills[0];
            DataContext = datacontext;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.WebControl.Dispose();
        }
    }
}
