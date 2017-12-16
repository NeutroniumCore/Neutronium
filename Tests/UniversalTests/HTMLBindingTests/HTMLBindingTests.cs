using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAssertions;
using MoreCollection.Extensions;
using Newtonsoft.Json;
using NSubstitute;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Xunit;
using Xunit.Abstractions;
using Neutronium.Core;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Exceptions;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Example.ViewModel;
using Neutronium.Example.ViewModel.Infra;
using Neutronium.MVVMComponents;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Tests.Universal.HTMLBindingTests.Helper;
using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Test.Helper;
using Neutronium.MVVMComponents.Relay;

namespace Tests.Universal.HTMLBindingTests
{
    public abstract class HtmlBindingTests : HtmlBindingBase
    {
        protected HtmlBindingTests(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }

        [Fact]
        public async Task InvokeAsync_No_Function()
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
        public async Task GetElements_Should_Throw_ArgumentException()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (binding) =>
                {
                    var js = binding.JsRootObject;
                    IJavascriptObject[] res = null;
                    Action act = () => res = js.GetArrayElements();
                    act.ShouldThrow<ArgumentException>();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task OneWay_JSON_ToString()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (binding) =>
                {
                    var jsbridge = ((HtmlBinding)binding).JsBrideRootObject;
                    var alm = jsbridge.ToString();

                    JsArray arr = null;
                    jsbridge.VisitDescendantsSafe(glue =>
                    {
                        arr = arr ?? glue as JsArray;
                        return glue != null;
                    });

                    dynamic m = JsonConvert.DeserializeObject<dynamic>(jsbridge.ToString());
                    ((string)m.LastName).Should().Be("Desmaisons");
                    ((string)m.Name).Should().Be("O Monstro");

                    binding.ToString().Should().Be(alm);
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task Bind_WithoutFramework_ShouldThrowException()
        {
            using (Tester(TestContext.EmptyWithJs))
            {
                var vm = new object();
                NeutroniumException ex = null;

                try
                {
                    await HtmlBinding.Bind(_ViewEngine, new object(), JavascriptBindingMode.OneTime);
                }
                catch (AggregateException agregate)
                {
                    ex = agregate.Flatten().InnerException as NeutroniumException;
                }
                catch (NeutroniumException myex)
                {
                    ex = myex;
                }

                ex.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Bind_WithoutCorrectJs_ShouldThrowException()
        {
            using (Tester(TestContext.AlmostEmpty))
            {
                var vm = new object();
                NeutroniumException ex = null;

                try
                {
                    await HtmlBinding.Bind(_ViewEngine, _DataContext, JavascriptBindingMode.OneTime);
                }
                catch (NeutroniumException myex)
                {
                    ex = myex;
                }

                ex.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task OneTime_CheckContext()
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
        public async Task OneTime()
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.OneTime),
                Test = async (mb) =>
                {
                    var jsbridge = (mb as HtmlBinding).JsBrideRootObject;
                    var js = mb.JsRootObject;

                    string JSON = JsonConvert.SerializeObject(_DataContext);
                    string alm = jsbridge.ToString();

                    string res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    string res2 = GetStringAttribute(js, "LastName");
                    res2.Should().Be("Desmaisons");

                    _DataContext.Name = "23";
                    await Task.Delay(200);

                    string res3 = GetStringAttribute(js, "Name");
                    res3.Should().Be("O Monstro");

                    string res4 = GetLocalCity(js);
                    res4.Should().Be("Florianopolis");

                    _DataContext.Local.City = "Paris";
                    await Task.Delay(200);

                    //onetime does not update javascript from  C# 
                    res4 = GetLocalCity(js);
                    res4.Should().Be("Florianopolis");

                    string res5 = GetFirstSkillName(js);
                    res5.Should().Be("Langage");

                    _DataContext.Skills[0].Name = "Ling";
                    await Task.Delay(200);

                    //onetime does not update javascript from  C# 
                    res5 = GetFirstSkillName(js);
                    res5.Should().Be("Langage");

                    //onetime does not update C# from javascript
                    var stringName = Create(() => _WebView.Factory.CreateString("resName"));
                    SetAttribute(js, "Name", stringName);
                    await Task.Delay(200);
                    _DataContext.Name.Should().Be("23");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task OneWay()
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = async (mb) =>
                {
                    var jsbridge = (mb as HtmlBinding).JsBrideRootObject;
                    var js = mb.JsRootObject;

                    string JSON = JsonConvert.SerializeObject(_DataContext);
                    string alm = jsbridge.ToString();

                    string res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    string res2 = GetStringAttribute(js, "LastName");
                    res2.Should().Be("Desmaisons");

                    DoSafeUI(() =>
                    {
                        _DataContext.Name = "23";
                    });
                    await Task.Delay(200);

                    string res3 = GetStringAttribute(js, "Name");
                    res3.Should().Be("23");

                    string res4 = GetLocalCity(js);
                    res4.Should().Be("Florianopolis");

                    DoSafeUI(() =>
                    {
                        _DataContext.Local.City = "Paris";
                    });
                    await Task.Delay(200);

                    res4 = GetLocalCity(js);
                    ((string)res4).Should().Be("Paris");

                    string res5 = GetFirstSkillName(js);
                    res5.Should().Be("Langage");

                    DoSafeUI(() =>
                    {
                        _DataContext.Skills[0].Name = "Ling";
                    });
                    await Task.Delay(200);

                    res5 = GetFirstSkillName(js);
                    res5.Should().Be("Ling");

                    var stringName = Create(() => _WebView.Factory.CreateString("resName"));
                    SetAttribute(js, "Name", stringName);
                    await Task.Delay(200);
                    _DataContext.Name.Should().Be("23");
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

        private class Dummy
        {
            internal Dummy()
            {
                Int = 5;
            }
            public int Int { get; set; }
            public int Explosive { get { throw new Exception(); } }
        }


        [Fact]
        public async Task OneWay_Property_With_Exception()
        {
            var dt = new Dummy();

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, dt, JavascriptBindingMode.OneWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    int res = GetIntAttribute(js, "Int");
                    res.Should().Be(5);
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task RootViewModel_CanBeExtended_ByComputedProperties()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (mb) =>
                {
                    var js = GetRootViewModel();

                    string res = GetStringAttribute(js, "completeName");
                    res.Should().Be("O Monstro Desmaisons");
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task Null_Property()
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

                    DoSafeUI(() => _DataContext.MainSkill = new Skill()
                    {
                        Name = "C++",
                        Type = "Info"
                    });

                    await Task.Delay(200);

                    res = GetAttribute(js, "MainSkill");
                    DoSafe(() =>
                    {
                        res.IsNull.Should().BeFalse();
                        res.IsObject.Should().BeTrue();
                    });


                    string inf = GetStringAttribute(res, "Type");
                    inf.Should().Be("Info");

                    DoSafeUI(() => _DataContext.MainSkill = null);
                    await Task.Delay(200);

                    res = GetAttribute(js, "MainSkill");

                    //GetSafe(()=>res.IsNull).Should().BeTrue();
                    //Awesomium limitation can not test on isnull
                    //Todo: create specific test
                    object obj = null;
                    var boolres = GetSafe(() => _WebView.Converter.GetSimpleValue(res, out obj));
                    boolres.Should().BeTrue();
                    obj.Should().BeNull();
                }
            };
            await RunAsync(test);
        }


        [Fact]
        public async Task Circular_reference()
        {
            var datacontext = new Neutronium.Example.ViewModel.ForNavigation.Couple();
            var my = new Neutronium.Example.ViewModel.ForNavigation.Person()
            {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new Local() { City = "Florianopolis", Region = "SC" }
            };
            my.Couple = datacontext;
            datacontext.One = my;

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var One = GetAttribute(js, "One");

                    string res = GetStringAttribute(One, "Name");
                    res.Should().Be("O Monstro");

                    string res2 = GetStringAttribute(One, "LastName");
                    res2.Should().Be("Desmaisons");

                    //Test no stackoverflow in case of circular refernce
                    var jsbridge = (mb as HtmlBinding).JsBrideRootObject;
                    string alm = jsbridge.ToString();
                    alm.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_UpdateJSFromCSharp()
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    string res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    DoSafeUI(() =>
                    {
                        _DataContext.Name = "23";
                    });

                    await Task.Delay(350);

                    string res3 = GetStringAttribute(js, "Name");
                    res3.Should().Be("23");
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_UpdateCSharpFromJS()
        {
            var datacontext = new SimpleViewModel() { Name = "teste0" };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    string res = GetStringAttribute(js, "Name");
                    res.Should().Be("teste0");

                    var expected = "teste1";
                    var stringValue = Create(() => _WebView.Factory.CreateString(expected));
                    SetAttribute(js, "Name", stringValue);

                    await Task.Delay(50);

                    datacontext.Name.Should().Be(expected);
                    _Logger.Info("Test ended sucessfully");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Works_WithNullable()
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
                    SetAttribute(js, nameof(dataContext.Decimal), newDecimal);

                    await Task.Delay(50);
                    var newValue = GetDoubleAttribute(js, nameof(dataContext.Decimal));
                    newValue.Should().Be(0.5);

                    await Task.Delay(50);


                    DoSafeUI(() =>
                    {
                        dataContext.Decimal.Should().Be(0.5m);
                    });

                    //Test Two Way bool value => null
                    var nullJs = Create(() => _WebView.Factory.CreateNull());
                    SetAttribute(js, nameof(dataContext.Bool), nullJs);

                    await Task.Delay(50);
                    var boolValue = GetAttribute(js, nameof(dataContext.Bool));
                    boolValue.IsNull.Should().BeTrue();

                    await Task.Delay(50);

                    DoSafeUI(() =>
                    {
                        dataContext.Bool.Should().NotHaveValue();
                    });


                    //Test Two Way int value => value
                    var intValueJS = Create(() => _WebView.Factory.CreateInt(54));
                    SetAttribute(js, nameof(dataContext.Int32), intValueJS);

                    await Task.Delay(50);
                    var intValue = GetIntAttribute(js, nameof(dataContext.Int32));
                    intValue.Should().Be(54);

                    await Task.Delay(50);

                    DoSafeUI(() =>
                    {
                        dataContext.Int32.Should().Be(54);
                    });
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    string res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    string res2 = GetStringAttribute(js, "LastName");
                    res2.Should().Be("Desmaisons");

                    DoSafeUI(() =>
                    {
                        _DataContext.Name = "23";
                    });

                    await Task.Delay(50);

                    string res3 = GetStringAttribute(js, "Name");
                    res3.Should().Be("23");

                    string res4 = GetLocalCity(js);
                    res4.Should().Be("Florianopolis");

                    DoSafeUI(() =>
                    {
                        _DataContext.Local.City = "Paris";
                    });
                    await Task.Delay(50);

                    res4 = GetLocalCity(js);
                    ((string)res4).Should().Be("Paris");

                    string res5 = GetFirstSkillName(js);
                    res5.Should().Be("Langage");

                    DoSafeUI(() =>
                    {
                        _DataContext.Skills[0].Name = "Ling";
                    });
                    await Task.Delay(50);

                    res5 = GetFirstSkillName(js);
                    res5.Should().Be("Ling");

                    //Test Two Way
                    var stringName = Create(() => _WebView.Factory.CreateString("resName"));
                    SetAttribute(js, "Name", stringName);

                    await Task.Delay(150);
                    string resName = GetStringAttribute(js, "Name");
                    resName.Should().Be("resName");

                    await Task.Delay(150);

                    _DataContext.Name.Should().Be("resName");

                    DoSafeUI(() =>
                    {
                        _DataContext.Name = "nnnnvvvvvvv";
                    });

                    await Task.Delay(50);
                    res3 = GetStringAttribute(js, "Name");
                    ((string)res3).Should().Be("nnnnvvvvvvv");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Nested()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    string res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    string res2 = GetStringAttribute(js, "LastName");
                    res2.Should().Be("Desmaisons");

                    var local = GetAttribute(js, "Local");
                    string city = GetStringAttribute(local, "City");
                    city.Should().Be("Florianopolis");

                    DoSafeUI(() =>
                    {
                        _DataContext.Local.City = "Foz de Iguaçu";
                    });

                    await Task.Delay(100);
                    var local3 = GetAttribute(js, "Local");
                    string city3 = GetStringAttribute(local, "City");
                    city3.Should().Be("Foz de Iguaçu");

                    DoSafeUI(() =>
                    {
                        _DataContext.Local = new Local() { City = "Paris" };
                    });

                    await Task.Delay(100);
                    var local2 = GetAttribute(js, "Local");
                    string city2 = GetStringAttribute(local2, "City");
                    city2.Should().Be("Paris");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_to_Nested_Changes_After_Property_Updates_CSharp_Updates()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var local = new Local
                    {
                        City = "JJC"
                    };

                    DoSafeUI(() =>
                    {
                        _DataContext.Local = local;
                    });

                    await Task.Delay(100);

                    DoSafeUI(() =>
                    {
                        local.City = "Floripa";
                    });

                    await Task.Delay(100);

                    var js = mb.JsRootObject;

                    var jsLocal = GetAttribute(js, "Local");
                    string city = GetStringAttribute(jsLocal, "City");
                    city.Should().Be("Floripa");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_to_Nested_Changes_After_Property_Updates_Javascript_Updates()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var local = new Local
                    {
                        City = "JJC"
                    };

                    DoSafeUI(() =>
                    {
                        _DataContext.Local = local;
                    });

                    await Task.Delay(100);

                    var js = mb.JsRootObject;

                    var jsLocal = GetAttribute(js, "Local");

                    var stringName = Create(() => _WebView.Factory.CreateString("Floripa"));
                    SetAttribute(jsLocal, "City", stringName);

                    await Task.Delay(100);

                    _DataContext.Local.City.Should().Be("Floripa");
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Enum()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetAttribute(js, "PersonalState");
                    string dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                    dres.Should().Be("Married");

                    DoSafeUI(() =>
                    {
                        _DataContext.PersonalState = PersonalState.Single;
                    });
                    await Task.Delay(50);

                    res = GetAttribute(js, "PersonalState");
                    dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                    dres.Should().Be("Single");
                }
            };

            await RunAsync(test);

        }

        [Fact]
        public async Task TwoWay_Enum_Round_Trip()
        {
            _DataContext.Name = "toto";

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetAttribute(js, "PersonalState");
                    string dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                    dres.Should().Be("Married");

                    DoSafeUI(() =>
                    {
                        _DataContext.PersonalState = PersonalState.Single;
                    });
                    await Task.Delay(50);

                    res = GetAttribute(js, "PersonalState");
                    dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                    dres.Should().Be("Single");

                    var othervalue = GetCollectionAttribute(js, "States");

                    var di = othervalue.GetValue(2);
                    string name = di.GetValue("displayName").GetStringValue();
                    name.Should().Be("Divorced");

                    SetAttribute(js, "PersonalState", di);
                    await Task.Delay(100);

                    _DataContext.PersonalState.Should().Be(PersonalState.Divorced);
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Listens_to_property_update_during_property_changes_update_from_js()
        {
            var dataContext = new PropertyUpdatingTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetStringAttribute(js, "Property1");
                    res.Should().Be("1");

                    res = GetStringAttribute(js, "Property2");
                    res.Should().Be("2");

                    SetAttribute(js, "Property1", _WebView.Factory.CreateString("a"));

                    await Task.Delay(50);

                    res = GetStringAttribute(js, "Property1");
                    res.Should().Be("a");

                    res = GetStringAttribute(js, "Property2");
                    res.Should().Be("a", "Neutronium listen to object during update");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Listens_to_property_update_during_property_changes_update_from_Csharp()
        {
            var dataContext = new PropertyUpdatingTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetStringAttribute(js, "Property1");
                    res.Should().Be("1");

                    res = GetStringAttribute(js, "Property2");
                    res.Should().Be("2");

                    DoSafeUI(() =>
                    {
                        dataContext.Property1 = "a";
                    });

                    await Task.Delay(50);

                    res = GetStringAttribute(js, "Property1");
                    res.Should().Be("a");

                    res = GetStringAttribute(js, "Property2");
                    res.Should().Be("a", "Neutronium listen to object during update");
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

                    DoSafeUI(() => { dynamicDataContext.ValueInt = 110; });

                    await Task.Delay(50);

                    resInt = GetIntAttribute(js, "ValueInt");
                    resInt.Should().Be(110);
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

        private class SimplePerson : ViewModelBase
        {
            private PersonalState _PersonalState;
            public PersonalState PersonalState
            {
                get { return _PersonalState; }
                set { Set(ref _PersonalState, value, "PersonalState"); }
            }
        }

        [Fact]
        public async Task TwoWay_Enum_NotMapped()
        {
            var datacontext = new SimplePerson();
            datacontext.PersonalState = PersonalState.Single;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetAttribute(js, "PersonalState");
                    string dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                    dres.Should().Be("Single");


                    DoSafeUI(() =>
                    {
                        datacontext.PersonalState = PersonalState.Married;
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
            var datacontext = new Couple();
            var p1 = new Person() { Name = "David" };
            datacontext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            datacontext.Two = p2;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res1 = GetAttribute(js, "One");
                    string n1 = GetStringAttribute(res1, "Name");
                    n1.Should().Be("David");

                    var res2 = GetAttribute(js, "Two");
                    res2.Should().NotBeNull();
                    var n2 = GetStringAttribute(res2, "Name");
                    n2.Should().Be("Claudia");

                    var jsValue = GetAttribute(js, "Two");
                    SetAttribute(js, "One", jsValue);

                    await Task.Delay(100);

                    var res3 = GetAttribute(js, "One");
                    res3.Should().NotBeNull();
                    string n3 = GetStringAttribute(res3, "Name");
                    n3.Should().Be("Claudia");

                    await Task.Delay(100);

                    datacontext.One.Should().Be(p2);

                    var res4 = GetAttribute(res3, "ChildrenNumber");
                    res4.IsNull.Should().BeTrue();

                    var five = Create(() => _WebView.Factory.CreateInt(5));
                    SetAttribute(res3, "ChildrenNumber", five);
                    await Task.Delay(100);

                    datacontext.One.ChildrenNumber.Should().Be(5);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Set_Null_From_Javascript()
        {
            var datacontext = new Couple();
            var p1 = new Person() { Name = "David" };
            datacontext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            datacontext.Two = p2;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res1 = GetAttribute(js, "One");
                    res1.Should().NotBeNull();
                    string n1 = GetStringAttribute(res1, "Name");
                    n1.Should().Be("David");

                    var res2 = GetAttribute(js, "Two");
                    res2.Should().NotBeNull();
                    string n2 = GetStringAttribute(res2, "Name");
                    n2.Should().Be("Claudia");

                    var nullSO = Create(() => _WebView.Factory.CreateNull());
                    SetAttribute(js, "One", nullSO);

                    await Task.Delay(100);
                    var res3 = GetAttribute(js, "One");
                    //GetSafe(() => res3.IsNull).Should().BeTrue();
                    //Init case of awesomium an object is used on JS side
                    //Todo: create specific test

                    await Task.Delay(100);

                    datacontext.One.Should().BeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Set_Object_From_Javascipt_Survive_MissUse()
        {
            var datacontext = new Couple();
            var p1 = new Person() { Name = "David" };
            datacontext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            datacontext.Two = p2;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var res1 = GetAttribute(js, "One");
                    res1.Should().NotBeNull();
                    string n1 = GetStringAttribute(res1, "Name");
                    n1.Should().Be("David");

                    var res2 = GetAttribute(js, "Two");
                    res2.Should().NotBeNull();
                    string n2 = GetStringAttribute(res2, "Name");
                    n2.Should().Be("Claudia");

                    var StringJS = _WebView.Factory.CreateString("Dede");
                    SetAttribute(js, "One", StringJS);

                    string res3 = GetStringAttribute(js, "One");
                    res3.Should().Be("Dede");

                    await Task.Delay(100);

                    datacontext.One.Should().Be(p1);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Set_Object_From_Javascipt_Survive_MissUse_NoReset_OnAttribute()
        {
            var datacontext = new Couple();
            var p1 = new Person() { Name = "David" };
            datacontext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            datacontext.Two = p2;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var res1 = GetAttribute(js, "One");
                    res1.Should().NotBeNull();
                    string n1 = GetStringAttribute(res1, "Name");
                    n1.Should().Be("David");

                    var res2 = GetAttribute(js, "Two");
                    res2.Should().NotBeNull();
                    string n2 = GetStringAttribute(res2, "Name");
                    n2.Should().Be("Claudia");

                    var trueJs = _WebView.Factory.CreateObject(false);
                    SetAttribute(js, "One", trueJs);

                    var res3 = GetAttribute(js, "One");
                    res3.IsObject.Should().BeTrue();

                    await Task.Delay(100);
                    datacontext.One.Should().Be(p1);
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task Property_Test()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new FakeTestViewModel() { Command = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    string res = GetStringAttribute(js, "Name");
                    res.Should().Be("NameTest");

                    var stringJS = _WebView.Factory.CreateString("NewName");
                    SetAttribute(js, "Name", stringJS);
                    res = GetStringAttribute(js, "Name");
                    res.Should().Be("NewName");

                    await Task.Delay(100);
                    datacontexttest.Name.Should().Be("NameTest");

                    bool resf = GetSafe(() => js.HasValue("UselessName"));
                    resf.Should().BeFalse();

                    Action Safe = () => datacontexttest.InconsistentEventEmit();

                    Safe.ShouldNotThrow("Inconsistent Name in property should not throw exception");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Basic()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new FakeTestViewModel() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = ((HtmlBinding)mb).JsBrideRootObject as JsGenericObject;

                    var mycommand = js.GetAttribute("Command") as JsCommand;
                    mycommand.Should().NotBeNull();
                    mycommand.ToString().Should().Be("{}");
                    mycommand.Type.Should().Be(JsCsGlueType.Command);
                    mycommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new FakeTestViewModel() { Command = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(100);
                    command.Received().Execute(Arg.Any<object>());
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Command_With_Parameter()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new FakeTestViewModel() { Command = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute", js));
                    await Task.Delay(100);
                    command.Received().Execute(datacontexttest);
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TwoWay_Command_CanExecute_Set_CanExecuteValue(bool canExecute)
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(canExecute);
            var datacontexttest = new FakeTestViewModel() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");

                    res.Should().Be(canExecute);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Uptate_From_Null()
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(true);
            var datacontexttest = new FakeTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    mycommand.IsNull.Should().BeTrue();

                    DoSafeUI(() => datacontexttest.Command = command);
                    await Task.Delay(200);

                    mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute", js));
                    await Task.Delay(200);
                    command.Received().Execute(datacontexttest);
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandWithoutParameter_Is_Mapped()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            var datacontexttest = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = ((HtmlBinding)mb).JsBrideRootObject as JsGenericObject;

                    var mycommand = js.GetAttribute("CommandWithoutParameters") as JsCommandWithoutParameter;
                    mycommand.Should().NotBeNull();
                    mycommand.ToString().Should().Be("{}");
                    mycommand.Type.Should().Be(JsCsGlueType.Command);
                    mycommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TwoWay_CommandWithoutParameter_CanExecuteValue_has_CanExecute_value(bool canExecute)
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            command.CanExecute.Returns(canExecute);
            var datacontexttest = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandWithoutParameters");

                    var res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().Be(canExecute);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandWithoutParameter_CanExecute_Refresh_Ok()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            command.CanExecute.Returns(true);
            var datacontexttest = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContextAsync()
            {
                Path = TestContext.GenericBind,
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CommandWithoutParameters");
                    var res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    command.CanExecute.Returns(false);
                    command.CanExecuteChanged += Raise.EventWith(_ICommand, new EventArgs());

                    await Task.Delay(150);

                    mycommand = GetAttribute(js, "CommandWithoutParameters");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandWithoutParameter_Can_Be_Called()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            var datacontexttest = new FakeTestViewModel() { CommandWithoutParameters = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandWithoutParameters");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(100);
                    command.Received(1).Execute();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandWithoutParameter_Uptate_From_Null()
        {
            var command = Substitute.For<ICommandWithoutParameter>();
            var datacontexttest = new FakeTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    mycommand.IsNull.Should().BeTrue();

                    DoSafeUI(() => datacontexttest.CommandWithoutParameters = command);
                    await Task.Delay(200);

                    mycommand = GetAttribute(js, "CommandWithoutParameters");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(200);
                    command.Received().Execute();
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandGeneric_Is_Mapped()
        {
            var command = Substitute.For<ICommand<string>>();
            var datacontexttest = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = ((HtmlBinding)mb).JsBrideRootObject as JsGenericObject;

                    var mycommand = js.GetAttribute("CommandGeneric") as JsCommand<string>;
                    mycommand.Should().NotBeNull();
                    mycommand.ToString().Should().Be("{}");
                    mycommand.Type.Should().Be(JsCsGlueType.Command);
                    mycommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TwoWay_CommandGeneric_CanExecuteValue_default_value_is_true(bool canExecute)
        {
            var command = Substitute.For<ICommand<string>>();
            command.CanExecute(Arg.Any<string>()).Returns(canExecute);
            var datacontexttest = new FakeTestViewModel() { CommandGenericNotBound = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandGenericNotBound");

                    var res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData("parameter")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("abc")]
        public async Task TwoWay_CommandGeneric_Can_Be_Called_With_Parameter(string parameter)
        {
            var command = Substitute.For<ICommand<string>>();
            var datacontexttest = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandGeneric");
                    var jsParameter = (parameter != null) ? _WebView.Factory.CreateString(parameter) : _WebView.Factory.CreateNull();
                    DoSafe(() => Call(mycommand, "Execute", jsParameter));
                    await Task.Delay(150);
                    command.Received(1).Execute(parameter);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandGeneric_Can_Convert_Argument_When_Argument_Mismatch()
        {
            var command = Substitute.For<ICommand<string>>();
            var datacontexttest = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandGeneric");
                    DoSafe(() => Call(mycommand, "Execute", _WebView.Factory.CreateInt(10)));
                    await Task.Delay(100);
                    command.Received(1).Execute("10");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CommandGeneric_Do_Not_Call_Execute_When_Argument_Can_Not_Be_Converted()
        {
            var command = Substitute.For<ICommand<int>>();
            var datacontexttest = new FakeTestViewModel() { CommandGenericInt = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandGenericInt");
                    DoSafe(() => Call(mycommand, "Execute", _WebView.Factory.CreateString("taDa")));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<int>());
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_CommandGeneric_Do_Not_Call_Execute_When_Called_Without_Argument()
        {
            var command = Substitute.For<ICommand<string>>();
            var datacontexttest = new FakeTestViewModel() { CommandGeneric = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CommandGeneric");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<string>());
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Command_Generic_Uptate_From_Null()
        {
            var command = Substitute.For<ICommand<string>>();
            var datacontexttest = new FakeTestViewModel();
            var argument = "argument";

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    mycommand.IsNull.Should().BeTrue();

                    DoSafeUI(() => datacontexttest.CommandGeneric = command);
                    await Task.Delay(200);

                    mycommand = GetAttribute(js, "CommandGeneric");
                    DoSafe(() => Call(mycommand, "Execute", _WebView.Factory.CreateString(argument)));
                    await Task.Delay(200);
                    command.Received().Execute(argument);
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Generic_FakeTestViewModel_CanExecute_Refresh_Ok()
        {
            var command = Substitute.For<ICommand<FakeTestViewModel>>();
            command.CanExecute(Arg.Any<FakeTestViewModel>()).Returns(true);
            var datacontexttest = new FakeTestViewModel() { AutoCommand = command };

            var test = new TestInContextAsync()
            {
                Path = TestContext.GenericBind,
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "AutoCommand");
                    var res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    command.CanExecute(Arg.Any<FakeTestViewModel>()).Returns(false);
                    command.CanExecuteChanged += Raise.EventWith(_ICommand, new EventArgs());

                    await Task.Delay(100);

                    mycommand = GetAttribute(js, "AutoCommand");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        #region SimpleCommand

        [Fact]
        public async Task TwoWay_SimpleCommand_Without_Parameter()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandNoArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommandNoArgument");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(100);
                    command.Received().Execute();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_T_With_Parameter()
        {
            var command = Substitute.For<ISimpleCommand<SimpleCommandTestViewModel>>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandObjectArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommandObjectArgument");
                    DoSafe(() => Call(mycommand, "Execute", js));
                    await Task.Delay(100);
                    command.Received().Execute(datacontexttest);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_T_With_Parameter_Convert_Number_Type()
        {
            var command = Substitute.For<ISimpleCommand<decimal>>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandDecimalArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommandDecimalArgument");
                    DoSafe(() => Call(mycommand, "Execute", _WebView.Factory.CreateDouble(0.55)));
                    await Task.Delay(100);
                    command.Received().Execute(0.55m);
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_SimpleCommand_T_With_Parameter_Does_Not_Throw_On_Type_Mismatch()
        {
            var command = Substitute.For<ISimpleCommand<decimal>>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandDecimalArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommandDecimalArgument");
                    DoSafe(() => Call(mycommand, "Execute", _WebView.Factory.CreateString("u")));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<decimal>());
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_T_Without_Parameter_Does_not_Throw()
        {
            var command = Substitute.For<ISimpleCommand<SimpleCommandTestViewModel>>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandObjectArgument = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommandObjectArgument");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(100);
                    command.DidNotReceive().Execute(Arg.Any<SimpleCommandTestViewModel>());
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_T_Name()
        {
            var command = Substitute.For<ISimpleCommand<SimpleCommandTestViewModel>>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandObjectArgument = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;

                    var mysimplecommand = js.GetAttribute("SimpleCommandObjectArgument") as JsSimpleCommand<SimpleCommandTestViewModel>;
                    mysimplecommand.Should().NotBeNull();
                    mysimplecommand.ToString().Should().Be("{}");
                    mysimplecommand.Type.Should().Be(JsCsGlueType.SimpleCommand);
                    mysimplecommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_Name()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontexttest = new SimpleCommandTestViewModel() { SimpleCommandNoArgument = command };

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;

                    var mysimplecommand = js.GetAttribute("SimpleCommandNoArgument") as JsSimpleCommand;
                    mysimplecommand.Should().NotBeNull();
                    mysimplecommand.ToString().Should().Be("{}");
                    mysimplecommand.Type.Should().Be(JsCsGlueType.SimpleCommand);
                    mysimplecommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        #endregion

        private void CheckIntValue(IJavascriptObject js, string pn, int value)
        {
            IJavascriptObject res = GetAttribute(js, pn);
            res.Should().NotBeNull();
            res.IsNumber.Should().BeTrue();
            res.GetIntValue().Should().Be(0);
        }


        [Fact]
        public async Task TwoWay_CLR_Type_FromCtojavascript()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontext = new ClrTypesTestViewModel();

            var test = new TestInContext()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
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
        public async Task TwoWay_CLR_Type_From_CSharp()
        {
            var datacontext = new ClrTypesTestViewModel
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
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;
                    js.Should().NotBeNull();

                    var res = GetIntAttribute(js, "Int64");
                    res.Should().Be((int)datacontext.Int64);

                    res = GetIntAttribute(js, "Uint64");
                    res.Should().Be((int)datacontext.Uint64);

                    res = GetIntAttribute(js, "Int32");
                    res.Should().Be(datacontext.Int32);

                    res = GetIntAttribute(js, "Uint32");
                    res.Should().Be((int)datacontext.Uint32);

                    res = GetIntAttribute(js, "Int16");
                    res.Should().Be(datacontext.Int16);

                    res = GetIntAttribute(js, "Uint16");
                    res.Should().Be(datacontext.Uint16);

                    var res2 = GetDoubleAttribute(js, "Float");
                    res2.Should().Be(datacontext.Float);

                    res2 = GetDoubleAttribute(js, "Double");
                    res2.Should().Be(datacontext.Double);

                    res2 = GetDoubleAttribute(js, "Decimal");
                    res2.Should().Be((double)datacontext.Decimal);

                    res = GetIntAttribute(js, "Byte");
                    res.Should().Be(datacontext.Byte);

                    res = GetIntAttribute(js, "Sbyte");
                    res.Should().Be(datacontext.Sbyte);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CLR_Type_FromjavascripttoCto()
        {
            var datacontext = new ClrTypesTestViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    js.Should().NotBeNull();

                    SetAttribute(js, "Int64", _WebView.Factory.CreateInt(32));
                    await Task.Delay(200);
                    datacontext.Int64.Should().Be(32);

                    SetAttribute(js, "Uint64", _WebView.Factory.CreateInt(456));
                    await Task.Delay(200);
                    datacontext.Uint64.Should().Be(456);

                    SetAttribute(js, "Int32", _WebView.Factory.CreateInt(5));
                    await Task.Delay(200);
                    datacontext.Int32.Should().Be(5);

                    SetAttribute(js, "Uint32", _WebView.Factory.CreateInt(67));
                    await Task.Delay(200);
                    datacontext.Uint32.Should().Be(67);

                    SetAttribute(js, "Int16", _WebView.Factory.CreateInt(-23));
                    await Task.Delay(200);
                    datacontext.Int16.Should().Be(-23);

                    SetAttribute(js, "Uint16", _WebView.Factory.CreateInt(9));
                    await Task.Delay(200);
                    datacontext.Uint16.Should().Be(9);

                    SetAttribute(js, "Float", _WebView.Factory.CreateDouble(888.78));
                    await Task.Delay(200);
                    datacontext.Float.Should().Be(888.78f);

                    SetAttribute(js, "Double", _WebView.Factory.CreateDouble(866.76));
                    await Task.Delay(200);
                    datacontext.Double.Should().Be(866.76);

                    SetAttribute(js, "Decimal", _WebView.Factory.CreateDouble(0.5));
                    await Task.Delay(200);
                    datacontext.Decimal.Should().Be(0.5m);

                    SetAttribute(js, "Byte", _WebView.Factory.CreateInt(10));
                    await Task.Delay(200);
                    datacontext.Byte.Should().Be(10);

                    SetAttribute(js, "Sbyte", _WebView.Factory.CreateInt(-12));
                    await Task.Delay(200);
                    datacontext.Sbyte.Should().Be(-12);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_CanExecute_Refresh_Ok()
        {
            bool canexecute = true;
            _ICommand.CanExecute(Arg.Any<object>()).ReturnsForAnyArgs(x => canexecute);


            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "TestCommand");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    canexecute = false;
                    _ICommand.CanExecuteChanged += Raise.EventWith(_ICommand, new EventArgs());

                    await Task.Delay(100);

                    mycommand = GetAttribute(js, "TestCommand");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    ((bool)res).Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_CanExecute_Refresh_Ok_Argument()
        {
            bool canexecute = true;
            _ICommand.CanExecute(Arg.Any<object>()).ReturnsForAnyArgs(x => canexecute);

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    await Task.Delay(100);

                    var mycommand = GetAttribute(js, "TestCommand");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    _ICommand.Received().CanExecute(_DataContext);

                    canexecute = false;
                    _ICommand.ClearReceivedCalls();

                    _ICommand.CanExecuteChanged += Raise.EventWith(_ICommand, new EventArgs());

                    await Task.Delay(100);

                    _ICommand.Received().CanExecute(_DataContext);

                    mycommand = GetAttribute(js, "TestCommand");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_CanExecute_Refresh_Ok_Argument_Exception()
        {
            _ICommand.CanExecute(Arg.Any<object>()).Returns(x => { if (x[0] == null) throw new Exception(); return false; });

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    _ICommand.Received().CanExecute(Arg.Any<object>());
                    var js = mb.JsRootObject;

                    await Task.Delay(100);

                    var mycommand = GetAttribute(js, "TestCommand");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();

                    _ICommand.Received().CanExecute(_DataContext);
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Command_Received_javascript_variable()
        {
            _ICommand.CanExecute(Arg.Any<object>()).Returns(true);

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "TestCommand");
                    Call(mycommand, "Execute", _WebView.Factory.CreateString("titi"));

                    await Task.Delay(150);
                    _ICommand.Received().Execute("titi");
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_Command_Complete()
        {
            _ICommand = new RelaySimpleCommand(() =>
                {
                    _DataContext.MainSkill = new Skill();
                    _DataContext.Skills.Add(_DataContext.MainSkill);
                });

            _DataContext.TestCommand = _ICommand;
            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    _DataContext.Skills.Should().HaveCount(2);

                    DoSafeUI(() =>
                    {
                        _ICommand.Execute(null);
                    });

                    await Task.Delay(150);

                    var res = GetCollectionAttribute(js, "Skills");

                    res.Should().NotBeNull();
                    res.GetArrayLength().Should().Be(3);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_With_Null_Parameter()
        {
            var command = Substitute.For<ICommand>();
            var test = new FakeTestViewModel() { Command = command };

            var testR = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, test, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    Call(mycommand, "Execute", _WebView.Factory.CreateNull());

                    await Task.Delay(150);
                    command.Received().Execute(null);
                }
            };

            await RunAsync(testR);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Received_javascript_variable_and_not_crash_withoutcallback()
        {
            var function = NSubstitute.Substitute.For<Func<int, int>>();
            var dc = new FakeFactory<int, int>(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CreateObject");
                    var intValue = _WebView.Factory.CreateInt(25);
                    Call(mycommand, "Execute", intValue);

                    await Task.Delay(700);
                    function.Received(1).Invoke(25);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Received_javascript_variable()
        {
            var function = NSubstitute.Substitute.For<Func<int, int>>();
            function.Invoke(Arg.Any<int>()).Returns(255);
            var dc = new FakeFactory<int, int>(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {

                    {
                        var glueobj = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;
                        var jsResultCommand = glueobj.GetAttribute("CreateObject") as JsResultCommand<int, int>;
                        jsResultCommand.Should().NotBeNull();
                        jsResultCommand.ToString().Should().Be("{}");
                        jsResultCommand.Type.Should().Be(JsCsGlueType.ResultCommand);
                        jsResultCommand.CachableJsValue.Should().NotBeNull();
                    }

                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CreateObject");

                    var cb = GetCallBackObject();

                    var intValue = _WebView.Factory.CreateInt(25);
                    var resdummy = this.CallWithRes(mycommand, "Execute", intValue, cb);

                    await Task.Delay(100);

                    DoSafeUI(() => function.Received(1).Invoke(25));

                    await Task.Yield();

                    var error = _WebView.GetGlobal().GetValue("err");
                    error.IsUndefined.Should().BeTrue();

                    var resvalue = _WebView.GetGlobal().GetValue("res");
                    int intres = resvalue.GetIntValue();
                    intres.Should().Be(255);
                }
            };

            await RunAsync(test);
        }

        private IJavascriptObject GetCallBackObject()
        {
            IJavascriptObject cb = null;
            bool res = _WebView.Eval("(function(){return { fullfill: function (res) {window.res=res; }, reject: function(err){window.err=err;}}; })();", out cb);

            res.Should().BeTrue();
            cb.Should().NotBeNull();
            cb.IsObject.Should().BeTrue();
            return cb;
        }

        [Fact]
        public async Task TwoWay_ResultCommand_can_be_listened_from_Javascript()
        {
            const string original = "original";
            const string stringExpected = "NewName";
            var result = new SimpleViewModel { Name = original };
            var function = NSubstitute.Substitute.For<Func<int, SimpleViewModel>>();
            function.Invoke(Arg.Any<int>()).Returns(result);

            var dc = new FakeFactory<int, SimpleViewModel>(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CreateObject");
                    var cb = GetCallBackObject();

                    var resdummy = this.CallWithRes(mycommand, "Execute", _WebView.Factory.CreateInt(25), cb);

                    await Task.Delay(700);

                    var resvalue = _WebView.GetGlobal().GetValue("res");

                    await Task.Delay(100);

                    var originalValue = GetAttribute(resvalue, nameof(SimpleViewModel.Name)).GetStringValue();

                    originalValue.Should().Be(original);

                    DoSafeUI(() => result.Name = stringExpected);

                    await Task.Delay(100);

                    var newValue = GetAttribute(resvalue, nameof(SimpleViewModel.Name)).GetStringValue();
                    newValue.Should().Be(stringExpected);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Without_Argument_Is_Correctly_Mapped()
        {
            var function = Substitute.For<Func<string>>();
            var dc = new FakeFactory<string>(function);

            var test = new TestInContext()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var glueobj = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;
                    var jsResultCommand = glueobj.GetAttribute("CreateObject") as JsResultCommand<string>;
                    jsResultCommand.Should().NotBeNull();
                    jsResultCommand.ToString().Should().Be("{}");
                    jsResultCommand.Type.Should().Be(JsCsGlueType.ResultCommand);
                    jsResultCommand.CachableJsValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Without_Argument_returns_result()
        {
            var result = "resultString";
            var function = Substitute.For<Func<string>>();
            function.Invoke().Returns(result);

            var dc = new FakeFactory<string>(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var mycommand = GetAttribute(js, "CreateObject");
                    var cb = GetCallBackObject();

                    var resdummy = this.CallWithRes(mycommand, "Execute", cb);

                    await Task.Delay(700);

                    var resvalue = _WebView.GetGlobal().GetValue("res");

                    await Task.Delay(100);

                    var originalValue = resvalue.GetStringValue();

                    originalValue.Should().Be(result);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_can_be_listened_from_CSharp_when_used_in_Vm()
        {
            var child = new BasicTestViewModel();

            var function = NSubstitute.Substitute.For<Func<int, BasicTestViewModel>>();
            function.Invoke(Arg.Any<int>()).Returns(child);

            var dataContext = new FactoryFatherTestViewModel(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var factory = GetAttribute(js, "Factory");
                    var mycommand = GetAttribute(factory, "CreateObject");
                    var cb = GetCallBackObject();

                    Call(mycommand, "Execute", _WebView.Factory.CreateInt(25), cb);

                    await Task.Delay(700);

                    var resvalue = _WebView.GetGlobal().GetValue("res");

                    await Task.Delay(100);

                    var originalValue = GetAttribute(resvalue, nameof(BasicTestViewModel.Value)).GetIntValue();

                    originalValue.Should().Be(-1);

                    SetAttribute(js, nameof(FactoryFatherTestViewModel.Child), resvalue);

                    var res = GetAttribute(js, nameof(FactoryFatherTestViewModel.Child));
                    res.IsObject.Should().BeTrue();

                    await Task.Delay(200);

                    DoSafeUI(() => dataContext.Child.Should().Be(child));

                    var newInt = 45;
                    SetAttribute(resvalue, nameof(BasicTestViewModel.Value), _WebView.Factory.CreateInt(newInt));
                    var updatedValue = GetAttribute(resvalue, nameof(BasicTestViewModel.Value)).GetIntValue();
                    updatedValue.Should().Be(newInt);
                    await Task.Delay(200);
                    DoSafeUI(() => dataContext.Child.Value.Should().Be(newInt));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_ResultCommand_Received_javascript_variable_should_fault_Onexception()
        {
            string errormessage = "original error message";
            var function = NSubstitute.Substitute.For<Func<int, int>>();
            function.When(f => f.Invoke(Arg.Any<int>())).Do(_ => { throw new Exception(errormessage); });
            var dc = new FakeFactory<int, int>(function);

            var test = new TestInContextAsync()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HtmlBinding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var mycommand = GetAttribute(js, "CreateObject");
                    IJavascriptObject cb = null;
                    bool res = _WebView.Eval("(function(){return { fullfill: function (res) {window.res=res; }, reject: function(err){window.err=err;}}; })();", out cb);

                    res.Should().BeTrue();
                    cb.Should().NotBeNull();
                    cb.IsObject.Should().BeTrue();

                    var resdummy = this.CallWithRes(mycommand, "Execute", _WebView.Factory.CreateInt(25), cb);
                    await Task.Delay(100);
                    DoSafeUI(() => function.Received(1).Invoke(25));

                    await Task.Yield();

                    var error = _WebView.GetGlobal().GetValue("err").GetStringValue();
                    error.Should().Be(errormessage);

                    var resvalue = _WebView.GetGlobal().GetValue("res");
                    resvalue.IsUndefined.Should().BeTrue();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection()
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

                    Check(col, _DataContext.Skills);

                    DoSafeUI(() =>
                    {
                        _DataContext.Skills.Add(new Skill() { Name = "C++", Type = "Info" });
                    });

                    await Task.Delay(1000);
                    col = GetCollectionAttribute(js, "Skills");
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    DoSafeUI(() =>
                    {
                        _DataContext.Skills.Insert(0, new Skill() { Name = "C#", Type = "Info" });
                    });
                    await Task.Delay(100);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    DoSafeUI(() =>
                    {
                        _DataContext.Skills.RemoveAt(1);
                    });
                    await Task.Delay(100);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    DoSafeUI(() =>
                    {
                        _DataContext.Skills[0] = new Skill() { Name = "HTML", Type = "Info" };
                    });
                    await Task.Delay(100);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    DoSafeUI(() =>
                    {
                        _DataContext.Skills[0] = new Skill() { Name = "HTML5", Type = "Info" };
                        _DataContext.Skills.Insert(0, new Skill() { Name = "HTML5", Type = "Info" });
                    });
                    await Task.Delay(100);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    DoSafeUI(() =>
                    {
                        _DataContext.Skills.Clear();
                    });
                    await Task.Delay(100);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_FromJSUpdate()
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

                    Check(col, _DataContext.Skills);

                    var coll = GetAttribute(js, "Skills");
                    Call(coll, "push", (root.GetAttribute("Skills") as JsArray).Items[0].GetJsSessionValue());

                    await Task.Delay(5000);
                    _DataContext.Skills.Should().HaveCount(3);
                    _DataContext.Skills[2].Should().Be(_DataContext.Skills[0]);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    Check(col, _DataContext.Skills);

                    Call(coll, "pop");

                    await Task.Delay(100);
                    _DataContext.Skills.Should().HaveCount(2);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    Call(coll, "shift");

                    await Task.Delay(100);
                    _DataContext.Skills.Should().HaveCount(1);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    Check(col, _DataContext.Skills);


                    Call(coll, "unshift",
                          (root.GetAttribute("Skills") as JsArray).Items[0].GetJsSessionValue());

                    await Task.Delay(150);
                    _DataContext.Skills.Should().HaveCount(2);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    Check(col, _DataContext.Skills);

                    DoSafeUI(() =>
                    {
                        _DataContext.Skills.Add(new Skill() { Type = "Langage", Name = "French" });
                    });
                    await Task.Delay(150);
                    _DataContext.Skills.Should().HaveCount(3);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    Call(coll, "reverse");

                    await Task.Delay(150);
                    _DataContext.Skills.Should().HaveCount(3);
                    col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    Check(col, _DataContext.Skills);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_JSUpdate_Should_Survive_ViewChanges()
        {
            var test = new TestInContextAsync()
            {
                Path = TestContext.Simple,
                Bind = (win) => HtmlBinding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var root = (mb as HtmlBinding).JsBrideRootObject as JsGenericObject;
                    var js = mb.JsRootObject;

                    var col = GetCollectionAttribute(js, "Skills");
                    col.GetArrayLength().Should().Be(2);

                    Check(col, _DataContext.Skills);

                    var coll = GetAttribute(js, "Skills");
                    Call(coll, "push", _WebView.Factory.CreateString("Whatever"));

                    await Task.Delay(150);
                    _DataContext.Skills.Should().HaveCount(2);
                }
            };

            await RunAsync(test);
        }


        private class VMWithList<T> : ViewModelTestBase
        {
            public VMWithList()
            {
                List = new ObservableCollection<T>();
            }
            public ObservableCollection<T> List { get; }
        }

        private class VMWithListNonGeneric : ViewModelTestBase
        {
            public VMWithListNonGeneric()
            {
                List = new ArrayList();
            }
            public ArrayList List { get; }
        }

        private class VMwithdecimal : ViewModelTestBase
        {
            public VMwithdecimal()
            {
            }

            private decimal _DecimalValue;
            public decimal decimalValue
            {
                get { return _DecimalValue; }
                set { Set(ref _DecimalValue, value); }
            }
        }


        private void Checkstring(IJavascriptObject coll, IList<string> iskill)
        {
            var javaCollection = Enumerable.Range(0, coll.GetArrayLength()).Select(i => coll.GetValue(i).GetStringValue());
            javaCollection.Should().Equal(iskill);
        }

        private void Checkdecimal(IJavascriptObject coll, IList<decimal> iskill)
        {
            coll.GetArrayLength().Should().Be(iskill.Count);

            for (int i = 0; i < iskill.Count; i++)
            {
                var c = (decimal)coll.GetValue(i).GetDoubleValue();
                c.Should().Be(iskill[i]);
            }
        }


        [Fact]
        public async Task TwoWay_Decimal_ShouldOK()
        {
            var datacontext = new VMwithdecimal();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    int res = GetIntAttribute(js, "decimalValue");
                    res.Should().Be(0);

                    //Call(js, "decimalValue", _WebView.Factory.CreateDouble(0.5));
                    var halfJavascript = Create(() => _WebView.Factory.CreateDouble(0.5));
                    SetAttribute(js, "decimalValue", halfJavascript);
                    await Task.Delay(200);

                    datacontext.decimalValue.Should().Be(0.5m);

                    double doublev = GetDoubleAttribute(js, "decimalValue");
                    double half = 0.5;
                    doublev.Should().Be(half);
                }
            };

            await RunAsync(test);
        }

        private class VMwithlong : ViewModelTestBase
        {
            private long _LongValue;
            public long longValue
            {
                get { return _LongValue; }
                set { Set(ref _LongValue, value); }
            }
        }

        [Fact]
        public async Task TwoWay_Long_ShouldOK()
        {
            var datacontext = new VMwithlong() { longValue = 45 };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var doublev = GetDoubleAttribute(js, "longValue");
                    doublev.Should().Be(45);

                    var intValue = 24524;
                    var jsInt = Create(() => _WebView.Factory.CreateInt(intValue));
                    SetAttribute(js, "longValue", jsInt);
                    await Task.Delay(100);

                    datacontext.longValue.Should().Be(24524);

                    doublev = GetDoubleAttribute(js, "longValue");
                    long half = 24524;
                    doublev.Should().Be(half);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_string()
        {
            var datacontext = new VMWithList<string>();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var col = GetSafe(() => GetCollectionAttribute(js, "List"));
                    col.GetArrayLength().Should().Be(0);
                    await Task.Delay(200);
                    Checkstring(col, datacontext.List);

                    DoSafeUI(() =>
                    {
                        datacontext.List.Add("titi");
                    });
                    await Task.Delay(200);
                    col = GetSafe(() => GetCollectionAttribute(js, "List"));
                    Checkstring(col, datacontext.List);

                    DoSafeUI(() =>
                    {
                        datacontext.List.Add("kiki");
                        datacontext.List.Add("toto");
                    });
                    await Task.Delay(100);
                    col = GetSafe(() => GetCollectionAttribute(js, "List"));
                    Checkstring(col, datacontext.List);

                    DoSafeUI(() =>
                    {
                        datacontext.List.Move(0, 2);
                    });
                    await Task.Delay(100);
                    col = GetSafe(() => GetCollectionAttribute(js, "List"));
                    Checkstring(col, datacontext.List);

                    DoSafeUI(() =>
                    {
                        datacontext.List.Move(2, 1);
                    });
                    await Task.Delay(100);
                    col = GetSafe(() => GetCollectionAttribute(js, "List"));
                    Checkstring(col, datacontext.List);

                    var comp = new List<string>(datacontext.List) { "newvalue" };

                    col = GetSafe(() => GetCollectionAttribute(js, "List"));
                    var chcol = GetAttribute(js, "List");
                    Call(chcol, "push", _WebView.Factory.CreateString("newvalue"));

                    await Task.Delay(350);

                    col = GetSafe(() => GetCollectionAttribute(js, "List"));

                    datacontext.List.Should().Equal(comp);
                    Checkstring(col, datacontext.List);

                    DoSafeUI(() =>
                    {
                        datacontext.List.Clear();
                    });
                    await Task.Delay(100);
                    col = GetSafe(() => GetCollectionAttribute(js, "List"));

                    Checkstring(col, datacontext.List);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_should_be_observable_attribute()
        {
            var datacontext = new ChangingCollectionViewModel();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var col = GetSafe(() => GetCollectionAttribute(js, "Items"));
                    col.GetArrayLength().Should().NotBe(0);

                    DoSafeUI(() => datacontext.Replace.Execute(null));

                    datacontext.Items.Should().BeEmpty();

                    await Task.Delay(300);
                    col = GetSafe(() => GetCollectionAttribute(js, "Items"));
                    col.GetArrayLength().Should().Be(0);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_NoneGenericList()
        {
            var datacontext = new VMWithListNonGeneric();
            datacontext.List.Add(888);

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var col = GetSafe(() => GetCollectionAttribute(js, "List"));
                    col.GetArrayLength().Should().Be(1);

                    var res = GetAttribute(js, "List");
                    Call(res, "push", _WebView.Factory.CreateString("newvalue"));

                    col = GetSafe(() => GetCollectionAttribute(js, "List"));
                    col.GetArrayLength().Should().Be(2);

                    await Task.Delay(350);

                    datacontext.List.Should().HaveCount(2);
                    datacontext.List[1].Should().Be("newvalue");
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Collection_decimal()
        {
            var datacontext = new VMWithList<decimal>();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var col = GetSafe(() => GetCollectionAttribute(js, "List"));
                    col.GetArrayLength().Should().Be(0);

                    Checkdecimal(col, datacontext.List);

                    DoSafeUI(() =>
                    {
                        datacontext.List.Add(3);
                    });

                    await Task.Delay(150);
                    col = GetSafe(() => GetCollectionAttribute(js, "List"));

                    Checkdecimal(col, datacontext.List);

                    DoSafeUI(() =>
                    {
                        datacontext.List.Add(10.5m);
                        datacontext.List.Add(126);
                    });

                    await Task.Delay(150);
                    col = GetSafe(() => GetCollectionAttribute(js, "List"));

                    Checkdecimal(col, datacontext.List);

                    await Task.Delay(100);
                    col = GetSafe(() => GetCollectionAttribute(js, "List"));

                    Checkdecimal(col, datacontext.List);

                    var comp = new List<decimal>(datacontext.List) { 0.55m };

                    var res = GetAttribute(js, "List");
                    Call(res, "push", _WebView.Factory.CreateDouble(0.55));

                    await Task.Delay(500);

                    col = GetSafe(() => GetCollectionAttribute(js, "List"));

                    comp.Should().Equal(datacontext.List);
                    Checkdecimal(col, datacontext.List);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task StringBinding()
        {
            var datacontext = new VMWithList<decimal>();

            var test = new TestInContext()
            {
                Bind = (win) => Neutronium.Core.StringBinding.Bind(win, "{\"LastName\":\"Desmaisons\",\"Name\":\"O Monstro\",\"BirthDay\":\"0001-01-01T00:00:00.000Z\",\"PersonalState\":\"Married\",\"Age\":0,\"Local\":{\"City\":\"Florianopolis\",\"Region\":\"SC\"},\"MainSkill\":{},\"States\":[\"Single\",\"Married\",\"Divorced\"],\"Skills\":[{\"Type\":\"French\",\"Name\":\"Langage\"},{\"Type\":\"C++\",\"Name\":\"Info\"}]}"),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;

                    mb.Root.Should().BeNull();

                    mb.Context.Should().NotBeNull();

                    string res = GetStringAttribute(js, "Name");
                    res.Should().Be("O Monstro");

                    string res2 = GetStringAttribute(js, "LastName");
                    res2.Should().Be("Desmaisons");

                    string res4 = GetLocalCity(js);
                    res4.Should().Be("Florianopolis");

                    var res5 = GetFirstSkillName(js);
                    res5.Should().Be("Langage");
                }
            };
            await RunAsync(test);
        }

        [Fact]
        public async Task HTML_Without_Correct_js_ShouldThrowException_2()
        {
            using (Tester(TestContext.AlmostEmpty))
            {
                var vm = new object();
                NeutroniumException ex = null;

                try
                {
                    await HtmlBinding.Bind(_ViewEngine, vm, JavascriptBindingMode.OneTime);
                }
                catch (NeutroniumException myex)
                {
                    ex = myex;
                }

                ex.Should().NotBeNull();
            }
        }

        public static IEnumerable<object> BasicVmData
        {
            get
            {
                yield return new object[] { new BasicTestViewModel() };
                yield return new object[] { null };
            }
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_should_unlisten_when_changing_property(BasicTestViewModel remplacementChild)
        {
            var child = new BasicTestViewModel();
            var datacontext = new BasicTestViewModel { Child = child };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    child.ListenerCount.Should().Be(1);

                    DoSafeUI(() => datacontext.Child = remplacementChild);
                    await Task.Delay(300);

                    child.ListenerCount.Should().Be(0);

                    //If still listening to child, this will raise an exception
                    //for changing property on the wrong thread
                    var third = new BasicTestViewModel();
                    Action safe = () => child.Child = third;
                    safe.ShouldNotThrow();
                }
            };

            await RunAsync(test);
        }

        public static IEnumerable<object> CircularData
        {
            get
            {
                var root = new BasicTestViewModel { Child = new BasicTestViewModel() };
                yield return new object[] { root, new[] { root, root.Child } };

                var root2 = new BasicTestViewModel { Child = new BasicTestViewModel { Child = new BasicTestViewModel() } };
                yield return new object[] { root2, new[] { root2, root2.Child, root2.Child.Child } };

                var circular1 = new BasicTestViewModel();
                circular1.Child = circular1;

                yield return new object[] { circular1, new[] { circular1 } };

                var circular2 = new BasicTestViewModel { Child = new BasicTestViewModel() };
                circular2.Child.Child = circular2;

                yield return new object[] { circular2, new[] { circular2, circular2.Child } };
            }
        }


        [Theory]
        [MemberData(nameof(CircularData))]
        public async Task OneTime_does_not_listens_to_any_object(ViewModelTestBase datacontext, params ViewModelTestBase[] children)
        {
            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.OneTime),
                Test = (mb) =>
                {
                    children.ForEach(child => child.ListenerCount.Should().Be(0));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(CircularData))]
        public async Task TwoWay_listens_only_once_to_any_object(ViewModelTestBase datacontext, params ViewModelTestBase[] children)
        {
            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    children.ForEach(child => child.ListenerCount.Should().Be(1));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(CircularData))]
        public async Task TwoWay_unlistens_after_dipose(ViewModelTestBase datacontext, params ViewModelTestBase[] children)
        {
            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    mb.Dispose();
                    children.ForEach(child => child.ListenerCount.Should().Be(0));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_cleans_javascriptObject_cache_when_object_is_not_part_of_the_graph(BasicTestViewModel remplacementChild)
        {
            var datacontext = new BasicFatherTestViewModel();
            var child = new BasicTestViewModel();
            datacontext.Child = child;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var childJs = GetAttribute(js, "Child");

                    DoSafeUI(() => datacontext.Child = remplacementChild);
                    await Task.Delay(300);

                    var mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute", childJs));
                    await Task.Delay(300);

                    datacontext.CallCount.Should().Be(0);
                    datacontext.LastCallElement.Should().BeNull();
                }
            };

            await RunAsync(test);
        }

        public static IEnumerable<object> CircularDataBreaker
        {
            get
            {
                var auto = new BasicTestViewModel { };
                auto.Child = auto;

                yield return new object[] { auto, auto, new[] { auto }, new BasicTestViewModel[0] };

                var circular = new BasicTestViewModel { Child = new BasicTestViewModel() };
                circular.Child.Child = circular;

                yield return new object[] { circular, circular, new[] { circular }, new[] { circular.Child } };

                var circularLong = BuildLongCircular();
                yield return new object[] { circularLong, circularLong, new[] { circularLong }, new[] { circularLong.Child, circularLong.Child.Child } };

                var circularLong2 = BuildLongCircular();
                yield return new object[] { circularLong2, circularLong2.Child, new[] { circularLong2, circularLong2.Child }, new[] { circularLong2.Child.Child } };
            }
        }

        private static BasicTestViewModel BuildLongCircular()
        {
            var circularLong = new BasicTestViewModel { Child = new BasicTestViewModel { Child = new BasicTestViewModel() } };
            circularLong.Child.Child.Child = circularLong;
            return circularLong;
        }

        [Theory]
        [MemberData(nameof(CircularDataBreaker))]
        public async Task TwoWay_unlistens_when_object_is_not_part_of_the_graph_respecting_cycle(BasicTestViewModel root, BasicTestViewModel breaker, BasicTestViewModel[] survivores, BasicTestViewModel[] cleaned)
        {
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await DoSafeAsyncUI(() => breaker.Child = null);

                    survivores.ForEach(sur => sur.ListenerCount.Should().Be(1));
                    cleaned.ForEach(sur => sur.ListenerCount.Should().Be(0));
                }
            };

            await RunAsync(test);
        }

        private static BasicListTestViewModel BuildList()
        {
            var root = new BasicListTestViewModel
            {
                Children =
                {
                    new BasicTestViewModel(),
                    new BasicTestViewModel(),
                    new BasicTestViewModel()
                }
            };
            return root;
        }

        [Fact]
        public async Task Two_listens_to_elements_in_list()
        {
            var root = BuildList();
            var list = root.Children.ToList();
            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
               {
                   list.ForEach(child => child.ListenerCount.Should().Be(1));
               }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Two_unlistens_to_elements_removed_from_list_be_clean()
        {
            var root = BuildList();
            var list = root.Children.ToList();
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await DoSafeAsyncUI(() => root.Children.Clear());
                    list.ForEach(child => child.ListenerCount.Should().Be(0));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Two_unlistens_to_elements_removed_from_list_by_remove()
        {
            var root = BuildList();
            var list = root.Children.ToList();
            var removed = list[0];
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    await DoSafeAsyncUI(() => root.Children.RemoveAt(0));

                    removed.ListenerCount.Should().Be(0);
                    list.Skip(1).ForEach(child => child.ListenerCount.Should().Be(1));
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Two_unlistens_to_elements_removed_from_list_by_set()
        {
            var root = BuildList();
            var list = root.Children.ToList();
            var removed = list[0];
            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, root, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var newChild = new BasicTestViewModel();
                    await DoSafeAsyncUI(() => root.Children[0] = newChild);

                    removed.ListenerCount.Should().Be(0);
                    list.Skip(1).ForEach(child => child.ListenerCount.Should().Be(1));
                    newChild.ListenerCount.Should().Be(1);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_calls_comand_with_correct_argument()
        {
            var datacontext = new BasicFatherTestViewModel();
            var child = new BasicTestViewModel();
            datacontext.Child = child;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var childJs = GetAttribute(js, "Child");

                    var mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute", childJs));
                    await Task.Delay(300);

                    datacontext.CallCount.Should().Be(1);
                    datacontext.LastCallElement.Should().Be(child);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_rebinds_with_updated_objects()
        {
            var child = new BasicTestViewModel();
            var datacontext = new BasicTestViewModel { Child = child };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    DoSafeUI(() => datacontext.Child = null);

                    await Task.Delay(300);

                    var third = new BasicTestViewModel();
                    child.Child = third;

                    DoSafeUI(() => datacontext.Child = child);

                    await Task.Delay(300);

                    var child1 = GetAttribute(js, "Child");
                    var child2 = GetAttribute(child1, "Child");

                    var value = GetIntAttribute(child2, "Value");
                    value.Should().Be(-1);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_listens_to_all_changes()
        {
            var child = new BasicTestViewModel();
            var datacontext = new BasicTestViewModel { Child = child };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    DoSafeUI(() => datacontext.Child = null);

                    await Task.Delay(300);

                    var third = new BasicTestViewModel();
                    child.Child = third;

                    DoSafeUI(() => datacontext.Child = child);

                    await Task.Delay(300);

                    DoSafeUI(() => third.Value = 3);

                    await Task.Delay(300);

                    var child1 = GetAttribute(js, "Child");
                    var child2 = GetAttribute(child1, "Child");

                    var value = GetIntAttribute(child2, "Value");
                    value.Should().Be(3);

                    var newvalue = 44;
                    var intJS = _WebView.Factory.CreateInt(newvalue);
                    SetAttribute(child2, "Value", intJS);

                    await Task.Delay(300);

                    DoSafeUI(() =>
                    {
                        third.Value.Should().Be(newvalue);
                    });
                }
            };
            await RunAsync(test);
        }

        private class SmartVM : ViewModelTestBase
        {
            private int _MagicNumber;
            public int MagicNumber
            {
                get { return _MagicNumber; }
                set
                {
                    if (value == 9)
                    {
                        value = 42;
                    }
                    Set(ref _MagicNumber, value);
                }
            }
        }

        protected Task<IHtmlBinding> Bind(HtmlViewEngine engine, object dataContext, JavascriptBindingMode mode = JavascriptBindingMode.TwoWay)
        {
            return HtmlBinding.Bind(engine, dataContext, mode);
        }

        [Fact]
        public async Task TwoWay_RespectsPropertyValidation()
        {
            var datacontext = new SmartVM
            {
                MagicNumber = 8
            };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HtmlBinding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;

                    var res = GetIntAttribute(js, "MagicNumber");
                    res.Should().Be(8);

                    var intValue = 9;
                    var jsInt = Create(() => _WebView.Factory.CreateInt(intValue));
                    SetAttribute(js, "MagicNumber", jsInt);
                    await Task.Delay(100);

                    datacontext.MagicNumber.Should().Be(42);
                    await Task.Delay(100);

                    res = GetIntAttribute(js, "MagicNumber");
                    res.Should().Be(42);
                }
            };

            await RunAsync(test);
        }
    }
}

