using System;
using System.Threading;
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
        public void Test_WPFBrowserNavigator_Simple()
        {
            var vm = new VM();
            Test((wpfnav, WindowTest)
                =>
                {
                    wpfnav.Should().NotBeNull();
                    var path = string.Format("{0}\\{1}",GetType().Assembly.GetPath(), "Navigation data\\index.html");;
                    _INavigationBuilder.RegisterAbsolute<VM>(path);
                    wpfnav.UseINavigable = true;

                    var mre = new ManualResetEvent(false);
                    DateTime? nav = null;
                    DateTime? Opened = null;
                    DisplayEvent de = null;

                    wpfnav.OnDisplay += (o, e) => { Opened = DateTime.Now; de = e; };

                    WindowTest.RunOnUIThread(
                   () =>
                   {
                       wpfnav.NavigateAsync(vm).ContinueWith
                      (
                          t =>
                          {
                              nav = DateTime.Now;
                              mre.Set();
                          });
                   });

                    mre.WaitOne();
                    mre = new ManualResetEvent(false);
                    Thread.Sleep(3000);

                    de.Should().NotBeNull();
                    de.DisplayedViewModel.Should().Be(vm);
                    Opened.HasValue.Should().BeTrue();
                    Opened.Value.Subtract(nav.Value).Should().BeGreaterThan(TimeSpan.FromSeconds(1.9)).
                        And.BeLessOrEqualTo(TimeSpan.FromSeconds(2.2));

                    WindowTest.RunOnUIThread(
                 () =>
                 {
                     wpfnav.NavigateAsync(vm).ContinueWith
                    (
                        t =>
                        {

                            mre.Set();
                        });
                 });

                    mre.WaitOne();
                    Thread.Sleep(500);
                });
        }
    }
}
