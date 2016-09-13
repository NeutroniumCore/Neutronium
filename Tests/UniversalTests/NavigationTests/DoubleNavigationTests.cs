using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FluentAssertions;
using Neutronium.Core;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Navigation;
using Neutronium.Example.ViewModel.Infra;
using Neutronium.WPF;
using NSubstitute;
using Tests.Infra.IntegratedContextTesterHelper.Window;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Xunit;

namespace Tests.Universal.NavigationTests
{
    public abstract class DoubleNavigationTests : WpfComponentTest_Base<HTMLWindow>
    {
        protected DoubleNavigationTests(IWindowContextProvider windowTestEnvironment) :
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
            builder.Register<A1>(GetRelativePath(TestContext.Navigation1));
            builder.Register<AA1>(GetRelativePath(TestContext.Navigation1));
            builder.Register<A2>(GetRelativePath(TestContext.Navigation2));
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
        public async Task SetNavigation_WhenNavigationIsTrigerred_ShouldBeSet()
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

        [Fact(Skip = "should be reimplemented")]
        public async Task Test_HTMLWindowRecovery_Capacity()
        {
            await Test_HTMLWindowRecovery_Capacity_Base(null);
        }

        [Fact(Skip = "should be reimplemented")]
        public async Task Test_HTMLWindowRecovery_Capacity_Watcher()
        {
            var watch = Substitute.For<IWebSessionLogger>();
            await Test_HTMLWindowRecovery_Capacity_Base(watch);
            //watch.Received().Error(Arg.Any<string>());
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
        public async Task OnNavigate_shouldFireEvents()
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

        private async Task Test_HTMLWindowRecovery_Capacity_Base(IWebSessionLogger iLogger)
        {
            bool fl = false;
            EventHandler ea = null;
            var a = new A1();

            await TestNavigation(async (wpfbuild, wpfnav)  =>
            {
                wpfnav.WebSessionLogger.Should().NotBeNull();
                if (iLogger != null)
                    wpfnav.WebSessionLogger = iLogger;
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
                var watch = Substitute.For<IWebSessionLogger>();

                wpfnav.WebSessionLogger.Should().NotBeNull();
                wpfnav.WebSessionLogger = watch;
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

                //watch.DidNotReceive().Error("WebView crashed trying recover");
            }, false, false);
        }

        [Fact(Skip ="should be reimplemented")]
        public async Task Test_HTMLWindow_WebCoreShutDown()
        {
            await Test_HTMLWindow_WebCoreShutDown_Base(null);
        }

        [Fact(Skip = "should be reimplemented")]
        public async Task Test_HTMLWindow_WebCoreShutDown_Watcher()
        {
            var watch = Substitute.For<IWebSessionLogger>();
            await Test_HTMLWindow_WebCoreShutDown_Base(watch);
            //watch.Received().Error("Critical: WebCore ShuttingDown!!");
            //watch.Received().WebBrowserError(null, Arg.Any<Action>());
        }

        private async Task Test_HTMLWindow_WebCoreShutDown_Base(IWebSessionLogger iLogger)
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                var a = new A1();

                wpfnav.WebSessionLogger.Should().NotBeNull();
                if (iLogger != null)
                    wpfnav.WebSessionLogger = iLogger;
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

        private async Task<Exception> GetExceptionFromHTMLWindow_WebCoreShutDown(IWebSessionLogger iLogger)
        {
            var a = new AA1();
            Exception res = null;

            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.WebSessionLogger.Should().NotBeNull();
                if (iLogger != null)
                    wpfnav.WebSessionLogger = iLogger;
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
        //    IWebSessionLogger watch = Substitute.For<IWebSessionLogger>();
        //    var exp = Test_HTMLWindow_WebCoreShutDown_Base_Exception(watch);
        //    //watch.Received().Error("Critical: WebCore ShuttingDown!!");
        //    //watch.Received().WebBrowserError(exp, Arg.Any<Action>());
        //}

        [Fact]
        public async Task NavigateAsync_UseTypeInformationFromRegister()
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
                CheckPath(wpfnav.Source, TestContext.Navigation1);

                await wpfnav.NavigateAsync(a2);

                a2.Navigation.Should().NotBeNull();
                a1.Navigation.Should().BeNull();
                CheckPath(wpfnav.Source, TestContext.Navigation2);
            });
        }

