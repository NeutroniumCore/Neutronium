using AutoFixture.Xunit2;
using FluentAssertions;
using Neutronium.Core;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.Test.Helper;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Example.ViewModel;
using NSubstitute;
using System;
using System.Dynamic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Tests.Infra.WebBrowserEngineTesterHelper.Window;
using Tests.Universal.HTMLBindingTests.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Universal.HTMLBindingTests
{
    public abstract partial class HtmlBindingTests : HtmlBindingBase
    {
        protected HtmlBindingTests(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }

        [Fact]
        public async Task InvokeAsync_Does_Not_Throw_When_Called_With_Wrong_Name()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (binding) =>
                {
                    var js = binding.JsRootObject;
                    var res = js.InvokeAsync("NotFound", binding.Context).Result;
                    res.IsUndefined.Should().BeTrue();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task GetArrayElements_Throws_ArgumentException_When_Called_On_Object()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (binding) =>
                {
                    var js = binding.JsRootObject;
                    IJavascriptObject[] res = null;
                    Action act = () => res = js.GetArrayElements();
                    act.Should().Throw<ArgumentException>();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Bind_Throws_Exception_Without_Framework()
        {
            using (Tester(TestContext.EmptyWithJs))
            {
                NeutroniumException ex = null;

                try
                {
                    await HtmlBinding.Bind(_ViewEngine, new object(), JavascriptBindingMode.OneTime);
                }
                catch (AggregateException aggregate)
                {
                    ex = aggregate.Flatten().InnerException as NeutroniumException;
                }
                catch (NeutroniumException myex)
                {
                    ex = myex;
                }

                ex.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Bind_Throws_Exception_Without_CorrectJs()
        {
            using (Tester(TestContext.AlmostEmpty))
            {
                NeutroniumException ex = null;

                try
                {
                    await HtmlBinding.Bind(_ViewEngine, _DataContext, JavascriptBindingMode.OneTime);
                }
                catch (AggregateException aggregate)
                {
                    ex = aggregate.InnerExceptions[0] as NeutroniumException;
                }
                catch (NeutroniumException myex)
                {
                    ex = myex;
                }

                ex.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task OneTime_Checks_Context()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.OneTime),
                Test = (mb) =>
                {
                    mb.Context.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task OneTime_Maps_Object_But_Does_Not_Synchronize_Data()
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.OneTime),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    var res2 = GetStringAttribute(js, "LastName");
                    res2.Should().Be("Desmaisons");

                    await DoSafeAsyncUI(() => { _DataContext.Name = "23"; });

                    var res3 = GetStringAttribute(js, "Name");
                    res3.Should().Be("O Monstro");

                    var res4 = GetLocalCity(js);
                    res4.Should().Be("Florianopolis");

                    await DoSafeAsyncUI(() => { _DataContext.Local.City = "Paris"; });
                    
                    //onetime does not update javascript from  C# 
                    res4 = GetLocalCity(js);
                    res4.Should().Be("Florianopolis");

                    var res5 = GetFirstSkillName(js);
                    res5.Should().Be("Langage");

                    await DoSafeAsyncUI(() => { _DataContext.Skills[0].Name = "Ling"; });

                    //onetime does not update javascript from  C# 
                    res5 = GetFirstSkillName(js);
                    res5.Should().Be("Langage");

                    //onetime does not update C# from javascript
                    var stringName = Create(() => _WebView.Factory.CreateString("resName"));
                    await SetAttributeAsync(js, "Name", stringName);

                    await DoSafeAsyncUI(() => { _DataContext.Name.Should().Be("23"); });
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task OneWay_Maps_Properties()
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    var res2 = GetStringAttribute(js, "LastName");
                    res2.Should().Be("Desmaisons");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.Name = "23";
                    });
                    await Task.Delay(200);

                    var res3 = GetStringAttribute(js, "Name");
                    res3.Should().Be("23");

                    var res4 = GetLocalCity(js);
                    res4.Should().Be("Florianopolis");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.Local.City = "Paris";
                    });
                    await Task.Delay(300);

                    res4 = GetLocalCity(js);
                    res4.Should().Be("Paris");

                    var res5 = GetFirstSkillName(js);
                    res5.Should().Be("Langage");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.Skills[0].Name = "Ling";
                    });
                    await Task.Delay(300);

                    res5 = GetFirstSkillName(js);
                    res5.Should().Be("Ling");

                    var stringName = Create(() => _WebView.Factory.CreateString("resName"));
                    await SetAttributeAsync(js, "Name", stringName);

                    await DoSafeAsyncUI(()=>  _DataContext.Name.Should().Be("23"));
                }
            };

            await RunAsync(test);
        }

        private string GetLocalCity(IJavascriptObject js)
        {
            return GetStringAttribute(GetAttribute(js, "Local"), "City"); ;
        }

        private string GetFirstSkillName(IJavascriptObject javascriptObject)
        {
            var firstSkill = GetCollectionAttribute(javascriptObject, "Skills").GetValue(0);
            return GetStringAttribute(firstSkill, "Name");
        }

        [Fact]
        public async Task OneWay_Property_With_Exception()
        {
            var dt = new VmThatThrowsException();

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, dt, JavascriptBindingMode.OneWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetIntAttribute(js, "Int");
                    res.Should().Be(5);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task RootViewModel_Can_Be_Extended_By_Computed_Properties()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (mb) =>
                {
                    var js = GetRootViewModel();

                    var res = GetStringAttribute(js, "completeName");
                    res.Should().Be("O Monstro Desmaisons");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_Null_Property()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetAttribute(js, "MainSkill");
                    res.IsNull.Should().BeTrue();

                    await DoSafeAsyncUI(() => _DataContext.MainSkill = new Skill()
                    {
                        Name = "C++",
                        Type = "Info"
                    });

                    await Task.Delay(200);

                    res = GetAttribute(js, "MainSkill");
                    //DoSafe(() =>
                    //{
                        res.IsNull.Should().BeFalse();
                        res.IsObject.Should().BeTrue();
                    //});

                    var inf = GetStringAttribute(res, "Type");
                    inf.Should().Be("Info");

                    await DoSafeAsyncUI(() => _DataContext.MainSkill = null);
                    await Task.Delay(200);

                    res = GetAttribute(js, "MainSkill");

                    //GetSafe(()=>res.IsNull).Should().BeTrue();
                    //Awesomium limitation can not test on isnull
                    //Todo: create specific test
                    var obj = default(object);
                    var boolres = GetSafe(() => _WebView.Converter.GetSimpleValue(res, out obj));
                    boolres.Should().BeTrue();
                    obj.Should().BeNull();
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_Circular_reference()
        {
            var dataContext = new Neutronium.Example.ViewModel.ForNavigation.Couple();
            var my = new Neutronium.Example.ViewModel.ForNavigation.Person
            {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new Local() {City = "Florianopolis", Region = "SC"},
                Couple = dataContext
            };
            dataContext.One = my;

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var One = GetAttribute(js, "One");

                    var res = GetStringAttribute(One, "Name");
                    res.Should().Be("O Monstro");

                    var res2 = GetStringAttribute(One, "LastName");
                    res2.Should().Be("Desmaisons");

                    //Test no stackoverflow in case of circular reference
                    var jsbridge = (mb as HtmlBinding).JsBrideRootObject;
                    string alm = jsbridge.ToString();
                    alm.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Updates_JS_From_CSharp()
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.Name = "23";
                    });

                    await Task.Delay(350);

                    var res3 = GetStringAttribute(js, "Name");
                    res3.Should().Be("23");
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Update_CSharp_From_JS()
        {
            var dataContext = new SimpleViewModel() { Name = "teste0" };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetStringAttribute(js, "Name");
                    res.Should().Be("teste0");

                    var expected = "teste1";
                    var stringValue = Create(() => _WebView.Factory.CreateString(expected));
                    await SetAttributeAsync(js, "Name", stringValue);

                    dataContext.Name.Should().Be(expected);
                    _Logger.Info("Test ended sucessfully");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Works_With_Nullable()
        {
            var dataContext = new NullableTestViewModel
            {
                Bool = true,
                Int32 = 25
            };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetBoolAttribute(js, nameof(dataContext.Bool));
                    res.Should().Be(true);

                    var res2 = GetIntAttribute(js, nameof(dataContext.Int32));
                    res2.Should().Be(25);

                    var res3 = GetAttribute(js, nameof(dataContext.Decimal));
                    res3.IsNull.Should().BeTrue();

                    //Test Two Way Decimal: null => value
                    var newDecimal = Create(() => _WebView.Factory.CreateDouble(0.5));
                    await SetAttributeAsync(js, nameof(dataContext.Decimal), newDecimal);

                    var newValue = GetDoubleAttribute(js, nameof(dataContext.Decimal));
                    newValue.Should().Be(0.5);

                    await Task.Delay(50);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.Decimal.Should().Be(0.5m);
                    });

                    //Test Two Way bool value => null
                    var nullJs = Create(() => _WebView.Factory.CreateNull());
                    await SetAttributeAsync(js, nameof(dataContext.Bool), nullJs);

                    var boolValue = GetAttribute(js, nameof(dataContext.Bool));
                    boolValue.IsNull.Should().BeTrue();

                    await Task.Delay(50);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.Bool.Should().NotHaveValue();
                    });

                    //Test Two Way int value => value
                    var intValueJS = Create(() => _WebView.Factory.CreateInt(54));
                    await SetAttributeAsync(js, nameof(dataContext.Int32), intValueJS);

                    var intValue = GetIntAttribute(js, nameof(dataContext.Int32));
                    intValue.Should().Be(54);

                    await Task.Delay(50);

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.Int32.Should().Be(54);
                    });
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Synchronizes_Data_From_CSharp_To_Javascript()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    var res2 = GetStringAttribute(js, "LastName");
                    res2.Should().Be("Desmaisons");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.Name = "23";
                    });

                    await Task.Delay(50);

                    var res3 = GetStringAttribute(js, "Name");
                    res3.Should().Be("23");

                    var res4 = GetLocalCity(js);
                    res4.Should().Be("Florianopolis");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.Local.City = "Paris";
                    });
                    await Task.Delay(50);

                    res4 = GetLocalCity(js);
                    res4.Should().Be("Paris");

                    var res5 = GetFirstSkillName(js);
                    res5.Should().Be("Langage");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.Skills[0].Name = "Ling";
                    });
                    await Task.Delay(50);

                    res5 = GetFirstSkillName(js);
                    res5.Should().Be("Ling");

                    //Test Two Way
                    var stringName = Create(() => _WebView.Factory.CreateString("resName"));
                    await SetAttributeAsync(js, "Name", stringName);

                    var resName = GetStringAttribute(js, "Name");
                    resName.Should().Be("resName");

                    await Task.Delay(150);

                    _DataContext.Name.Should().Be("resName");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.Name = "nnnnvvvvvvv";
                    });

                    await Task.Delay(50);
                    res3 = GetStringAttribute(js, "Name");
                    res3.Should().Be("nnnnvvvvvvv");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_Nested()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    var res2 = GetStringAttribute(js, "LastName");
                    res2.Should().Be("Desmaisons");

                    var local = GetAttribute(js, "Local");
                    var city = GetStringAttribute(local, "City");
                    city.Should().Be("Florianopolis");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.Local.City = "Foz de Iguaçu";
                    });

                    await Task.Delay(100);
                    var city3 = GetStringAttribute(local, "City");
                    city3.Should().Be("Foz de Iguaçu");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.Local = new Local() { City = "Paris" };
                    });

                    await Task.Delay(100);
                    var local2 = GetAttribute(js, "Local");
                    var city2 = GetStringAttribute(local2, "City");
                    city2.Should().Be("Paris");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_Enum()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetAttribute(js, "PersonalState");
                    var dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                    dres.Should().Be("Married");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.PersonalState = PersonalState.Single;
                    });

                    res = GetAttribute(js, "PersonalState");
                    dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                    dres.Should().Be("Single");
                }
            };

            await RunAsync(test);

        }

        [Fact]
        public async Task TwoWay_Maps_Enum_From_CSharp_To_Javascript()
        {
            _DataContext.Name = "toto";

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetAttribute(js, "PersonalState");
                    var dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                    dres.Should().Be("Married");

                    await DoSafeAsyncUI(() =>
                    {
                        _DataContext.PersonalState = PersonalState.Single;
                    });
                    await Task.Delay(50);

                    res = GetAttribute(js, "PersonalState");
                    dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                    dres.Should().Be("Single");

                    var othervalue = GetCollectionAttribute(js, "States");

                    var di = othervalue.GetValue(2);
                    var name = di.GetValue("displayName").GetStringValue();
                    name.Should().Be("Divorced");

                    await SetAttributeAsync(js, "PersonalState", di);

                    _DataContext.PersonalState.Should().Be(PersonalState.Divorced);
                }
            };

            await RunAsync(test);
        }
     
        [Fact]
        public async Task TwoWay_Maps_ExpandoObject()
        {
            dynamic dynamicDataContext = new ExpandoObject();
            dynamicDataContext.ValueInt = 1;
            dynamicDataContext.ValueString = "string";
            dynamicDataContext.ValueBool = true;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dynamicDataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var resInt = GetIntAttribute(js, "ValueInt");
                    resInt.Should().Be(1);

                    var resString = GetStringAttribute(js, "ValueString");
                    resString.Should().Be("string");

                    var resBool = GetBoolAttribute(js, "ValueBool");
                    resBool.Should().BeTrue();

                    await DoSafeAsyncUI(() => { dynamicDataContext.ValueInt = 110; });

                    await Task.Delay(50);

                    resInt = GetIntAttribute(js, "ValueInt");
                    resInt.Should().Be(110);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Updates_ExpandoObject()
        {
            var expected = "newValueString";
            dynamic dynamicDataContext = new ExpandoObject();
            dynamicDataContext.ValueInt = 1;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dynamicDataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    if (!SupportDynamicBinding)
                    {
                        _Logger.Info("Test not supported for this framework");
                        return;
                    }

                    var js = mb.JsRootObject;

                    var res = GetAttribute(js, "NewValue");
                    res.IsUndefined.Should().BeTrue();

                    await DoSafeAsyncUI(() => 
                    {
                        dynamicDataContext.NewValue = expected;
                    });

                    await Task.Delay(50);

                    var stringValue = GetStringAttribute(js, "NewValue");
                    stringValue.Should().Be(expected);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_DynamicObject()
        {
            var dynamicDataContext = new TestDynamicObject();

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, dynamicDataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var resInt = GetIntAttribute(js, "Property5");
                    resInt.Should().Be(5);

                    resInt = GetIntAttribute(js, "Property0");
                    resInt.Should().Be(0);

                    resInt = GetIntAttribute(js, "Property1");
                    resInt.Should().Be(1);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_Enum_Initially_NotMapped()
        {
            var dataContext = new SimplePersonViewModel
            {
                PersonalState = PersonalState.Single
            };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetAttribute(js, "PersonalState");
                    var dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                    dres.Should().Be("Single");

                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.PersonalState = PersonalState.Married;
                    });
                    await Task.Delay(50);

                    res = GetAttribute(js, "PersonalState");
                    dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                    dres.Should().Be("Married");
                }

            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Set_Object_From_Javascript()
        {
            var dataContext = new Couple();
            var p1 = new Person() { Name = "David" };
            dataContext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            dataContext.Two = p2;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res1 = GetAttribute(js, "One");
                    var n1 = GetStringAttribute(res1, "Name");
                    n1.Should().Be("David");

                    var res2 = GetAttribute(js, "Two");
                    res2.Should().NotBeNull();
                    var n2 = GetStringAttribute(res2, "Name");
                    n2.Should().Be("Claudia");

                    var jsValue = GetAttribute(js, "Two");
                    await SetAttributeAsync(js, "One", jsValue);

                    var res3 = GetAttribute(js, "One");
                    res3.Should().NotBeNull();
                    var n3 = GetStringAttribute(res3, "Name");
                    n3.Should().Be("Claudia");

                    await Task.Delay(100);

                    dataContext.One.Should().Be(p2);

                    var res4 = GetAttribute(res3, "ChildrenNumber");
                    res4.IsNull.Should().BeTrue();

                    var five = Create(() => _WebView.Factory.CreateInt(5));
                    await SetAttributeAsync(res3, "ChildrenNumber", five);

                    dataContext.One.ChildrenNumber.Should().Be(5);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Set_Null_From_Javascript()
        {
            var dataContext = new Couple();
            var p1 = new Person() { Name = "David" };
            dataContext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            dataContext.Two = p2;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res1 = GetAttribute(js, "One");
                    res1.Should().NotBeNull();
                    var n1 = GetStringAttribute(res1, "Name");
                    n1.Should().Be("David");

                    var res2 = GetAttribute(js, "Two");
                    res2.Should().NotBeNull();
                    var n2 = GetStringAttribute(res2, "Name");
                    n2.Should().Be("Claudia");

                    var nullSO = Create(() => _WebView.Factory.CreateNull());
                    await SetAttributeAsync(js, "One", nullSO);

                    //var res3 = GetAttribute(js, "One");
                    //GetSafe(() => res3.IsNull).Should().BeTrue();
                    //Init case of awesomium an object is used on JS side
                    //Todo: create specific test

                    dataContext.One.Should().BeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Set_Object_From_Javascipt_Survive_MissUse()
        {
            var dataContext = new Couple();
            var p1 = new Person() { Name = "David" };
            dataContext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            dataContext.Two = p2;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var res1 = GetAttribute(js, "One");
                    res1.Should().NotBeNull();
                    var n1 = GetStringAttribute(res1, "Name");
                    n1.Should().Be("David");

                    var res2 = GetAttribute(js, "Two");
                    res2.Should().NotBeNull();
                    var n2 = GetStringAttribute(res2, "Name");
                    n2.Should().Be("Claudia");

                    var stringJs = _WebView.Factory.CreateString("Dede");
                    await SetAttributeAsync(js, "One", stringJs);

                    var res3 = GetStringAttribute(js, "One");
                    res3.Should().Be("Dede");

                    dataContext.One.Should().Be(p1);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Set_Object_From_Javascipt_Survive_MissUse_NoReset_OnAttribute()
        {
            var dataContext = new Couple();
            var p1 = new Person() { Name = "David" };
            dataContext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            dataContext.Two = p2;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var res1 = GetAttribute(js, "One");
                    res1.Should().NotBeNull();
                    var n1 = GetStringAttribute(res1, "Name");
                    n1.Should().Be("David");

                    var res2 = GetAttribute(js, "Two");
                    res2.Should().NotBeNull();
                    var n2 = GetStringAttribute(res2, "Name");
                    n2.Should().Be("Claudia");

                    var trueJs = _WebView.Factory.CreateObject(ObjectObservability.None);
                    await SetAttributeAsync(js, "One", trueJs);

                    var res3 = GetAttribute(js, "One");
                    res3.IsObject.Should().BeTrue();

                    await DoSafeAsyncUI(()=> dataContext.One.Should().Be(p1));
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task Property_Survives_Missuse_Of_NotifyPropertyChanged()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new FakeTestViewModel() { Command = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetStringAttribute(js, "Name");
                    res.Should().Be("NameTest");

                    var stringJS = _WebView.Factory.CreateString("NewName");
                    await SetAttributeAsync(js, "Name", stringJS);
                    res = GetStringAttribute(js, "Name");
                    res.Should().Be("NewName");

                    await DoSafeAsyncUI(()=> datacontexttest.Name.Should().Be("NameTest"));

                    var resFalse = GetSafe(() => js.HasValue("UselessName"));
                    resFalse.Should().BeFalse();

                    Action safe = () =>  datacontexttest.InconsistentEventEmit();
                    await DoSafeAsyncUI(() => safe.Should().NotThrow("Inconsistent Name in property should not throw exception"));
                }
            };

            await RunAsync(test);
        }

        private void CheckIntValue(IJavascriptObject js, string pn, int value)
        {
            var res = GetAttribute(js, pn);
            res.Should().NotBeNull();
            res.IsNumber.Should().BeTrue();
            res.GetIntValue().Should().Be(value);
        }

        [Fact]
        public async Task TwoWay_Maps_Number_Type_From_CSharp_0()
        {
            var dataContext = new ClrTypesTestViewModel();

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;
                    js.Should().NotBeNull();

                    CheckIntValue(js, "Int64", 0);
                    CheckIntValue(js, "Int32", 0);
                    CheckIntValue(js, "Int16", 0);

                    CheckIntValue(js, "Uint16", 0);
                    CheckIntValue(js, "Uint32", 0);
                    CheckIntValue(js, "Uint64", 0);

                    CheckIntValue(js, "Double", 0);
                    CheckIntValue(js, "Decimal", 0);
                    CheckIntValue(js, "Float", 0);

                    CheckIntValue(js, "Byte", 0);
                    CheckIntValue(js, "Sbyte", 0);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Maps_Number_Type_From_CSharp()
        {
            var dataContext = new ClrTypesTestViewModel
            {
                Int64 = 32,
                Uint64 = 456,
                Int32 = 231,
                Uint32 = 77,
                Int16 = 55,
                Uint16 = 23,
                Float = 18.19f,
                Double = 55.88,
                Decimal = 47.89m,
                Byte = 12,
                Sbyte = -17
            };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;
                    js.Should().NotBeNull();

                    var res = GetIntAttribute(js, "Int64");
                    res.Should().Be((int)dataContext.Int64);

                    res = GetIntAttribute(js, "Uint64");
                    res.Should().Be((int)dataContext.Uint64);

                    res = GetIntAttribute(js, "Int32");
                    res.Should().Be(dataContext.Int32);

                    res = GetIntAttribute(js, "Uint32");
                    res.Should().Be((int)dataContext.Uint32);

                    res = GetIntAttribute(js, "Int16");
                    res.Should().Be(dataContext.Int16);

                    res = GetIntAttribute(js, "Uint16");
                    res.Should().Be(dataContext.Uint16);

                    var res2 = GetDoubleAttribute(js, "Float");
                    res2.Should().Be(dataContext.Float);

                    res2 = GetDoubleAttribute(js, "Double");
                    res2.Should().Be(dataContext.Double);

                    res2 = GetDoubleAttribute(js, "Decimal");
                    res2.Should().Be((double)dataContext.Decimal);

                    res = GetIntAttribute(js, "Byte");
                    res.Should().Be(dataContext.Byte);

                    res = GetIntAttribute(js, "Sbyte");
                    res.Should().Be(dataContext.Sbyte);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Updates_CLR_Types_From_Javascript()
        {
            var dataContext = new ClrTypesTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    js.Should().NotBeNull();

                    await SetAttributeAsync(js, "Int64", _WebView.Factory.CreateInt(32));
                    await DoSafeAsyncUI(() => dataContext.Int64.Should().Be(32));

                    await SetAttributeAsync(js, "Uint64", _WebView.Factory.CreateInt(456));
                    await DoSafeAsyncUI(() => dataContext.Uint64.Should().Be(456));

                    await SetAttributeAsync(js, "Int32", _WebView.Factory.CreateInt(5));
                    await DoSafeAsyncUI(() => dataContext.Int32.Should().Be(5));

                    await SetAttributeAsync(js, "Uint32", _WebView.Factory.CreateInt(67));
                    await DoSafeAsyncUI(() => dataContext.Uint32.Should().Be(67));

                    await SetAttributeAsync(js, "Int16", _WebView.Factory.CreateInt(-23));
                    await DoSafeAsyncUI(() => dataContext.Int16.Should().Be(-23));

                    await SetAttributeAsync(js, "Uint16", _WebView.Factory.CreateInt(9));
                    await DoSafeAsyncUI(() => dataContext.Uint16.Should().Be(9));

                    await SetAttributeAsync(js, "Float", _WebView.Factory.CreateDouble(888.78));
                    await DoSafeAsyncUI(() => dataContext.Float.Should().Be(888.78f));

                    await SetAttributeAsync(js, "Double", _WebView.Factory.CreateDouble(866.76));
                    await DoSafeAsyncUI(() => dataContext.Double.Should().Be(866.76));

                    await SetAttributeAsync(js, "Decimal", _WebView.Factory.CreateDouble(0.5));
                    await DoSafeAsyncUI(() => dataContext.Decimal.Should().Be(0.5m));

                    await SetAttributeAsync(js, "Byte", _WebView.Factory.CreateInt(10));
                    await DoSafeAsyncUI(() => dataContext.Byte.Should().Be(10));

                    await SetAttributeAsync(js, "Sbyte", _WebView.Factory.CreateInt(-12));
                    await DoSafeAsyncUI(() => dataContext.Sbyte.Should().Be(-12));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Updates_Decimal_From_Javascript()
        {
            var dataContext = new VmWithDecimal();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetIntAttribute(js, "decimalValue");
                    res.Should().Be(0);

                    var halfJavascript = Create(() => _WebView.Factory.CreateDouble(0.5));
                    await SetAttributeAsync(js, "decimalValue", halfJavascript);

                    await DoSafeAsyncUI(() => dataContext.decimalValue.Should().Be(0.5m));

                    var doubleValue = GetDoubleAttribute(js, "decimalValue");
                    const double half = 0.5;
                    doubleValue.Should().Be(half);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Updates_Long_From_Javascript()
        {
            var dataContext = new VmWithLong() { longValue = 45 };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var doublev = GetDoubleAttribute(js, "longValue");
                    doublev.Should().Be(45);

                    var intValue = 24524;
                    var jsInt = Create(() => _WebView.Factory.CreateInt(intValue));
                    await SetAttributeAsync(js, "longValue", jsInt);

                    await DoSafeAsyncUI(() => dataContext.longValue.Should().Be(24524));

                    doublev = GetDoubleAttribute(js, "longValue");
                    long half = 24524;
                    doublev.Should().Be(half);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task String_Binding_Maps_Properties()
        {
            var test = new TestInContext()
            {
                Bind = (win) => Neutronium.Core.StringBinding.Bind(win, "{\"LastName\":\"Desmaisons\",\"Name\":\"O Monstro\",\"BirthDay\":\"0001-01-01T00:00:00.000Z\",\"PersonalState\":\"Married\",\"Age\":0,\"Local\":{\"City\":\"Florianopolis\",\"Region\":\"SC\"},\"MainSkill\":{},\"States\":[\"Single\",\"Married\",\"Divorced\"],\"Skills\":[{\"Type\":\"French\",\"Name\":\"Langage\"},{\"Type\":\"C++\",\"Name\":\"Info\"}]}"),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    mb.Root.Should().BeNull();

                    mb.Context.Should().NotBeNull();

                    var res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    var res2 = GetStringAttribute(js, "LastName");
                    res2.Should().Be("Desmaisons");

                    var res4 = GetLocalCity(js);
                    res4.Should().Be("Florianopolis");

                    var res5 = GetFirstSkillName(js);
                    res5.Should().Be("Langage");
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task Bind_Throws_Exception_When_Html_Without_Correct_js()
        {
            using (Tester(TestContext.AlmostEmpty))
            {
                var vm = new object();
                NeutroniumException ex = null;

                try
                {
                    await HtmlBinding.Bind(_ViewEngine, vm, JavascriptBindingMode.OneTime);
                }
                catch (AggregateException aggregate)
                {
                    ex = aggregate.InnerExceptions[0] as NeutroniumException;
                }
                catch (NeutroniumException myex)
                {
                    ex = myex;
                }

                ex.Should().NotBeNull();
            }
        }


        [Fact]
        public async Task TwoWay_Rebinds_With_Updated_Objects()
        {
            var child = new BasicTestViewModel();
            var dataContext = new BasicTestViewModel { Child = child };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    await DoSafeAsyncUI(() => dataContext.Child = null);

                    await Task.Delay(300);

                    var third = new BasicTestViewModel();
                    child.Child = third;

                    await DoSafeAsyncUI(() => dataContext.Child = child);

                    await Task.Delay(300);

                    var child1 = GetAttribute(js, "Child");
                    var child2 = GetAttribute(child1, "Child");

                    var value = GetIntAttribute(child2, "Value");
                    value.Should().Be(-1);
                }
            };

            await RunAsync(test);
        }

        [Theory, AutoData]
        public async Task Binding_Propagates_Updates_Occurring_During_Binding(int value)
        {
            var dataContext = new Person()
            {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new Local() { City = "Florianopolis", Region = "SC" },
                PersonalState = PersonalState.Married
            };
            dataContext.Skills.Add(new Skill() { Name = "Langage", Type = "French" });
            dataContext.Skills.Add(new Skill() { Name = "Info", Type = "C++" });
            var dispatcher = WpfThread.GetWpfThread().Dispatcher;
            var timer = new DispatcherTimer(DispatcherPriority.Send, dispatcher) { Interval = TimeSpan.FromMilliseconds(2) };

            void OnTimerOnTick(object o, EventArgs e)
            {
                dataContext.Count = value;
                timer.Tick -= OnTimerOnTick;
                timer.Stop();
            }
            timer.Tick += OnTimerOnTick;

            var test = new TestInContextAsync()
            {
                Bind = (win) =>
                {
                    timer.Start();
                    return Bind(win, dataContext, JavascriptBindingMode.TwoWay);
                },
                Test = async (mb) =>
                {
                    await Task.Delay(200);

                    var js = mb.JsRootObject;
                    var countJs = GetIntAttribute(js, "Count");
                    countJs.Should().Be(value);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Binding_Updates_To_Last_Value()
        {
            var dataContext = new Person();
            var lastValue = 2;

            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await DoSafeAsyncUI(() =>
                    {
                        dataContext.Count = -1;
                        dataContext.Count = 1560;
                        dataContext.Count = 26;
                        dataContext.Count = lastValue;
                    });

                    await Task.Delay(200);

                    var js = mb.JsRootObject;
                    var countJs = GetIntAttribute(js, "Count");
                    countJs.Should().Be(lastValue);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Respects_Property_Validation()
        {
            var dataContext = new VmWithValidationOnPropertySet
            {
                MagicNumber = 8
            };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetIntAttribute(js, "MagicNumber");
                    res.Should().Be(8);

                    var intValue = 9;
                    var jsInt = Create(() => _WebView.Factory.CreateInt(intValue));
                    await SetAttributeAsync(js, "MagicNumber", jsInt);

                    await DoSafeAsyncUI(()=>  dataContext.MagicNumber.Should().Be(42));
                    await Task.Delay(100);

                    res = GetIntAttribute(js, "MagicNumber");
                    res.Should().Be(42);
                }
            };

            await RunAsync(test);
        }
    }
}

