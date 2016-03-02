using System;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Input;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Xunit;
using FluentAssertions;
using NSubstitute;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core;
using MVVM.ViewModel.Infra;
using MVVM.ViewModel;
using HTML_WPF.Component;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Navigation;
using IntegratedTest.WPF.Infra;

namespace Integrated.WPF
{
    public abstract class Test_DoubleNavigation
    {
        protected abstract WindowTestEnvironment GetEnvironment();

        private WindowTest BuildWindow(Func<HTMLWindow> iWebControlFac, WindowTestEnvironment environment, 
                                        bool iManageLifeCycle)
        {
            environment.Register();

            return new WindowTest(
                (w) =>
                {
                    StackPanel stackPanel = new StackPanel();
                    w.Content = stackPanel;
                    var iWebControl = iWebControlFac();
                    if (iManageLifeCycle)
                        w.Closed += (o, e) => { iWebControl.Dispose(); };
                    stackPanel.Children.Add(iWebControl);
                });
        }

        internal void TestNavigation(Action<INavigationBuilder, HTMLWindow, WindowTest> Test, bool iDebug = false, bool iManageLifeCycle = true)
        {
            AssemblyHelper.SetEntryAssembly();
            HTMLWindow wc1 = null;
            Func<HTMLWindow> iWebControlFac = () =>
            {
                wc1 = new HTMLWindow();
                wc1.IsDebug = iDebug;
                return wc1;
            };

            using (var wcontext = BuildWindow(iWebControlFac, GetEnvironment(), iManageLifeCycle))
            {
                Test(wc1.NavigationBuilder, wc1, wcontext);
            }
        }

        private void SetUpRoute(INavigationBuilder builder)
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

            public ICommand GoTo1 { get; private set; }
            public ICommand Change { get; private set; }
        }

        private class AA1 : A
        {
            public AA1()
            {
                Exception = new Exception();
                Change = new RelayCommand(() => { throw Exception; });
                GoTo1 = new RelayCommand(() => Navigation.NavigateAsync(new A2()));
            }

            public Exception Exception { get; private set; }

            public ICommand GoTo1 { get; private set; }
            public ICommand Change { get; private set; }
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
        public void Test_WPFBrowserNavigator_Simple()
        {
            bool fl = false;
            EventHandler ea = null;

            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
                {
                    ea = (o, e) => { fl = true; wpfnav.OnFirstLoad -= ea; };
                    wpfnav.OnFirstLoad += ea;
                    wpfnav.Should().NotBeNull();
                    SetUpRoute(wpfbuild);
                    wpfnav.UseINavigable = true;
                    var a = new A1();
                    var mre = new ManualResetEvent(false);
                    WindowTest.RunOnUIThread(() => wpfnav.IsHTMLLoaded.Should().BeFalse());

                    WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a).ContinueWith(t => mre.Set());
               });

                    mre.WaitOne();

                    fl.Should().BeTrue();

