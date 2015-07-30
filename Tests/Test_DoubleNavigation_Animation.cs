using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;

using FluentAssertions;
using Xunit;

using MVVM.CEFGlue.ViewModel;

namespace MVVM.CEFGlue.Test
{
    public class Test_DoubleNavigation_Animation
    {
        private NavigationBuilder _INavigationBuilder;

        public Test_DoubleNavigation_Animation()
        {
            _INavigationBuilder = new NavigationBuilder();
        }

        private WindowTest BuildWindow(Func<HTMLWindow> iWebControlFac)
        {
            return new WindowTest(
                (w) =>
                {
                    StackPanel stackPanel = new StackPanel();
                    w.Content = stackPanel;
                    var iWebControl = iWebControlFac();
                    w.RegisterName(iWebControl.Name, iWebControl);
                    stackPanel.Children.Add(iWebControl);
                }
                );
        }


        internal void TestNavigation(Action<HTMLWindow, WindowTest> Test)
        {
            AssemblyHelper.SetEntryAssembly();

            HTMLWindow wc1 = null;
            Func<HTMLWindow> Build = () =>
            {
                wc1 = new HTMLWindow(_INavigationBuilder);
                return wc1;
            };

            using (var wcontext = BuildWindow(Build))
            {
                Test(wc1, wcontext);
            }
        }

        public class VM : ViewModelBase, INavigable
        {
            public INavigationSolver Navigation { get; set; }

        }

        [Fact]

        public void Test_WPFBrowserNavigator_Simple()
        {
            var vm = new VM();
            TestNavigation((wpfnav, WindowTest)
                =>
                {
                    wpfnav.Should().NotBeNull();
                    _INavigationBuilder.Register<VM>("Navigation data\\index.html");
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
