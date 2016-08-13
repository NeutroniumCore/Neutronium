using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FluentAssertions;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Navigation;
using MVVM.ViewModel;
using MVVM.ViewModel.Infra;
using NSubstitute;
using Xunit;

namespace IntegratedTest.Tests.WPF
{
    public abstract class Test_DoubleNavigation : Test_WpfComponent_Base<HTMLWindow>
    {
        protected Test_DoubleNavigation(IWindowTestEnvironment windowTestEnvironment) :
            base(windowTestEnvironment)
        {
        }

        protected override HTMLWindow GetNewHTMLControlBase(bool iDebug)
        {
            return new HTMLWindow { IsDebug = iDebug };
        }

        internal void TestNavigation(Action<INavigationBuilder, HTMLWindow> test, bool iDebug = false, bool iManageLifeCycle = true)
        {
            Action<HTMLWindow> simpleTest = (windowHtml) => test(windowHtml.NavigationBuilder, windowHtml);

            base.Test(simpleTest, iDebug, iManageLifeCycle);
        }

        internal async Task TestNavigation(Func<INavigationBuilder, HTMLWindow, Task> test, bool iDebug = false, bool iManageLifeCycle = true) {
            Func<HTMLWindow, WindowTest, Task> simpleTest = (windowHtml, windowTest) => test(windowHtml.NavigationBuilder, windowHtml);

            await base.Test(simpleTest, iDebug, iManageLifeCycle);
        }

        private void SetUpRoute(INavigationBuilder builder, HTMLWindow wpfnav)
        {
            builder.Register<A1>("javascript\\navigation_1.html");
            builder.Register<AA1>("javascript\\navigation_1.html");
            builder.Register<A2>("javascript\\navigation_2.html");
        }

        #region ViewModel

        private class A : ViewModelBase, INavigable
        {
            public INavigationSolver Navigation { get; set; }
        }

        private class A1 : A
        {
            public A1()
            {
                Change = new RelayCommand(() => Navigation.NavigateAsync(new A1()));
                GoTo1 = new RelayCommand(() => Navigation.NavigateAsync(new A2()));
            }

            public ICommand GoTo1 { get; }
            public ICommand Change { get; }
        }

        private class AA1 : A
        {
            public AA1()
            {
                Exception = new Exception();
                Change = new RelayCommand(() => { throw Exception; });
                GoTo1 = new RelayCommand(() => Navigation.NavigateAsync(new A2()));
            }

            public Exception Exception { get; }
            public ICommand GoTo1 { get; }
            public ICommand Change { get; }
        }

        private class A2 : A
        {
            public A2()
            {
                GoTo1 = new RelayCommand(() => Navigation.NavigateAsync(new A1()));
            }

            public ICommand GoTo1 { get; private set; }
        }

        #endregion

