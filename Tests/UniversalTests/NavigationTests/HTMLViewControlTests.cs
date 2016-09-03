using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using HTML_WPF.Component;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Navigation;
using MVVM.ViewModel.Example;
using Xunit;
using Tests.Infra.IntegratedContextTesterHelper.Window;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace IntegratedTest.Tests.WPF
{
    public abstract class HTMLViewControlTests : WpfComponentTest_Base<HTMLViewControl>
    {
        protected HTMLViewControlTests(IWindowContextProvider windowTestEnvironment) :
            base(windowTestEnvironment)
        {
        }

        protected override HTMLViewControl GetNewHTMLControlBase(bool iDebug)
        {
            return new HTMLViewControl { IsDebug = iDebug };
        }

        [Fact]
        public void Basic_Option()
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

                var relp = "javascript\\navigation_1.html";
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
                string path = $"{typeof(HTMLViewControl).Assembly.GetPath()}\\{relp}";
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
            await Test(async (viewControl, w) =>
            {
                var tcs = new TaskCompletionSource<DisplayEvent>();

                EventHandler<DisplayEvent> ea = null;
                ea = (o, e) => { tcs.TrySetResult(e); viewControl.OnDisplay -= ea; };
                viewControl.OnDisplay += ea;
                var dc = new Person();

                viewControl.Mode = JavascriptBindingMode.OneWay;
                viewControl.Uri = new Uri(GetPath(TestContext.Navigation1));
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
                //string path = $"{typeof(HTMLViewControl).Assembly.GetPath()}\\{relp}";
                //var jvs = PrepareFiles();

                c.Mode = JavascriptBindingMode.OneWay;
                c.RelativeSource = relp;
                w.Window.DataContext = dc;

                var de = await tcs.Task;

                de.Should().NotBeNull();
                de.DisplayedViewModel.Should().Be(dc);
            });
        }

        [Fact]
        public async Task Basic_Option_Simple_UsingRelativePath_AfterDataContext()
        {
            await Test(async (c, w) =>
            {
                var tcs = new TaskCompletionSource<DisplayEvent>();
                EventHandler<DisplayEvent> ea = null;
                ea = (o, e) => { c.OnDisplay -= ea; tcs.SetResult(e); };
                c.OnDisplay += ea;
                var dc = new Person();

                c.Mode = JavascriptBindingMode.OneWay;
                w.Window.DataContext = dc;
                c.RelativeSource = GetRelativePath(TestContext.Navigation1);

                var de = await tcs.Task;

                de.Should().NotBeNull();
                de.DisplayedViewModel.Should().Be(dc);
            });
        }
    }
}
