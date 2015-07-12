//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Windows.Controls;

//using FluentAssertions;
//using Xunit;

//using MVVM.CEFGlue.ViewModel;

//namespace MVVM.CEFGlue.Test
//{
//    public class Test_DoubleNavigation_Animation
//    {
//        private NavigationBuilder _INavigationBuilder;

//        public Test_DoubleNavigation_Animation()
//        {
//            _INavigationBuilder = new NavigationBuilder();
//        }

//        private WindowTest BuildWindow(Func<Tuple<WebControl, WebControl>> iWebControlFac)
//        {
//            return new WindowTest(
//                (w) =>
//                {
//                    StackPanel stackPanel = new StackPanel();
//                    w.Content = stackPanel;
//                    var iWebControl = iWebControlFac();
//                    w.RegisterName(iWebControl.Item1.Name, iWebControl);
//                    stackPanel.Children.Add(iWebControl.Item1);

//                    w.RegisterName(iWebControl.Item2.Name, iWebControl);
//                    stackPanel.Children.Add(iWebControl.Item2);
//                }
//                );
//        }


//        internal void TestNavigation(Action<WPFDoubleBrowserNavigator, WindowTest> Test)
//        {
//            AssemblyHelper.SetEntryAssembly();
//            WebControl wc1 = null;
//            WebControl wc2 = null;
//            Func<Tuple<WebControl, WebControl>> Build = () =>
//                {
//                    wc1 = new WebControl() { Name = "WebControl1" };
//                    wc2 = new WebControl() { Name = "WebControl2" };
//                    return new Tuple<WebControl, WebControl>(wc1, wc2);
//                };

//            using (var wcontext = BuildWindow(Build))
//            {
//                WPFDoubleBrowserNavigator res = null;
//                wcontext.RunOnUIThread(() => res = new WPFDoubleBrowserNavigator(wc1, wc2, _INavigationBuilder));
//                    Test(res, wcontext);
//                    WebCore.QueueWork(() => res.Dispose());
//            }
//        }

//        public class VM : ViewModelBase, INavigable
//        {
//            public INavigationSolver Navigation { get; set; }
  
//        }

//        [Fact]

//        public void Test_WPFBrowserNavigator_Simple()
//        {
//            var vm = new VM();
//            TestNavigation((wpfnav, WindowTest)
//                =>
//                {
//                    wpfnav.Should().NotBeNull();
//                    _INavigationBuilder.Register<VM>("Navigation data\\index.html");
//                    wpfnav.UseINavigable = true;

//                    var mre = new ManualResetEvent(false);
//                    DateTime? nav = null;
//                    DateTime? Opened = null;
//                    DisplayEvent de = null;

//                    wpfnav.OnDisplay += (o, e) => { Opened = DateTime.Now; de = e; };

//                    WindowTest.RunOnUIThread(
//                   () =>
//                   {
//                       wpfnav.NavigateAsync(vm).ContinueWith
//                      (
//                          t =>
//                          {
//                              vm.Navigation.Should().Be(wpfnav);
//                              nav = DateTime.Now;
//                              mre.Set();
//                          });
//                   });

//                    mre.WaitOne();
//                    mre = new ManualResetEvent(false);
//                    Thread.Sleep(4500);

//                    de.Should().NotBeNull();
//                    de.DisplayedViewModel.Should().Be(vm);
//                    Opened.HasValue.Should().BeTrue();
//                    Opened.Value.Subtract(nav.Value).Should().BeGreaterThan(TimeSpan.FromSeconds(2)).
//                        And.BeLessOrEqualTo(TimeSpan.FromSeconds(3));

//                    WindowTest.RunOnUIThread(
//                 () =>
//                 {
//                     wpfnav.NavigateAsync(vm).ContinueWith
//                    (
//                        t =>
//                        {
                           
//                            mre.Set();
//                        });
//                 });

//                    mre.WaitOne();
//                    Thread.Sleep(500); 
//                    vm.Navigation.Should().Be(wpfnav);
//                });


//        }

//    }
//}
