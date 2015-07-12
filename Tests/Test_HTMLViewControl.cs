//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Controls;
//using System.Threading;
//using System.Reflection;

//using Xunit;
//using FluentAssertions;

//using MVVM.CEFGlue.Infra;
//using MVVM.CEFGlue.ViewModel.Example;
//using MVVM.CEFGlue.Exceptions;

//namespace MVVM.CEFGlue.Test
//{
//    public class Test_HTMLViewControl
//    {
//        private WindowTest BuildWindow(Func<HTMLViewControl> iWebControlFac)
//        {
//            return new WindowTest(
//                (w) =>
//                {
//                    StackPanel stackPanel = new StackPanel();
//                    w.Content = stackPanel;
//                    var iWebControl = iWebControlFac();
//                    w.RegisterName(iWebControl.Name, iWebControl);
//                    w.Closed += (o, e) => { iWebControl.Dispose(); };
//                    stackPanel.Children.Add(iWebControl);
//                }
//                );
//        }

//        internal void Test(Action<HTMLViewControl, WindowTest> Test, bool iDebug = false)
//        {
//            AssemblyHelper.SetEntryAssembly();
//            HTMLViewControl wc1 = null;
//            Func<HTMLViewControl> iWebControlFac = () =>
//            {
//                wc1 = new HTMLViewControl();
//                wc1.IsDebug = iDebug;
//                return wc1;
//            };

//            using (var wcontext = BuildWindow(iWebControlFac))
//            {
//                Test(wc1, wcontext);
//            }
//        }

//        [Fact]
//        public void Basic_Option()
//        {
//            Test((c, w) =>
//                {
//                    var mre = new ManualResetEvent(false);

//                    w.RunOnUIThread(() =>
//                        {
//                            c.SessionPath.Should().BeNull();
//                            c.Mode.Should().Be(JavascriptBindingMode.TwoWay);
//                            c.Uri.Should().BeNull();
//                            mre.Set();
//                        });

//                    mre.WaitOne();


//                });
//        }

//        [Fact]
//        public void Basic_Option_Find_Path()
//        {
//            Test((c, w) =>
//            {
//                var mre = new ManualResetEvent(false);

//                w.RunOnUIThread(() =>
//                {
//                    c.SessionPath.Should().BeNull();
//                    c.Mode.Should().Be(JavascriptBindingMode.TwoWay);
//                    c.Uri.Should().BeNull();

//                    string relp = "javascript\\navigation_1.html";
//                    Action act = () => c.RelativeSource = relp;
//                    act.ShouldThrow<MVVMforAwesomiumException>();
//                    //c.Uri.AbsolutePath.Replace("/", "\\").Should().Be(string.Format("{0}\\{1}", Assembly.GetAssembly(typeof(HTMLViewControl)).GetPath(), relp));
//                    mre.Set();
//                });

//                mre.WaitOne();


//            });
//        }

//        [Fact]
//        public void Basic_Option_Simple()
//        {
//            Test((c, w) =>
//            {
//                var mre = new ManualResetEvent(false);

//                DisplayEvent de = null;
//                EventHandler<DisplayEvent> ea = null;
//                ea =(o, e) => { de = e; c.OnDisplay-=ea; };
//                c.OnDisplay += ea;
//                var dc = new Person();

//                w.RunOnUIThread(() =>
//                {
//                    c.Mode = JavascriptBindingMode.OneWay;
//                    string relp = "javascript\\navigation_1.html";
//                    c.Uri = new Uri(string.Format("{0}\\{1}", Assembly.GetAssembly(typeof(Test_HTMLViewControl)).GetPath(), relp));
//                    w.Window.DataContext = dc;
//                    mre.Set();
//                });

//                mre.WaitOne();

//                Thread.Sleep(1500);
//                de.Should().NotBeNull();
//                de.DisplayedViewModel.Should().Be(dc);


//            });
//        }
//    }
//}
