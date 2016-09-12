using System;
using Neutronium.Core.Infra;
using Neutronium.Core.Navigation;
using Neutronium.Example.ViewModel;
using Couple = Neutronium.Example.ViewModel.ForNavigation.Couple;
using Person = Neutronium.Example.ViewModel.ForNavigation.Person;

namespace Example.CefGlue.Ko.BasicNavigation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {

        private void SetUpRoute(INavigationBuilder iNavigationBuilder)
        {
            iNavigationBuilder.Register<Person>("HTMLUI\\index_one.html");
            iNavigationBuilder.Register<Couple>("HTMLUI\\index_couple.html");
        }


        public MainWindow()
        {      
            InitializeComponent();

            HTMLWindow.UseINavigable = true;
            SetUpRoute(HTMLWindow.NavigationBuilder);

            var datacontext = new Couple();
            var my = new Person()
            {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new Local() { City = "Florianopolis", Region = "SC" }
            };
            my.Couple = datacontext;
            datacontext.One = my;

            HTMLWindow.NavigateAsync(datacontext).DoNotWait();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.HTMLWindow.Dispose();
        } 
    }
}
