using System;
using System.Windows;
using MVVM.ViewModel.Example;

namespace Example.CefGlue.Ko.SelectedItems
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var datacontext = new SkillsViewModel();
            datacontext.Skills.Add(new Skill() {Name="knockout", Type="Info" });
            datacontext.SelectedSkills.Add(datacontext.Skills[0]);
            DataContext = datacontext;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.WebControl.Dispose();
        } 
    }
}
