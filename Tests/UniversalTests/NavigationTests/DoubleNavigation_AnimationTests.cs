using System;
using System.Threading.Tasks;
using FluentAssertions;
using Neutronium.Core.Navigation;
using Neutronium.Example.ViewModel.Infra;
using Neutronium.WPF;
using Xunit;
using Tests.Infra.IntegratedContextTesterHelper.Window;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace IntegratedTest.Tests.WPF
{
    public abstract class DoubleNavigation_AnimationTests : WpfComponentTest_Base<HTMLWindow> 
    {
        private readonly NavigationBuilder _NavigationBuilder;

        protected DoubleNavigation_AnimationTests(IWindowContextProvider windowTestEnvironment):
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
        public async Task NavigateAsync_ShouldWaitForAnimationEnd()
        {        
            await Test(async (wpfnav, _) =>
            {
                var vm = new VM();
                wpfnav.Should().NotBeNull();

                _NavigationBuilder.Register<VM>(GetRelativePath(TestContext.AnimatedNavigation));

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