        [Fact]
        public async Task Test_WPFBrowserNavigator_Simple()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>            
            {
                bool fl = false;
                EventHandler ea = null;
                ea = (o, e) => { fl = true; wpfnav.OnFirstLoad -= ea; };
                wpfnav.OnFirstLoad += ea;
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;
                var a = new A1();
                   
                wpfnav.IsHTMLLoaded.Should().BeFalse();

                await wpfnav.NavigateAsync(a);

                fl.Should().BeTrue();
                wpfnav.IsHTMLLoaded.Should().BeTrue();
                a.Navigation.Should().NotBeNull();
            });
        }

        [Fact]
        public async Task Test_HTMLWindowRecovery_Capacity()
        {
            await Test_HTMLWindowRecovery_Capacity_Base(null);
        }

        [Fact]
        public async Task Test_HTMLWindowRecovery_Capacity_Watcher()
        {
            var watch = Substitute.For<IWebSessionWatcher>();
            await Test_HTMLWindowRecovery_Capacity_Base(watch);
            //watch.Received().LogCritical(Arg.Any<string>());
        }

        [Fact]
        public async Task Test_HTMLWindow_Path()
        {   
            var a = new A1();
            var pn = Path.Combine(Path.GetTempPath(), "MVMMAWe");
            try
            {
                Directory.Delete(pn);
            }
            catch
            {
            }

            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.SessionPath.Should().BeNull();
                wpfnav.SessionPath = pn;
                EventHandler ea = null;
                bool fl = false;
                ea = (o, e) => { fl = true; wpfnav.OnFirstLoad -= ea; };
                wpfnav.OnFirstLoad += ea;
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;
                wpfnav.IsHTMLLoaded.Should().BeFalse();

                await wpfnav.NavigateAsync(a);

                fl.Should().BeTrue();

                Directory.Exists(pn).Should().BeTrue();
            });
        }

        [Fact]
        public async Task Test_HTMLWindow_Event()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {       
                var a = new A1();
                string pn = Path.Combine(Path.GetTempPath(), "MVMMAWe");

                bool fslr = false;
                wpfnav.OnFirstLoad += (o, e) => { fslr = true; };

                NavigationEvent nea = null;
                wpfnav.OnNavigate += (o, e) => { nea = e; };

                EventHandler ea = null;
                bool fl = false;
                ea = (o, e) => { fl = true; wpfnav.OnFirstLoad -= ea; };
                wpfnav.OnFirstLoad += ea;
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;

                wpfnav.IsHTMLLoaded.Should().BeFalse();
                await wpfnav.NavigateAsync(a);

                fl.Should().BeTrue();
                fslr.Should().BeTrue();
                nea.Should().NotBeNull();
                nea.OldViewModel.Should().BeNull();
                nea.NewViewModel.Should().Be(a);
            });
        }

        private async Task Test_HTMLWindowRecovery_Capacity_Base(IWebSessionWatcher iWatcher)
        {
            bool fl = false;
            EventHandler ea = null;
            var a = new A1();

            await TestNavigation(async (wpfbuild, wpfnav)  =>
            {
                wpfnav.WebSessionWatcher.Should().NotBeNull();
                if (iWatcher != null)
                    wpfnav.WebSessionWatcher = iWatcher;
                ea = (o, e) => { fl = true; wpfnav.OnFirstLoad -= ea; };
                wpfnav.OnFirstLoad += ea;
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;

                wpfnav.IsHTMLLoaded.Should().BeFalse();
                await wpfnav.NavigateAsync(a);

                fl.Should().BeTrue();

                wpfnav.IsHTMLLoaded.Should().BeTrue();
                a.Navigation.Should().NotBeNull();

                //var webv = (a.Navigation as IWebViewProvider).WebView;

                //mre = new ManualResetEvent(false);
                //Process p = null;
                //WindowTest.RunOnUIThread(() =>
                //{
                //    p = webv.RenderProcess;
                //    p.Kill();
                //    mre.Set();
                //});

                //mre.WaitOne();
                //Thread.Sleep(500);
                //p.WaitForExit();


                //Process np = null;
                //mre = new ManualResetEvent(false);
                //WindowTest.RunOnUIThread(() =>
                //{
                //    np = webv.RenderProcess;
                //    mre.Set();
                //});
                //np.Should().NotBe(p);
            });
        }

        [Fact]
        public async Task Test_HTMLWindowRecovery_UnderClosure_Capacity_Base()
        {
            //bool fl = false;
            //EventHandler ea = null;
            await TestNavigation(async (wpfbuild, wpfnav)  =>
            {
                var a = new A1();
                var watch = Substitute.For<IWebSessionWatcher>();

                wpfnav.WebSessionWatcher.Should().NotBeNull();
                wpfnav.WebSessionWatcher = watch;
                //ea = (o, e) => { fl = true; wpfnav.OnFirstLoad -= ea; };
                //wpfnav.OnFirstLoad += ea;
                //wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;

                wpfnav.IsHTMLLoaded.Should().BeFalse();

                await wpfnav.NavigateAsync(a);

                //WindowTest.RunOnUIThread(() =>
                //{
                //    wpfnav.IsHTMLLoaded.Should().BeTrue();
                //    //a1.Navigation.Should().Be(wpfnav);
                //    a.Navigation.Should().NotBeNull();
                //    System.Windows.Application.Current.Shutdown();
                //    var p = (a.Navigation as IWebViewProvider).WebView.RenderProcess;
                //    WebCore.Shutdown();
                //    //p.Kill();
                //    mre.Set();
                //});


                //mre.WaitOne();

                //Thread.Sleep(1000);

                //watch.DidNotReceive().LogCritical("WebView crashed trying recover");
            }, false, false);
        }

        private async Task Test_HTMLWindow_WebCoreShutDown_Base(IWebSessionWatcher iWatcher)
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                var a = new A1();

                wpfnav.WebSessionWatcher.Should().NotBeNull();
                if (iWatcher != null)
                    wpfnav.WebSessionWatcher = iWatcher;
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;

                wpfnav.IsHTMLLoaded.Should().BeFalse();

                await wpfnav.NavigateAsync(a);

                wpfnav.IsHTMLLoaded.Should().BeTrue();
                a.Navigation.Should().NotBeNull();
                //WebCore.Shutdown();
                //await Task.Delay(1500);
            });
        }

        [Fact]
        public async Task Test_HTMLWindow_WebCoreShutDown()
        {
            await Test_HTMLWindow_WebCoreShutDown_Base(null);
        }

        [Fact]
        public async Task Test_HTMLWindow_WebCoreShutDown_Watcher()
        {
            var watch = Substitute.For<IWebSessionWatcher>();
            await Test_HTMLWindow_WebCoreShutDown_Base(watch);
            //watch.Received().LogCritical("Critical: WebCore ShuttingDown!!");
            //watch.Received().OnSessionError(null, Arg.Any<Action>());
        }


        private async Task<Exception> Test_HTMLWindow_WebCoreShutDown_Base_Exception(IWebSessionWatcher iWatcher)
        {
            var a = new AA1();
            Exception res = null;

            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.WebSessionWatcher.Should().NotBeNull();
                if (iWatcher != null)
                    wpfnav.WebSessionWatcher = iWatcher;
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;

                wpfnav.IsHTMLLoaded.Should().BeFalse();

                var bind = await wpfnav.NavigateAsync(a);

                wpfnav.IsHTMLLoaded.Should().BeTrue();
                a.Navigation.Should().NotBeNull();

                var js = bind.JSRootObject;
                var command = js.Invoke("Change", bind.Context);
                command.Invoke("Execute", bind.Context);

                await Task.Delay(1500);
                res = a.Exception;
            });

            return res;
        }

        //[Fact]
        //public void Test_HTMLWindow_WebCoreShutDown_Watcher_Exception() {
        //    WPFWindowTestWrapper.ShouldReceivedError = true;
        //    IWebSessionWatcher watch = Substitute.For<IWebSessionWatcher>();
        //    var exp = Test_HTMLWindow_WebCoreShutDown_Base_Exception(watch);
        //    //watch.Received().LogCritical("Critical: WebCore ShuttingDown!!");
        //    //watch.Received().OnSessionError(exp, Arg.Any<Action>());
        //}

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navition_Simple()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var a2 = new A2();

                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");

                await wpfnav.NavigateAsync(a2);

                a2.Navigation.Should().NotBeNull();
                a1.Navigation.Should().BeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_2.html");
            });
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navition_Round_Trip()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var a2 = new A2();

                await wpfnav.NavigateAsync(a1);
                a1.Navigation.Should().NotBeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");

                await wpfnav.NavigateAsync(a2);

                a2.Navigation.Should().NotBeNull();
                a1.Navigation.Should().BeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_2.html");

                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                a2.Navigation.Should().BeNull();
               
                await Task.Delay(1000);
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");
            });
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navition_3_screens()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfbuild.Register<A1>("javascript\\navigation_3.html", "NewPath");
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var a2 = new A2();

                await wpfnav.NavigateAsync(a1);
                a1.Navigation.Should().NotBeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");

                await wpfnav.NavigateAsync(a2);

                a2.Navigation.Should().NotBeNull();
                a1.Navigation.Should().BeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_2.html");

                await wpfnav.NavigateAsync(a1, "NewPath");

                a1.Navigation.Should().NotBeNull();
                a2.Navigation.Should().BeNull();

                await Task.Delay(2000);
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_3.html");
            });
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navition_Simple_2()
        {
            await TestNavigation(async (wpfbuild, wpfnav)  =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;
                var a1 = new A1();

                await wpfnav.NavigateAsync(a1);
                a1.Navigation.Should();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");

                a1.GoTo1.Execute(null);

                await Task.Delay(1000);
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_2.html");
            });
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navigation_ToSame()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;
                var a1 = new A1();

                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");

                a1.Change.Execute(null);

                await Task.Delay(200);

                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");
            });
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navigation_ToNull()
        {
            await TestNavigation(async (wpfbuild, wpfnav)  =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");

                await wpfnav.NavigateAsync(null);

                await Task.Delay(200);

                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");
            });
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navition_Resolve_OnBaseType()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>("javascript\\navigation_1.html");
                wpfbuild.Register<A1>("javascript\\navigation_2.html", "Special");

                wpfnav.UseINavigable = true;
                var a1 = new A1();
                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");
            });
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navition_Resolve_OnName_alernativesignature()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.RegisterAbsolute<A2>(string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), "javascript\\navigation_1.html"), "Special1");
                wpfbuild.Register<A2>(new Uri(string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), "javascript\\navigation_2.html")), "Special2");

                wpfnav.UseINavigable = true;
                var a1 = new A2();

                await wpfnav.NavigateAsync(a1, "Special1");

                a1.Navigation.Should().NotBeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");

                await wpfnav.NavigateAsync(a1, "Special2");

                a1.Navigation.Should().NotBeNull();

                await Task.Delay(1000);
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_2.html");
            });
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navition_Resolve_OnName()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A2>("javascript\\navigation_1.html", "Special1");
                wpfbuild.Register<A2>("javascript\\navigation_2.html", "Special2");
                wpfnav.UseINavigable = true;
                wpfnav.UseINavigable = true;
                
                var a2 = new A2();
                await wpfnav.NavigateAsync(a2, "Special1");
                a2.Navigation.Should().NotBeNull();

                await Task.Delay(1000);
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");

                await wpfnav.NavigateAsync(a2, "Special2");
                a2.Navigation.Should().NotBeNull();

                await Task.Delay(1000);
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_2.html");
            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Resolve_NotFound()
        {
            TestNavigation((wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();

                wpfnav.UseINavigable = true;
                var a1 = new A2();

                Func<Task> wf = () => wpfnav.NavigateAsync(a1);
                wf.ShouldThrow<MVVMCEFGlueException>();
            });
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navition_Resolve_OnBaseType_2()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>("javascript\\navigation_1.html");
                wpfbuild.Register<A1>("javascript\\navigation_2.html", "Special");

                wpfnav.UseINavigable = true;
                var a1 = new A2();
                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");
            });
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navition_Debug_One()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.IsDebug.Should().BeTrue();
                Action safe = wpfnav.ShowDebugWindow;
                safe.ShouldNotThrow<Exception>();

                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>("javascript\\navigation_1.html");
                wpfbuild.Register<A1>("javascript\\navigation_2.html", "Special");

                wpfnav.UseINavigable = true;
                wpfnav.UseINavigable.Should().BeTrue();

                wpfnav.OnNavigate += wpfnav_OnNavigate;
                wpfnav.OnNavigate -= wpfnav_OnNavigate;

                var a1 = new A2();

                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_1.html");
                wpfnav.ShowDebugWindow();
                wpfnav.OpenDebugBrowser();

                wpfnav.Focus();

                wpfnav.RaiseEvent( new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice,
                         PresentationSource.FromVisual(wpfnav), 0,Key.F5) { RoutedEvent = Keyboard.PreviewKeyDownEvent }   );

                wpfnav.CloseDebugBrowser();
            }, true);
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Debug_One_NoDebug()
        {
            TestNavigation((wpfbuild, wpfnav) =>
            {
                wpfnav.IsDebug.Should().BeFalse();
                wpfnav.OpenDebugBrowser();
            });
        }

        void wpfnav_OnNavigate(object sender, NavigationEvent e)
        {
        }

        [Fact]
        public async Task Test_WPFBrowserNavigator_Navition_Resolve_OnBaseType_UsingName()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>("javascript\\navigation_1.html");
                wpfbuild.Register<A1>("javascript\\navigation_2.html", "Special");

                wpfnav.UseINavigable = true;
                var a1 = new A1();

                await wpfnav.NavigateAsync(a1, "Special");

                a1.Navigation.Should().NotBeNull();
                wpfnav.Source.AbsolutePath.Should().EndWith(@"javascript/navigation_2.html");
            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Resolve_OnBaseType_ShoulFailed()
        {
            TestNavigation((wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>("javascript\\navigation_1.html");
                wpfbuild.Register<A1>("javascript\\navigation_2.html", "Special");

                wpfnav.UseINavigable = true;
                var a1 = new object();

                Func<Task> wf = async () => await wpfnav.NavigateAsync(a1);
                wf.ShouldThrow<MVVMCEFGlueException>();
            });
        }
    }
}
