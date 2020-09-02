using FluentAssertions;
using Neutronium.Core;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Example.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Tests.Universal.HTMLBindingTests.Helper;
using Xunit;

namespace Tests.Universal.HTMLBindingTests
{
    public abstract partial class HtmlBindingTests
    {
        private static void CheckStringCollection(IJavascriptObject actual, IEnumerable<string> expected)
        {
            var javaCollection = Enumerable.Range(0, actual.GetArrayLength()).Select(i => actual.GetValue(i).GetStringValue());
            javaCollection.Should().Equal(expected);
        }

        private static void CheckIntCollection(IJavascriptObject actual, IEnumerable<int> expected)
        {
            var javaCollection = Enumerable.Range(0, actual.GetArrayLength()).Select(i => actual.GetValue(i).GetIntValue());
            javaCollection.Should().Equal(expected);
        }

        private static void CheckDecimalCollection(IJavascriptObject coll, IList<decimal> skill)
        {
            coll.GetArrayLength().Should().Be(skill.Count);

            for (var i = 0; i < skill.Count; i++)
            {
                var c = (decimal)coll.GetValue(i).GetDoubleValue();
                c.Should().Be(skill[i]);
            }
        }

        [Fact]
        public async Task TwoWay_Maps_Collection()
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var col = GetCollectionAttribute(js, "Skills");
                    col.Should().NotBeNull();
                    col.GetArrayLength().Should().Be(2);
                    CheckCollection(col, _DataContext.Skills);

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills.Add(new Skill() { Name = "C++", Type = "Info" });
                    });

                    col = await GetCollectionAttributeAsync(js, "Skills");
                    col.Should().NotBeNull();
                    CheckCollection(col, _DataContext.Skills);

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills.Insert(0, new Skill() { Name = "C#", Type = "Info" });
                    });
                    col = await GetCollectionAttributeAsync(js, "Skills");
                    col.Should().NotBeNull();
                    CheckCollection(col, _DataContext.Skills);

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills.RemoveAt(1);
                    });
                    col = await GetCollectionAttributeAsync(js, "Skills");
                    col.Should().NotBeNull();
                    CheckCollection(col, _DataContext.Skills);

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills[0] = new Skill() { Name = "HTML", Type = "Info" };
                    });
                    col = await GetCollectionAttributeAsync(js, "Skills");
                    col.Should().NotBeNull();
                    CheckCollection(col, _DataContext.Skills);

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills[0] = new Skill() { Name = "HTML5", Type = "Info" };
                        _DataContext.Skills.Insert(0, new Skill() { Name = "HTML5", Type = "Info" });
                    });
                    col = await GetCollectionAttributeAsync(js, "Skills");
                    col.Should().NotBeNull();
                    CheckCollection(col, _DataContext.Skills);

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills.Clear();
                    });
                    col = await GetCollectionAttributeAsync(js, "Skills");
                    col.Should().NotBeNull();
                    CheckCollection(col, _DataContext.Skills);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_Updates_Grouped_Changes()
        {
            _DataContext.Skills.Add(new Skill() { Name = "C++", Type = "Info" });
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        var skills = _DataContext.Skills;
                        skills.Add(new Skill() { Name = "C++", Type = "Info" });
                        skills.RemoveAt(2);
                        skills.RemoveAt(0);
                        skills[0] = new Skill() { Name = "Vue.js", Type = "Info" };
                        skills.Add(new Skill() { Name = "C++", Type = "Info" });
                    });

                    await WaitAnotherUiCycle();

                    var col = await GetCollectionAttributeAsync(js, "Skills");
                    col.Should().NotBeNull();
                    CheckCollection(col, _DataContext.Skills);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_Updates_After_AddRange_Changes()
        {
            var dataContext = new VmWithRangeCollection<int>();
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var addedItems = new[] { 1, 2, 3, 4 };
                    var js = mb.JsRootObject;

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.AddRange(addedItems);
                    });

                    var col = await GetCollectionAttributeAsync(js, "List");
                    col.Should().NotBeNull();
                    CheckIntCollection(col, addedItems);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_Updates_After_InsertRange_Changes()
        {
            var dataContext = new VmWithRangeCollection<int>();
            dataContext.List.AddRange(new[] { 1, 4 });
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.InsertRange(1, new[] { 2, 3 });
                    });

                    var col = await GetCollectionAttributeAsync(js, "List");
                    col.Should().NotBeNull();
                    CheckIntCollection(col, new[] { 1, 2, 3, 4 });
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Collection_Updates_After_RemoveRange_Changes()
        {
            var dataContext = new VmWithRangeCollection<int>();
            dataContext.List.AddRange(new[] { 1, 2, 3, 4, 5 });
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.RemoveRange(1, 3);
                    });

                    var col = await GetCollectionAttributeAsync(js, "List");
                    col.Should().NotBeNull();
                    CheckIntCollection(col, new[] { 1, 5 });
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_Updates_After_ReplaceRange_Changes()
        {
            var dataContext = new VmWithRangeCollection<int>();
            dataContext.List.AddRange(new[] { 1, 20, 30, 40, 5 });
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.ReplaceRange(1, 3, new[] { 2, 3, 4 });
                    });

                    var col = await GetCollectionAttributeAsync(js, "List");
                    col.Should().NotBeNull();
                    CheckIntCollection(col, new[] { 1, 2, 3, 4, 5 });
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_Updates_After_ReplaceRange_One_Collection_Changes()
        {
            var dataContext = new VmWithRangeCollection<int>();
            dataContext.List.AddRange(new[] { 100, 200 });
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.ReplaceRange(new[] { 1, 2, 3, 4, 5 });
                    });

                    var col = await GetCollectionAttributeAsync(js, "List");
                    col.Should().NotBeNull();
                    CheckIntCollection(col, new[] { 1, 2, 3, 4, 5 });
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_Updates_CSharp_From_JS_Update()
        {
            var test = new TestInContextAsync()
            {
                Path = TestContext.Simple,
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var root = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;
                    var js = mb.JsRootObject;

                    var col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    col.GetArrayLength().Should().Be(2);

                    CheckCollection(col, _DataContext.Skills);

                    var coll = GetAttribute(js, "Skills");
                    await CallAsync(coll, "push", (root.GetAttribute("Skills") as JsArray).Items[0].GetJsSessionValue());

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills.Should().HaveCount(3);
                        _DataContext.Skills[2].Should().Be(_DataContext.Skills[0]);
                    });

                    col = await GetCollectionAttributeAsync(js, "Skills");
                    CheckCollection(col, _DataContext.Skills);

                    await CallAsync(coll, "pop");

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills.Should().HaveCount(2);
                    });
                    col = await GetCollectionAttributeAsync(js, "Skills");
                    col.Should().NotBeNull();
                    CheckCollection(col, _DataContext.Skills);

                    await CallAsync(coll, "shift");

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills.Should().HaveCount(1);
                    });
                    col = await GetCollectionAttributeAsync(js, "Skills");
                    CheckCollection(col, _DataContext.Skills);


                    await CallAsync(coll, "unshift",
                          (root.GetAttribute("Skills") as JsArray).Items[0].GetJsSessionValue());

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills.Should().HaveCount(2);
                    });
                    col = await GetCollectionAttributeAsync(js, "Skills");
                    CheckCollection(col, _DataContext.Skills);

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills.Add(new Skill() { Type = "Langage", Name = "French" });
                    });
                    await DoSafeAsyncUI(()=> _DataContext.Skills.Should().HaveCount(3));
                    col = await GetCollectionAttributeAsync(js, "Skills");
                    col.Should().NotBeNull();
                    CheckCollection(col, _DataContext.Skills);

                    await CallAsync(coll, "reverse");

                    await DoSafeAsyncUIFullCycle(() =>
                    {
                        _DataContext.Skills.Should().HaveCount(3);
                    });
                    col = await GetCollectionAttributeAsync(js, "Skills");
                    CheckCollection(col, _DataContext.Skills);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_String_Collection()
        {
            var dataContext = new VmWithList<string>();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var col = await GetCollectionAttributeAsync(js, "List");
                    col.GetArrayLength().Should().Be(0);
                    CheckStringCollection(col, dataContext.List);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.Add("titi");
                    });
                    col = await GetCollectionAttributeAsync(js, "List");
                    CheckStringCollection(col, dataContext.List);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.Add("kiki");
                        dataContext.List.Add("toto");
                    });
                    col = await GetCollectionAttributeAsync(js, "List");
                    CheckStringCollection(col, dataContext.List);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.Move(0, 2);
                    });
                    col = await GetCollectionAttributeAsync(js, "List");
                    CheckStringCollection(col, dataContext.List);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.Move(2, 1);
                    });
                    col = await GetCollectionAttributeAsync(js, "List");
                    CheckStringCollection(col, dataContext.List);

                    var comp = new List<string>(dataContext.List) { "newvalue" };

                    col = await GetCollectionAttributeAsync(js, "List");
                    var chcol = GetAttribute(js, "List");
                    Call(chcol, "push", _WebView.Factory.CreateString("newvalue"));

                    col = await GetCollectionAttributeAsync(js, "List");

                    dataContext.List.Should().Equal(comp);
                    CheckStringCollection(col, dataContext.List);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.Clear();
                    });
                    col = await GetCollectionAttributeAsync(js, "List");
                    CheckStringCollection(col, dataContext.List);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Updates_Collection()
        {
            var datacontext = new ChangingCollectionViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var col = await GetCollectionAttributeAsync(js, "Items");
                    col.GetArrayLength().Should().NotBe(0);

                    await DoSafeAsyncUIFullCycle(() => datacontext.Replace.Execute(null));

                    datacontext.Items.Should().BeEmpty();

                    col = await GetCollectionAttributeAsync(js, "Items");
                    col.GetArrayLength().Should().Be(0);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_None_Generic_List()
        {
            var dataContext = new VmWithList();
            dataContext.List.Add(888);

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var col = await GetCollectionAttributeAsync(js, "List");
                    col.GetArrayLength().Should().Be(1);

                    var res = GetAttribute(js, "List");
                    await CallAsync(res, "push", _WebView.Factory.CreateString("newvalue"));

                    col = await GetCollectionAttributeAsync(js, "List");
                    col.GetArrayLength().Should().Be(2);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.Should().HaveCount(2);
                        dataContext.List[1].Should().Be("newvalue");
                    });
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_Decimal_Collection()
        {
            var dataContext = new VmWithList<decimal>();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var col = GetSafe(() => GetCollectionAttribute(js, "List"));
                    col.GetArrayLength().Should().Be(0);

                    CheckDecimalCollection(col, dataContext.List);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.Add(3);
                    });
                    col = await GetCollectionAttributeAsync(js, "List");

                    CheckDecimalCollection(col, dataContext.List);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.List.Add(10.5m);
                        dataContext.List.Add(126);
                    });
                    col = await GetCollectionAttributeAsync(js, "List");
                    CheckDecimalCollection(col, dataContext.List);

                    var comp = new List<decimal>(dataContext.List) { 0.55m };

                    var res = GetAttribute(js, "List");
                    await CallAsync(res, "push", _WebView.Factory.CreateDouble(0.55));

                    col = await GetCollectionAttributeAsync(js, "List");

                    await DoSafeAsyncUI(()=> comp.Should().Equal(dataContext.List));
                    CheckDecimalCollection(col, dataContext.List);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Survives_Collection_Update_From_Js_With_Wrong_Type()
        {
            var test = new TestInContextAsync()
            {
                Path = TestContext.Simple,
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var col = GetCollectionAttribute(js, "Skills");
                    col.GetArrayLength().Should().Be(2);

                    CheckCollection(col, _DataContext.Skills);

                    var coll = GetAttribute(js, "Skills");
                    await CallAsync(coll, "push", _WebView.Factory.CreateString("Whatever"));

                    await DoSafeAsyncUI(()=>  _DataContext.Skills.Should().HaveCount(2));
                }
            };

            await RunAsync(test);
        }
    }
}
