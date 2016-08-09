using System;
using System.Threading.Tasks;
using FluentAssertions;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Navigation;
using MVVM.ViewModel;
using Xunit;

namespace IntegratedTest.Tests.WPF
{
    public abstract class Test_DoubleNavigation_Animation : Test_WpfComponent_Base<HTMLWindow> 
    {
        private readonly NavigationBuilder _NavigationBuilder;

        protected Test_DoubleNavigation_Animation(IWindowTestEnvironment windowTestEnvironment):
            base(windowTestEnvironment)
        {
            _NavigationBuilder = new NavigationBuilder();
        }

        protected override HTMLWindow GetNewHTMLControlBase(bool iDebug) 
        {
            return new HTMLWindow(_NavigationBuilder) { IsDebug = iDebug };
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
                var path = $"{GetType().Assembly.GetPath()}\\{"Navigation data\\index.html"}";

                _NavigationBuilder.RegisterAbsolute<VM>(path);
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