                    WindowTest.RunOnUIThread(() =>
                    {
                        wpfnav.IsHTMLLoaded.Should().BeTrue();
                        a.Navigation.Should().NotBeNull();
                    });
                    mre.WaitOne();
                });
        }

        [Fact]
        public void Test_HTMLWindowRecovery_Capacity()
        {
            Test_HTMLWindowRecovery_Capacity_Base(null);
        }

        [Fact]
        public void Test_HTMLWindowRecovery_Capacity_Watcher()
        {
            IWebSessionWatcher watch = Substitute.For<IWebSessionWatcher>();
            Test_HTMLWindowRecovery_Capacity_Base(watch);
            //watch.Received().LogCritical(Arg.Any<string>());
        }

        [Fact]
        public void Test_HTMLWindow_Path()
        {
            bool fl = false;
            EventHandler ea = null;
            var a = new A1();
            string pn = Path.Combine(Path.GetTempPath(), "MVMMAWe");
            try 
            { 
                Directory.Delete(pn);
            }
            catch
            {
            }
            

            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.SessionPath.Should().BeNull();
                wpfnav.SessionPath = pn;

                ea = (o, e) => { fl = true; wpfnav.OnFirstLoad -= ea; };
                wpfnav.OnFirstLoad += ea;
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild);
                wpfnav.UseINavigable = true;

                var mre = new ManualResetEvent(false);
                WindowTest.RunOnUIThread(() => wpfnav.IsHTMLLoaded.Should().BeFalse());

                WindowTest.RunOnUIThread(
           () =>
           {
               wpfnav.NavigateAsync(a).ContinueWith(t => mre.Set());
           });

                mre.WaitOne();

                fl.Should().BeTrue();
            });

            Directory.Exists(pn).Should().BeTrue();
        }

        [Fact]
        public void Test_HTMLWindow_Event()
        {
            bool fl = false;
            EventHandler ea = null;
            var a = new A1();
            string pn = Path.Combine(Path.GetTempPath(), "MVMMAWe");
            bool fslr = false;
            NavigationEvent nea = null;

            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.OnFirstLoad += (o, e) => { fslr = true; };
                wpfnav.OnNavigate += (o, e) => { nea = e; };

                ea = (o, e) => { fl = true; wpfnav.OnFirstLoad -= ea; };
                wpfnav.OnFirstLoad += ea;
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild);
                wpfnav.UseINavigable = true;

                var mre = new ManualResetEvent(false);
                WindowTest.RunOnUIThread(() => wpfnav.IsHTMLLoaded.Should().BeFalse());

                WindowTest.RunOnUIThread(
           () =>
           {
               wpfnav.NavigateAsync(a).ContinueWith(t => mre.Set());
           });

                mre.WaitOne();

                fl.Should().BeTrue();
            });

            fslr.Should().BeTrue();
            nea.Should().NotBeNull();
            nea.OldViewModel.Should().BeNull();
            nea.NewViewModel.Should().Be(a);
        }

        private void Test_HTMLWindowRecovery_Capacity_Base(IWebSessionWatcher iWatcher)
        {
            bool fl = false;
            EventHandler ea = null;
            var a = new A1();

            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.WebSessionWatcher.Should().NotBeNull();
                if (iWatcher != null)
                    wpfnav.WebSessionWatcher = iWatcher;
                ea = (o, e) => { fl = true; wpfnav.OnFirstLoad -= ea; };
                wpfnav.OnFirstLoad += ea;
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild);
                wpfnav.UseINavigable = true;

                var mre = new ManualResetEvent(false);
                WindowTest.RunOnUIThread(() => wpfnav.IsHTMLLoaded.Should().BeFalse());

                WindowTest.RunOnUIThread(
           () =>
           {
               wpfnav.NavigateAsync(a).ContinueWith(t => mre.Set());
           });

                mre.WaitOne();

                fl.Should().BeTrue();

                WindowTest.RunOnUIThread(() =>
                {
                    wpfnav.IsHTMLLoaded.Should().BeTrue();
                    //a1.Navigation.Should().Be(wpfnav);
                    a.Navigation.Should().NotBeNull();
                });


                mre.WaitOne();
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
        public void Test_HTMLWindowRecovery_UnderClosure_Capacity_Base()
        {
            //bool fl = false;
            //EventHandler ea = null;
            var a = new A1();
            IWebSessionWatcher watch = Substitute.For<IWebSessionWatcher>();

            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.WebSessionWatcher.Should().NotBeNull();
                wpfnav.WebSessionWatcher = watch;
                //ea = (o, e) => { fl = true; wpfnav.OnFirstLoad -= ea; };
                //wpfnav.OnFirstLoad += ea;
                //wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild);
                wpfnav.UseINavigable = true;

                var mre = new ManualResetEvent(false);
                WindowTest.RunOnUIThread(() => wpfnav.IsHTMLLoaded.Should().BeFalse());

                WindowTest.RunOnUIThread(
           () =>
           {
               wpfnav.NavigateAsync(a).ContinueWith(t => mre.Set());
           });

                mre.WaitOne();

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

        private void Test_HTMLWindow_WebCoreShutDown_Base(IWebSessionWatcher iWatcher)
        {
            var a = new A1();

            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.WebSessionWatcher.Should().NotBeNull();
                if (iWatcher != null)
                    wpfnav.WebSessionWatcher = iWatcher;
                SetUpRoute(wpfbuild);
                wpfnav.UseINavigable = true;

                var mre = new ManualResetEvent(false);
                WindowTest.RunOnUIThread(() => wpfnav.IsHTMLLoaded.Should().BeFalse());

                WindowTest.RunOnUIThread(
           () =>
           {
               wpfnav.NavigateAsync(a).ContinueWith(t => mre.Set());
           });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    wpfnav.IsHTMLLoaded.Should().BeTrue();
                    a.Navigation.Should().NotBeNull();
                });

                //WebCore.Shutdown();

                Thread.Sleep(1500);
            });
        }

        [Fact]
        public void Test_HTMLWindow_WebCoreShutDown()
        {
            Test_HTMLWindow_WebCoreShutDown_Base(null);
        }

        [Fact]
        public void Test_HTMLWindow_WebCoreShutDown_Watcher()
        {
            IWebSessionWatcher watch = Substitute.For<IWebSessionWatcher>();
            Test_HTMLWindow_WebCoreShutDown_Base(watch);
            //watch.Received().LogCritical("Critical: WebCore ShuttingDown!!");
            //watch.Received().OnSessionError(null, Arg.Any<Action>());
        }


        private Exception Test_HTMLWindow_WebCoreShutDown_Base_Exception(IWebSessionWatcher iWatcher)
        {
            var a = new AA1();
            Exception res = null;

            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.WebSessionWatcher.Should().NotBeNull();
                if (iWatcher != null)
                    wpfnav.WebSessionWatcher = iWatcher;
                SetUpRoute(wpfbuild);
                wpfnav.UseINavigable = true;
                IHTMLBinding bind = null;

                var mre = new ManualResetEvent(false);
                WindowTest.RunOnUIThread(() => wpfnav.IsHTMLLoaded.Should().BeFalse());

                WindowTest.RunOnUIThread(
           () =>
           {
               wpfnav.NavigateAsync(a).ContinueWith(t => { var tt = t as Task<IHTMLBinding>; bind = tt.Result; mre.Set(); });
           });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    wpfnav.IsHTMLLoaded.Should().BeTrue();
                    a.Navigation.Should().NotBeNull();

                    var js = bind.JSRootObject;

                    var command = js.Invoke("Change", bind.Context);
                    command.Invoke("Execute", bind.Context);

                    //JSObject mycommand = (JSObject)js.Invoke("Change");
                    //mycommand.Invoke("Execute");
                });

                Thread.Sleep(1500);

                res = a.Exception;
            });

            return res;
        }

        //[Fact]
        //public void Test_HTMLWindow_WebCoreShutDown_Watcher_Exception()
        //{
        //    WPFWindowTestWrapper.ShouldReceivedError = true;
        //    IWebSessionWatcher watch = Substitute.For<IWebSessionWatcher>();
        //    var exp = Test_HTMLWindow_WebCoreShutDown_Base_Exception(watch);
        //    //watch.Received().LogCritical("Critical: WebCore ShuttingDown!!");
        //    //watch.Received().OnSessionError(exp, Arg.Any<Action>());
        //}

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Simple()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild);
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var a2 = new A2();
                var mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a1).ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    //a1.Navigation.Should().Be(wpfnav);
                    a1.Navigation.Should().NotBeNull();
                });

                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));

                mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
              () =>
              {
                  wpfnav.NavigateAsync(a2).ContinueWith(t => mre.Set());
              });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    //a1.Navigation.Should().Be(wpfnav);
                    a2.Navigation.Should().NotBeNull();
                    a1.Navigation.Should().BeNull();
                });

                WindowTest.RunOnUIThread(() =>
                     wpfnav.Source.Should().EndWith(@"javascript\navigation_2.html"));

            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Round_Trip()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild);
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var a2 = new A2();
                var mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a1).ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    //a1.Navigation.Should().Be(wpfnav);
                    a1.Navigation.Should().NotBeNull();
                });

                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));

                mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
                () =>
                {
                    wpfnav.NavigateAsync(a2).ContinueWith(t => mre.Set());
                });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    //a1.Navigation.Should().Be(wpfnav);
                    a2.Navigation.Should().NotBeNull();
                    a1.Navigation.Should().BeNull();
                });

                WindowTest.RunOnUIThread(() =>
                     wpfnav.Source.Should().EndWith(@"javascript\navigation_2.html"));

                mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a1).ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    //a1.Navigation.Should().Be(wpfnav);
                    a1.Navigation.Should().NotBeNull();
                    a2.Navigation.Should().BeNull();
                });

                Thread.Sleep(1000);


                WindowTest.RunOnUIThread(() =>
                     wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));

            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_3_screens()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild);
                wpfbuild.Register<A1>("javascript\\navigation_3.html", "NewPath");
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var a2 = new A2();
                var mre = new ManualResetEvent(false);


                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a1).ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    //a1.Navigation.Should().Be(wpfnav);
                    a1.Navigation.Should().NotBeNull();
                });

                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));

                mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a2).ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    a2.Navigation.Should().NotBeNull();
                    a1.Navigation.Should().BeNull();
                });

                WindowTest.RunOnUIThread(() =>
                     wpfnav.Source.Should().EndWith(@"javascript\navigation_2.html"));


                WindowTest.RunOnUIThread(
            () =>
            {
                wpfnav.NavigateAsync(a1, "NewPath").ContinueWith (
               t => {
                   a1.Navigation.Should().NotBeNull();
                   a2.Navigation.Should().BeNull();
                   mre.Set();
               });
            });
                mre.WaitOne();

                Thread.Sleep(2000);


                WindowTest.RunOnUIThread(() =>
                     wpfnav.Source.Should().EndWith(@"javascript\navigation_3.html"));

            });
        }


        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Simple_2()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild);
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a1).ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    a1.Navigation.Should().NotBeNull();
                });

                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));


                WindowTest.RunOnUIThread(
                () =>
                {
                    a1.GoTo1.Execute(null);
                });

                Thread.Sleep(1000);

                WindowTest.RunOnUIThread(() =>
                     wpfnav.Source.Should().EndWith(@"javascript\navigation_2.html"));

            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navigation_ToSame()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild);
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
                () =>
                {
                    wpfnav.NavigateAsync(a1).ContinueWith(t => mre.Set());
                });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                 {
                     a1.Navigation.Should().NotBeNull();
                 });


                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));


                WindowTest.RunOnUIThread(
                () =>
                {
                    a1.Change.Execute(null);
                });

                Thread.Sleep(200);

                WindowTest.RunOnUIThread(() =>
                     wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));

            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navigation_ToNull()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                SetUpRoute(wpfbuild);
                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a1).ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    a1.Navigation.Should().NotBeNull();
                });

                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));


                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(null).ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    a1.Navigation.Should().NotBeNull();
                });

                Thread.Sleep(200);

                WindowTest.RunOnUIThread(() =>
                     wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));

            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Resolve_OnBaseType()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>("javascript\\navigation_1.html");
                wpfbuild.Register<A1>("javascript\\navigation_2.html", "Special");

                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var mre = new ManualResetEvent(false);


                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a1).ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    a1.Navigation.Should().NotBeNull();
                });

                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));

            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Resolve_OnName_alernativesignature()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.RegisterAbsolute<A2>(string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), "javascript\\navigation_1.html"), "Special1");
                wpfbuild.Register<A2>(new Uri(string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), "javascript\\navigation_2.html")), "Special2");

                wpfnav.UseINavigable = true;
                var a1 = new A2();
                var mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
                () =>
                {
                    wpfnav.NavigateAsync(a1, "Special1").ContinueWith(t => mre.Set());
                });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    a1.Navigation.Should().NotBeNull();
                });

                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));

                mre = new ManualResetEvent(false);
                WindowTest.RunOnUIThread(
                () =>
                {
                    wpfnav.NavigateAsync(a1, "Special2").ContinueWith(t => mre.Set());
                });

                mre.WaitOne();


                WindowTest.RunOnUIThread(() =>
                {
                    a1.Navigation.Should().NotBeNull();
                });

                Thread.Sleep(1000);

                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_2.html"));

            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Resolve_OnName()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A2>("javascript\\navigation_1.html", "Special1");
                wpfbuild.Register<A2>("javascript\\navigation_2.html", "Special2");

                wpfnav.UseINavigable = true;
                wpfnav.UseINavigable = true;
                var a1 = new A2();
                var mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a1, "Special1").ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    a1.Navigation.Should().NotBeNull();
                });

                Thread.Sleep(1000);

                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html"));

                mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
             () =>
             {
                 wpfnav.NavigateAsync(a1, "Special2").ContinueWith(t => mre.Set());
             });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    a1.Navigation.Should().NotBeNull();
                });

                Thread.Sleep(1000);

                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_2.html"));

            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Resolve_NotFound()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();

                wpfnav.UseINavigable = true;
                var a1 = new A2();

                WindowTest.RunOnUIThread(
               () =>
               {
                   Action wf = () => wpfnav.NavigateAsync(a1).Wait();
                   wf.ShouldThrow<MVVMCEFGlueException>();
               });
            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Resolve_OnBaseType_2()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>("javascript\\navigation_1.html");
                wpfbuild.Register<A1>("javascript\\navigation_2.html", "Special");

                wpfnav.UseINavigable = true;
                var a1 = new A2();
                var mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a1).ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    a1.Navigation.Should().NotBeNull();
                    wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html");
                });
            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Debug_One()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                WindowTest.RunOnUIThread(() =>
                {
                    wpfnav.IsDebug.Should().BeTrue();
                    Action safe = () =>
                    wpfnav.ShowDebugWindow();
                    safe.ShouldNotThrow<Exception>();
                });

                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>("javascript\\navigation_1.html");
                wpfbuild.Register<A1>("javascript\\navigation_2.html", "Special");

                wpfnav.UseINavigable = true;
                wpfnav.UseINavigable.Should().BeTrue();

                wpfnav.OnNavigate += wpfnav_OnNavigate;
                wpfnav.OnNavigate -= wpfnav_OnNavigate;

                var a1 = new A2();
                var mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a1).ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    a1.Navigation.Should().NotBeNull();
                    wpfnav.Source.Should().EndWith(@"javascript\navigation_1.html");
                    wpfnav.ShowDebugWindow();
                    wpfnav.OpenDebugBrowser();
                });

                WindowTest.RunOnUIThread(() =>
                {
                    wpfnav.Focus();

                    wpfnav.RaiseEvent(
                      new System.Windows.Input.KeyEventArgs(
                        Keyboard.PrimaryDevice,
                        PresentationSource.FromVisual(wpfnav),
                        0,
                        Key.F5) { RoutedEvent = Keyboard.PreviewKeyDownEvent }
                    );
                });
            }, true);
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Debug_One_NoDebug()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                WindowTest.RunOnUIThread(() =>
                {
                    wpfnav.IsDebug.Should().BeFalse();
                    DispatcherTimer dt = new DispatcherTimer();
                    dt.Interval = TimeSpan.FromSeconds(10);
                    dt.Tick += (o, e) =>
                        {
                            dt.Stop();
                            System.Windows.Application.Current.Shutdown();
                        };
                    dt.Start();
                    wpfnav.OpenDebugBrowser();

                });
            });
        }

        void wpfnav_OnNavigate(object sender, NavigationEvent e)
        {
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Resolve_OnBaseType_UsingName()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>("javascript\\navigation_1.html");
                wpfbuild.Register<A1>("javascript\\navigation_2.html", "Special");

                wpfnav.UseINavigable = true;
                var a1 = new A1();
                var mre = new ManualResetEvent(false);

                WindowTest.RunOnUIThread(
               () =>
               {
                   wpfnav.NavigateAsync(a1, "Special").ContinueWith(t => mre.Set());
               });

                mre.WaitOne();

                WindowTest.RunOnUIThread(() =>
                {
                    a1.Navigation.Should().NotBeNull();
                });

                WindowTest.RunOnUIThread(
                () =>
                wpfnav.Source.Should().EndWith(@"javascript\navigation_2.html"));

            });
        }

        [Fact]
        public void Test_WPFBrowserNavigator_Navition_Resolve_OnBaseType_ShoulFailed()
        {
            TestNavigation((wpfbuild, wpfnav, WindowTest)
                =>
            {
                wpfnav.Should().NotBeNull();
                wpfbuild.Register<A>("javascript\\navigation_1.html");
                wpfbuild.Register<A1>("javascript\\navigation_2.html", "Special");

                wpfnav.UseINavigable = true;
                var a1 = new object();

                WindowTest.RunOnUIThread(
               () =>
               {
                   Action wf = () => wpfnav.NavigateAsync(a1).Wait();
                   wf.ShouldThrow<MVVMCEFGlueException>();
               });

            });
        }
    }
}
