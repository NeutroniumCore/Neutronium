using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

using MVVM.ViewModel;
using HTML_WPF.Component;
using MVVM.HTML.Core.Navigation;
using MVVM.HTML.Core.Infra;
using IntegratedTest.WPF.Infra;

namespace IntegratedTest.WPF
{
    public abstract class Test_DoubleNavigation_Animation : Test_WpfComponent_Base<HTMLWindow> 
    {
        private readonly NavigationBuilder _INavigationBuilder;

        protected Test_DoubleNavigation_Animation(IWindowTestEnvironment windowTestEnvironment):
            base(windowTestEnvironment)
        {
            _INavigationBuilder = new NavigationBuilder();
        }

        protected override HTMLWindow GetNewHTMLControlBase(bool iDebug) 
        {
            return new HTMLWindow(_INavigationBuilder) { IsDebug = iDebug };
        }

        public class VM : ViewModelBase, INavigable
        {
            public INavigationSolver Navigation { get; set; }
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Simple()
        {        
            await Test(async (wpfnav, _) =>
            {
                var vm = new VM();
                wpfnav.Should().NotBeNull();
                var path = string.Format("{0}\\{1}",GetType().Assembly.GetPath(), "Navigation data\\index.html");;
                _INavigationBuilder.RegisterAbsolute<VM>(path);
                wpfnav.UseINavigable = true;

                DateTime? opened = null;
                DisplayEvent de = null;
                wpfnav.OnDisplay += (o, e) => { opened = DateTime.Now; de = e; };

                await wpfnav.NavigateAsync(vm);
                var nav = DateTime.Now;

                await Task.Delay(3000);

                de.Should().NotBeNull();
                de.DisplayedViewModel.Should().Be(vm);
                opened.HasValue.Should().BeTrue();
                opened.Value.Subtract(nav).Should().BeCloseTo(TimeSpan.FromSeconds(2), 100);
            });
        }
    }
}