        [Fact]
        public async Task NavigateAsync_UseTypeInformationFromRegister_RoundTrip()
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
                CheckPath(wpfnav.Source, TestContext.Navigation1);

                await wpfnav.NavigateAsync(a2);

                a2.Navigation.Should().NotBeNull();
                a1.Navigation.Should().BeNull();
                CheckPath(wpfnav.Source, TestContext.Navigation2);

                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                a2.Navigation.Should().BeNull();
               
                await Task.Delay(1000);
                CheckPath(wpfnav.Source, TestContext.Navigation1);
            });
        }

        [Fact]
        public async Task NavigateAsync_UseTypeInformationFromRegister_3Screens()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfbuild.Register<A1>(GetRelativePath(TestContext.Navigation3), "NewPath");
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var a2 = new A2();

                await wpfnav.NavigateAsync(a1);
                a1.Navigation.Should().NotBeNull();
                CheckPath(wpfnav.Source, TestContext.Navigation1);

                await wpfnav.NavigateAsync(a2);

                a2.Navigation.Should().NotBeNull();
                a1.Navigation.Should().BeNull();
                CheckPath(wpfnav.Source, TestContext.Navigation2);

                await wpfnav.NavigateAsync(a1, "NewPath");

                a1.Navigation.Should().NotBeNull();
                a2.Navigation.Should().BeNull();

                await Task.Delay(2000);
                CheckPath(wpfnav.Source, TestContext.Navigation3);
            });
        }

        [Fact]
        public async Task NavigateAsync_UseTypeInformationFromRegister_2Screens()            
        {
            await TestNavigation(async (wpfbuild, wpfnav)  =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;
                var a1 = new A1();

                await wpfnav.NavigateAsync(a1);
                a1.Navigation.Should();
                CheckPath(wpfnav.Source, TestContext.Navigation1);

                a1.GoTo1.Execute(null);

                await Task.Delay(1000);
                CheckPath(wpfnav.Source, TestContext.Navigation2);
            });
        }

        [Fact]
        public async Task NavigateAsync_ReloadPageIfNeeded()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;
                var a1 = new A1();

                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                CheckPath(wpfnav.Source, TestContext.Navigation1);

                a1.Change.Execute(null);

                await Task.Delay(200);

                CheckPath(wpfnav.Source, TestContext.Navigation1);
            });
        }

        [Fact]
        public async Task NavigateAsync_ToNull_DoesNotThrow()
        {
            await TestNavigation(async (wpfbuild, wpfnav)  =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild, wpfnav);
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                CheckPath(wpfnav.Source, TestContext.Navigation1);

                await wpfnav.NavigateAsync(null);

                await Task.Delay(200);

                CheckPath(wpfnav.Source, TestContext.Navigation1);
            });
        }

        [Fact]
        public async Task NavigateAsync_ResolveOnBaseType()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>(GetRelativePath(TestContext.Navigation1));
                wpfbuild.Register<A1>(GetRelativePath(TestContext.Navigation2), "Special");

                wpfnav.UseINavigable = true;
                var a1 = new A1();
                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                CheckPath(wpfnav.Source, TestContext.Navigation1);
            });
        }

        [Fact]
        public async Task NavigateAsync_WhenRegisterAbsoluteOrRegisterAbsoluteAreUsed_UsesIdINformation()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.RegisterAbsolute<A2>(GetPath(TestContext.Navigation1), "Special1");
                wpfbuild.Register<A2>(new Uri(GetPath(TestContext.Navigation2)), "Special2");

                wpfnav.UseINavigable = true;
                var a1 = new A2();

                await wpfnav.NavigateAsync(a1, "Special1");

                a1.Navigation.Should().NotBeNull();
                CheckPath(wpfnav.Source, TestContext.Navigation1);

                await wpfnav.NavigateAsync(a1, "Special2");

                a1.Navigation.Should().NotBeNull();

                await Task.Delay(1000);
                CheckPath(wpfnav.Source, TestContext.Navigation2);
            });
        }

        [Fact]
        public async Task NavigateAsync_UsesIdINformation()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();

                wpfbuild.Register<A2>(GetRelativePath(TestContext.Navigation1), "Special1");
                wpfbuild.Register<A2>(GetRelativePath(TestContext.Navigation2), "Special2");
               
                wpfnav.UseINavigable = true;
                wpfnav.UseINavigable = true;
                
                var a2 = new A2();
                await wpfnav.NavigateAsync(a2, "Special1");
                a2.Navigation.Should().NotBeNull();

                await Task.Delay(1000);
                CheckPath(wpfnav.Source, TestContext.Navigation1);

                await wpfnav.NavigateAsync(a2, "Special2");
                a2.Navigation.Should().NotBeNull();

                await Task.Delay(1000);
                CheckPath(wpfnav.Source, TestContext.Navigation2);
            });
        }

        [Fact]
        public void NavigateAsync_WhenNoMatchIsFound_ThrowException()
        {
            TestNavigation((wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();

                wpfnav.UseINavigable = true;
                var a1 = new A2();

                Func<Task> wf = () => wpfnav.NavigateAsync(a1);
                wf.ShouldThrow<NeutroniumException>();
            });
        }

        [Fact]
        public async Task OpenDebugBrowser_ShouldWork()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.IsDebug.Should().BeTrue();
                Action safe = wpfnav.ShowDebugWindow;
                safe.ShouldNotThrow<Exception>();

                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>(GetRelativePath(TestContext.Navigation1));
                wpfbuild.Register<A1>(GetRelativePath(TestContext.Navigation2), "Special");

                wpfnav.UseINavigable = true;
                wpfnav.UseINavigable.Should().BeTrue();

                wpfnav.OnNavigate += wpfnav_OnNavigate;
                wpfnav.OnNavigate -= wpfnav_OnNavigate;

                var a1 = new A2();

                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                CheckPath(wpfnav.Source, TestContext.Navigation1);
                wpfnav.ShowDebugWindow();
                wpfnav.OpenDebugBrowser();

                wpfnav.Focus();

                wpfnav.RaiseEvent( new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice,
                         PresentationSource.FromVisual(wpfnav), 0,Key.F5) { RoutedEvent = Keyboard.PreviewKeyDownEvent }   );

                wpfnav.CloseDebugBrowser();
            }, true);
        }

        [Fact]
        public void OpenDebugBrowser_ShouldNotCrash_WhenDebugIsFalse()
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
        public async Task NavigateAsync_WhenidIsRegistered_UsesIdInformationBeforeBaseType()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>(GetRelativePath(TestContext.Navigation1));
                wpfbuild.Register<A1>(GetRelativePath(TestContext.Navigation2), "Special");

                wpfnav.UseINavigable = true;
                var a1 = new A1();

                await wpfnav.NavigateAsync(a1, "Special");

                a1.Navigation.Should().NotBeNull();
                CheckPath(wpfnav.Source, TestContext.Navigation2);
            });
        }

        [Fact]
        public async Task NavigateAsync_WhenidIsRegistered_UsesBaseTypeIfIdInformationDoesNotMatch()
        {
            await TestNavigation(async (wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>(GetRelativePath(TestContext.Navigation1));
                wpfbuild.Register<A1>(GetRelativePath(TestContext.Navigation2), "Special2");

                wpfnav.UseINavigable = true;
                var a1 = new A2();
                await wpfnav.NavigateAsync(a1);

                a1.Navigation.Should().NotBeNull();
                CheckPath(wpfnav.Source, TestContext.Navigation1);
            });
        }

        [Fact]
        public void NavigateAsync_WhenTypeIsNotRegistered_ShouldThrowException()
        {
            TestNavigation((wpfbuild, wpfnav) =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A> (GetRelativePath(TestContext.Navigation1));
                wpfbuild.Register<A1>(GetRelativePath(TestContext.Navigation2), "Special");

                wpfnav.UseINavigable = true;
                var a1 = new object();

                Func<Task> wf = async () => await wpfnav.NavigateAsync(a1);
                wf.ShouldThrow<NeutroniumException>();
            });
        }

        private void CheckPath(Uri path, TestContext pathContext)
        {
            path.AbsolutePath.Should().EndWith(Path.GetFileName(GetRelativePath(pathContext)));
        }
    }
}
