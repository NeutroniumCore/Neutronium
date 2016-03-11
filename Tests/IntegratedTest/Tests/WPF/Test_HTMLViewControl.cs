using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Navigation;
using MVVM.ViewModel.Example;
using Xunit;

namespace IntegratedTest.Tests.WPF
{
    public abstract class Test_HTMLViewControl : Test_WpfComponent_Base<HTMLViewControl>
    {
        protected Test_HTMLViewControl(IWindowTestEnvironment windowTestEnvironment) :
            base(windowTestEnvironment)
        {
        }

        protected override HTMLViewControl GetNewHTMLControlBase(bool iDebug)
        {
            return new HTMLViewControl { IsDebug = iDebug };
        }

        [Fact]
        public void  Basic_Option()
        {
            Test((c) =>
            {
                c.SessionPath.Should().BeNull();
                c.Mode.Should().Be(JavascriptBindingMode.TwoWay);
                c.Uri.Should().BeNull();
            });
        }

        [Fact(Skip = "Should be rethink")]
        public void Basic_Option_Find_Path()
        {
            Test((c) =>
            {
                c.SessionPath.Should().BeNull();
                c.Mode.Should().Be(JavascriptBindingMode.TwoWay);
                c.Uri.Should().BeNull();

                string relp = "javascript\\navigation_1.html";
                Action act = () => c.RelativeSource = relp;
                act.ShouldThrow<MVVMCEFGlueException>();
            });
        }

        [Fact(Skip = "Should be rethink")]
        public void Basic_RelativeSource() 
        {
            Test((c) => 
            {
                string relp = "javascript\\navigation_1.html";
                string path = string.Format("{0}\\{1}", typeof(HTMLViewControl).Assembly.GetPath(), relp);
                if (File.Exists(path))
                    File.Delete(path);

                c.SessionPath.Should().BeNull();
                c.Mode.Should().Be(JavascriptBindingMode.TwoWay);
                c.Uri.Should().BeNull();

                string nd = Path.GetDirectoryName(path);
                Directory.CreateDirectory(nd);
                File.Copy(@"javascript\navigation_1.html", path);

                c.RelativeSource = relp;
                File.Delete(path);
            });
        }

        [Fact]
        public async Task OnDisplay_ShouldBeFired_OnDataContextChanges()
        {
            await Test(async (c, w) =>
            {
                var tcs = new TaskCompletionSource<DisplayEvent>();

                EventHandler<DisplayEvent> ea = null;
                ea = (o, e) => { tcs.TrySetResult(e); c.OnDisplay -= ea; };
                c.OnDisplay += ea;
                var dc = new Person();

                c.Mode = JavascriptBindingMode.OneWay;
                string relp = "javascript\\navigation_1.html";
                c.Uri = new Uri(string.Format("{0}\\{1}", Assembly.GetAssembly(typeof(Test_HTMLViewControl)).GetPath(), relp));
                w.Window.DataContext = dc;

                var de = await tcs.Task;
                de.Should().NotBeNull();
                de.DisplayedViewModel.Should().Be(dc);
            });
        }

        [Fact(Skip = "Should be rethink")]
        public async Task Basic_Option_Simple_UsingRelativePath() 
        {
            await Test(async (c, w) =>
            {
                var tcs = new TaskCompletionSource<DisplayEvent>();
                EventHandler<DisplayEvent> ea = null;
                ea = (o, e) => { c.OnDisplay -= ea; tcs.SetResult(e); };
                c.OnDisplay += ea;
                var dc = new Person();

                string relp = "javascript\\navigation_1.html";
                string path = string.Format("{0}\\{1}", typeof(HTMLViewControl).Assembly.GetPath(), relp);
                var jvs = PrepareFiles();

                c.Mode = JavascriptBindingMode.OneWay;
                c.RelativeSource = relp;
                w.Window.DataContext = dc;

                var de = await tcs.Task;

                foreach (string jv in jvs) 
                {
                    string p = string.Format("{0}\\javascript\\src\\{1}", typeof(HTMLViewControl).Assembly.GetPath(), jv);
                    File.Delete(p);
                }
                File.Delete(path);
                de.Should().NotBeNull();
                de.DisplayedViewModel.Should().Be(dc);
            });
        }

        private string[] PrepareFiles()
        {
            string relp = "javascript\\navigation_1.html";
            string path = string.Format("{0}\\{1}", typeof(HTMLViewControl).Assembly.GetPath(), relp);
            string nd = Path.GetDirectoryName(path);
            Directory.CreateDirectory(nd);

            if (!File.Exists(path))
                File.Copy("javascript\\navigation_1.html", path);

            string[] jvs = new string[] { "Ko_register.js", "Ko_Extension.js", "knockout.js" };

            string src = string.Format("{0}\\javascript\\src", typeof(HTMLViewControl).Assembly.GetPath());
            Directory.CreateDirectory(src);

            foreach (string jv in jvs)
            {
                string p = string.Format("{0}\\javascript\\src\\{1}", typeof(HTMLViewControl).Assembly.GetPath(), jv);
                if (!File.Exists(p))
                    File.Copy(string.Format("javascript\\src\\{0}", jv), p);
            }

            return jvs;
        }

        [Fact]
        public async Task Basic_Option_Simple_UsingRelativePath_AfterDataContext()
        {
            await Test(async (c, w) =>
            {
                var tcs = new TaskCompletionSource<DisplayEvent>();
                EventHandler<DisplayEvent> ea = null;
                ea = (o, e) => { c.OnDisplay -= ea;  tcs.SetResult(e);};
                c.OnDisplay += ea;
                var dc = new Person();

                string relp = "javascript\\navigation_1.html";
                string path = string.Format("{0}\\{1}", typeof(HTMLViewControl).Assembly.GetPath(), relp);
                var jvs = PrepareFiles();

                c.Mode = JavascriptBindingMode.OneWay; 
                w.Window.DataContext = dc;
                c.RelativeSource = relp;

                var de = await tcs.Task;

                foreach (string jv in jvs)
                {
                    string p = string.Format("{0}\\javascript\\src\\{1}", typeof(HTMLViewControl).Assembly.GetPath(), jv);
                    File.Delete(p);
                }
                File.Delete(path);
                de.Should().NotBeNull();
                de.DisplayedViewModel.Should().Be(dc);
            });
        }
    }
}
