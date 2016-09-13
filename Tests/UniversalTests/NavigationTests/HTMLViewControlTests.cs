using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Neutronium.Core;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using Neutronium.Core.Navigation;
using Neutronium.Example.ViewModel;
using Neutronium.WPF;
using Tests.Infra.IntegratedContextTesterHelper.Window;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Xunit;

namespace Tests.Universal.NavigationTests
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
        public void DefaultMode_IsTwoWay()
        {
            Test((c) =>
            {
                c.SessionPath.Should().BeNull();
                c.Mode.Should().Be(JavascriptBindingMode.TwoWay);
                c.Uri.Should().BeNull();
            });
        }

        [Fact]
        public async Task OnDisplay_WhenDataContextChanges_IsFired()
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

        [Fact]
        public async Task SetRelativePath_AfterSetDataContext_TriggersABinding()
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
                act.ShouldThrow<NeutroniumException>();
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
    }
}
