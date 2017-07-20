using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAssertions;
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

namespace Tests.Universal.HTMLBindingTests
{
    public abstract class HTMLBindingTests : HTMLBindingBase
    {
        protected HTMLBindingTests(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }

        [Fact]
        public async Task InvokeAsync_No_Function()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (binding) =>
                {
                    var js = binding.JSRootObject;
                    var res = js.InvokeAsync("NotFound",binding.Context).Result;
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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (binding) =>
                {
                    var js = binding.JSRootObject;
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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (binding) =>
                {
                    var jsbridge = ((HTML_Binding) binding).JSBrideRootObject;

                    string JSON = JsonConvert.SerializeObject(_DataContext);
                    string alm = jsbridge.ToString();

                    JSArray arr = (JSArray)jsbridge.GetAllChildren().First(c => c is JSArray);

                    string stringarr = arr.ToString();

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
                    await HTML_Binding.Bind(_ViewEngine, new object(), JavascriptBindingMode.OneTime);
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
                    await HTML_Binding.Bind(_ViewEngine, _DataContext, JavascriptBindingMode.OneTime);
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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneTime),
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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneTime),
                Test = async (mb) =>
                {
                    var jsbridge = (mb as HTML_Binding).JSBrideRootObject;
                    var js = mb.JSRootObject;

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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = async (mb) =>
                {
                    var jsbridge = (mb as HTML_Binding).JSBrideRootObject;
                    var js = mb.JSRootObject;

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

                    string res4 = GetLocalCity( js);
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
                 Bind = (win) => HTML_Binding.Bind(win, dt, JavascriptBindingMode.OneWay),
                 Test = (mb) =>
                 {
                     var jsbridge = (mb as HTML_Binding).JSBrideRootObject;
                     var js = mb.JSRootObject;

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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
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
                  Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                  Test = async (mb) => {
                      var js = mb.JSRootObject;

                      var res = GetAttribute(js, "MainSkill");
                      res.IsNull.Should().BeTrue();

                      DoSafeUI(() => _DataContext.MainSkill = new Skill() {
                          Name = "C++",
                          Type = "Info"
                      });

                      await Task.Delay(200);

                      res = GetAttribute(js, "MainSkill");
                      DoSafe(() => {
                          res.IsNull.Should().BeFalse();
                          res.IsObject.Should().BeTrue();
                      });
                    

                      string inf = GetStringAttribute(res, "Type");
                      inf.Should().Be("Info");

                      DoSafeUI(() =>
                      _DataContext.MainSkill = null);
                      await Task.Delay(200);

                      res = GetAttribute(js, "MainSkill");

                      //GetSafe(()=>res.IsNull).Should().BeTrue();
                      //Awesomium limitation can not test on isnull
                      //Todo: create specific test
                      object obj =null;
                      var boolres = GetSafe( () => _WebView.Converter.GetSimpleValue(res, out obj));
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
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;

                      var One = GetAttribute(js, "One");

                      string res = GetStringAttribute(One, "Name");
                      res.Should().Be("O Monstro");

                      string res2 = GetStringAttribute(One, "LastName");
                      res2.Should().Be("Desmaisons");

                      //Test no stackoverflow in case of circular refernce
                      var jsbridge = (mb as HTML_Binding).JSBrideRootObject;
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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JSRootObject;

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
            var datacontext = new SimpleViewModel() {Name = "teste0"};

            var test = new TestInContextAsync() 
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JSRootObject;

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
        public async Task TwoWay()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContextAsync()
              {
                  Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                  Test = async (mb) =>
                  {
                      var js = mb.JSRootObject;

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
                  Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                  Test = async (mb) =>
                  {
                      var js = mb.JSRootObject;

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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
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

                    var js = mb.JSRootObject;

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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
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

                    var js = mb.JSRootObject;

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
                  Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                  Test = async (mb) =>
                  {
                      var js = mb.JSRootObject;

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
                  Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                  Test = async (mb) =>
                  {
                      var js = mb.JSRootObject;

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
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = async (mb) =>
                  {
                      var js = mb.JSRootObject;

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
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = async (mb) =>
                  {
                      var js = mb.JSRootObject;

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

                      var five = Create(()=> _WebView.Factory.CreateInt(5));
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
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = async (mb) =>
                  {
                      var js = mb.JSRootObject;

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
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = async (mb) =>
                  {
                      var js = mb.JSRootObject;
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
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = async (mb) =>
                  {
                      var js = mb.JSRootObject;
                      var res1 = GetAttribute(js, "One");
                      res1.Should().NotBeNull();
                      string n1 = GetStringAttribute(res1, "Name");
                      n1.Should().Be("David");

                      var res2 = GetAttribute(js, "Two");
                      res2.Should().NotBeNull();
                      string n2 = GetStringAttribute(res2, "Name");
                      n2.Should().Be("Claudia");

                      var trueJs = _WebView.Factory.CreateObject(true);
                      SetAttribute(js, "One", trueJs);

                      var res3 = GetAttribute(js, "One");
                      res3.IsObject.Should().BeTrue();

                      await Task.Delay(100);
                      datacontext.One.Should().Be(p1);
                  }
              };
            await RunAsync(test);
        }

        private class ViewModelTest : ViewModelBase
        {
            private ICommand _ICommand;
            public ICommand Command { get { return _ICommand; } set { Set(ref _ICommand, value, "Command"); } }

            public string Name { get { return "NameTest"; } }

            public string UselessName { set { } }

            public void InconsistentEventEmit()
            {
                this.OnPropertyChanged("NonProperty");
            }
        }

        [Fact]
        public async Task Property_Test()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new ViewModelTest() { Command = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

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
            var datacontexttest = new ViewModelTest() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = ((HTML_Binding) mb).JSBrideRootObject as JsGenericObject;

                    var mycommand = js.Attributes["Command"] as JSCommand;
                    mycommand.Should().NotBeNull();
                    mycommand.ToString().Should().Be("{}");
                    mycommand.Type.Should().Be(JsCsGlueType.Command);
                    mycommand.MappedJSValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new ViewModelTest() { Command = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

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
            var datacontexttest = new ViewModelTest() { Command = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;
                    var mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute", js));
                    await Task.Delay(100);
                    command.Received().Execute(datacontexttest);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_CanExecute_False()
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(false);
            var datacontexttest = new ViewModelTest() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");

                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_CanExecute_True()
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(true);
            var datacontexttest = new ViewModelTest() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");

                    res.Should().BeTrue();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Command_Uptate_From_Null()
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(true);
            var datacontexttest = new ViewModelTest();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

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

        #region SimpleCommand

        private class ViewModelSimpleCommandTest : ViewModelBase
        {
            private ISimpleCommand _ICommand;
            public ISimpleCommand SimpleCommand { get { return _ICommand; } set { Set(ref _ICommand, value, "SimpleCommand"); } }
        }


        [Fact]
        public async Task TwoWay_SimpleCommand_Without_Parameter()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontexttest = new ViewModelSimpleCommandTest() { SimpleCommand = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommand");
                    DoSafe(() => Call(mycommand, "Execute"));
                    await Task.Delay(100);
                    command.Received().Execute(null);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_SimpleCommand_With_Parameter()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontexttest = new ViewModelSimpleCommandTest() { SimpleCommand = command };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;
                    var mycommand = GetAttribute(js, "SimpleCommand");
                    DoSafe(() => Call(mycommand, "Execute", js));
                    await Task.Delay(100);
                    command.Received().Execute(datacontexttest);
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task TwoWay_SimpleCommand_Name()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontexttest = new ViewModelSimpleCommandTest() { SimpleCommand = command };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {

                    mb.ToString().Should().Be(@"{""SimpleCommand"":{}}");

                    var js = (mb as HTML_Binding).JSBrideRootObject as JsGenericObject;

                    var mysimplecommand = js.Attributes["SimpleCommand"] as JsSimpleCommand;
                    mysimplecommand.Should().NotBeNull();
                    mysimplecommand.ToString().Should().Be("{}");
                    mysimplecommand.Type.Should().Be(JsCsGlueType.SimpleCommand);
                    mysimplecommand.MappedJSValue.Should().NotBeNull();
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
            var datacontext = new ViewModelCLRTypes();

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;
                    js.Should().NotBeNull();

                    CheckIntValue(js, "int64", 0);
                    CheckIntValue(js, "int32", 0);
                    CheckIntValue(js, "int16", 0);

                    CheckIntValue(js, "uint16", 0);
                    CheckIntValue(js, "uint32", 0);
                    CheckIntValue(js, "uint64", 0);

                    CheckIntValue(js, "Double", 0);
                    CheckIntValue(js, "Decimal", 0);
                    CheckIntValue(js, "Float", 0);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_CLR_Type_FromjavascripttoCto()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontext = new ViewModelCLRTypes();

            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;
                    js.Should().NotBeNull();

                    SetAttribute(js, "int64", _WebView.Factory.CreateInt(32));
                    await Task.Delay(200);
                    datacontext.int64.Should().Be(32);

                    SetAttribute(js, "uint64",  _WebView.Factory.CreateInt(456));
                    await Task.Delay(200);
                    datacontext.uint64.Should().Be(456);

                    SetAttribute(js, "int32",  _WebView.Factory.CreateInt(5));
                    await Task.Delay(200);
                    datacontext.int32.Should().Be(5);

                    SetAttribute(js, "uint32",  _WebView.Factory.CreateInt(67));
                    await Task.Delay(200);
                    datacontext.uint32.Should().Be(67);

                    SetAttribute(js, "int16",  _WebView.Factory.CreateInt(-23));
                    await Task.Delay(200);
                    datacontext.int16.Should().Be(-23);

                    SetAttribute(js, "uint16",  _WebView.Factory.CreateInt(9));
                    await Task.Delay(200);
                    datacontext.uint16.Should().Be(9);

                    SetAttribute(js, "Float",  _WebView.Factory.CreateDouble(888.78));
                    await Task.Delay(200);
                    datacontext.Float.Should().Be(888.78f);

                    SetAttribute(js, "Double",  _WebView.Factory.CreateDouble(866.76));
                    await Task.Delay(200);
                    datacontext.Double.Should().Be(866.76);

                    SetAttribute(js, "Decimal",  _WebView.Factory.CreateDouble(0.5));
                    await Task.Delay(200);
                    datacontext.Decimal.Should().Be(0.5m);
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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;
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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) => 
                {
                    var js = mb.JSRootObject;

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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    _ICommand.Received().CanExecute(Arg.Any<object>());
                    var js = mb.JSRootObject;

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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

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
            _ICommand = new RelayCommand(() =>
                {
                    _DataContext.MainSkill = new Skill();
                    _DataContext.Skills.Add(_DataContext.MainSkill);
                });
       
            _DataContext.TestCommand = _ICommand;
            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

                    _DataContext.Skills.Should().HaveCount(2);

                    DoSafeUI(() => {
                        _ICommand.Execute(null);
                    });

                    await Task.Delay(150);

                    var res = GetCollectionAttribute(js , "Skills");

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
            var test = new ViewModelTest() { Command = command };

            var testR = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, test, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

                    var mycommand = GetAttribute(js, "Command");
                    Call(mycommand, "Execute", _WebView.Factory.CreateNull());

                    await Task.Delay(150);
                    command.Received().Execute(null);
                }
            };

            await RunAsync(testR);
        }


        [Fact]
        public async Task TwoWay_ResultCommand_Should_have_ToString()
        {
            var command = Substitute.For<ICommand>();
            var test = new ViewModelTest() { Command = command };

            var testR = new TestInContext()
            {
                Path = TestContext.IndexPromise,
                Bind = (win) => HTML_Binding.Bind(win, test, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    mb.ToString().Should().NotBeNull();
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
                Bind = (win) => HTML_Binding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;
                    var mycommand = GetAttribute(js, "CreateObject");
                    Call(mycommand, "Execute", _WebView.Factory.CreateInt(25));

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
                Bind = (win) => HTML_Binding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                { 
                  
                    {
                        var glueobj = (mb as HTML_Binding).JSBrideRootObject as JsGenericObject;
                        var mysimplecommand = glueobj.Attributes["CreateObject"] as JsResultCommand;
                        mysimplecommand.Should().NotBeNull();
                        mysimplecommand.ToString().Should().Be("{}");
                        mysimplecommand.Type.Should().Be(JsCsGlueType.ResultCommand);
                        mysimplecommand.MappedJSValue.Should().NotBeNull();
                    }
                   
                    var js = mb.JSRootObject;
                    var mycommand = GetAttribute(js, "CreateObject");

                    IJavascriptObject cb = null;
                    bool res = _WebView.Eval("(function(){return { fullfill: function (res) {window.res=res; }, reject: function(err){window.err=err;}}; })();", out cb);

                    res.Should().BeTrue();
                    cb.Should().NotBeNull();
                    cb.IsObject.Should().BeTrue();

                    var resdummy = this.CallWithRes(mycommand, "Execute", _WebView.Factory.CreateInt(25), cb);

                    await Task.Delay(100);
                    function.Received(1).Invoke(25);

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
                Bind = (win) => HTML_Binding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

                    var mycommand = GetAttribute(js, "CreateObject");
                    IJavascriptObject cb = null;
                    bool res = _WebView.Eval("(function(){return { fullfill: function (res) {window.res=res; }, reject: function(err){window.err=err;}}; })();", out cb);

                    res.Should().BeTrue();
                    cb.Should().NotBeNull();
                    cb.IsObject.Should().BeTrue();

                    var resdummy = this.CallWithRes(mycommand, "Execute", _WebView.Factory.CreateInt(25), cb);
                    await Task.Delay(100);
                    function.Received(1).Invoke(25);

                    await Task.Yield();

                    var error = _WebView.GetGlobal().GetValue("err").GetStringValue();
                    error.Should().Be(errormessage);

                    var resvalue = _WebView.GetGlobal().GetValue("res");
                    resvalue.IsUndefined.Should().BeTrue();
                }
            };

            await RunAsync(test);
        }

        //private IJavascriptObject UnWrapCollection(IJavascriptObject root, string att)
        //{
        //    return root.Invoke(att, this._WebView).ExecuteFunction(_WebView);
        //    return 
        //}

        [Fact]
        public async Task TwoWay_Collection()
        {

            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var root = (mb as HTML_Binding).JSBrideRootObject as JsGenericObject;
                    var js = mb.JSRootObject;

                    var col = GetSafe(() => GetCollectionAttribute(js, "Skills"));
                    col.GetArrayLength().Should().Be(2);

                    Check(col, _DataContext.Skills);

                    var coll = GetAttribute(js, "Skills");
                    Call(coll, "push", (root.Attributes["Skills"] as JSArray).Items[0].GetJSSessionValue());

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
                          (root.Attributes["Skills"] as JSArray).Items[0].GetJSSessionValue());

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
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var root = (mb as HTML_Binding).JSBrideRootObject as JsGenericObject;
                    var js = mb.JSRootObject;

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



        private class VMWithList<T> : ViewModelBase
        {
            public VMWithList()
            {
                List = new ObservableCollection<T>();
            }
            public ObservableCollection<T> List { get; private set; }
        }

        private class VMWithListNonGeneric : ViewModelBase
        {
            public VMWithListNonGeneric()
            {
                List = new ArrayList();
            }
            public ArrayList List { get; private set; }
        }

        private class VMwithdecimal : ViewModelBase
        {
            public VMwithdecimal()
            {
            }

            private decimal _DecimalValue;
            public decimal decimalValue
            {
                get { return _DecimalValue; }
                set { Set(ref _DecimalValue, value, "decimalValue"); }
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
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

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


        private class VMwithlong : ViewModelBase
        {
            public VMwithlong()
            {
            }

            private long _LongValue;
            public long longValue
            {
                get { return _LongValue; }
                set { Set(ref _LongValue, value, "decimalValue"); }
            }
        }

        [Fact]
        public async Task TwoWay_Long_ShouldOK()
        {
            var datacontext = new VMwithlong() { longValue = 45 };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;
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
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

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

                    var comp = new List<string>(datacontext.List);
                    comp.Add("newvalue");

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
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

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
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

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
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

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

                    var comp = new List<decimal>(datacontext.List);
                    comp.Add(0.55m);

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
        public async Task stringBinding()
        {
            var datacontext = new VMWithList<decimal>();

            var test = new TestInContext()
            {
                Bind = (win) => StringBinding.Bind(win, "{\"LastName\":\"Desmaisons\",\"Name\":\"O Monstro\",\"BirthDay\":\"0001-01-01T00:00:00.000Z\",\"PersonalState\":\"Married\",\"Age\":0,\"Local\":{\"City\":\"Florianopolis\",\"Region\":\"SC\"},\"MainSkill\":{},\"States\":[\"Single\",\"Married\",\"Divorced\"],\"Skills\":[{\"Type\":\"French\",\"Name\":\"Langage\"},{\"Type\":\"C++\",\"Name\":\"Info\"}]}"),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

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
                    await HTML_Binding.Bind(_ViewEngine, vm, JavascriptBindingMode.OneTime);
                }
                catch (NeutroniumException myex)
                {
                    ex = myex;
                }

                ex.Should().NotBeNull();
            }
        }

        public class BasicVm : ViewModelBase
        {
            private BasicVm _Child;
            public BasicVm Child
            {
                get { return _Child; }
                set { Set(ref _Child, value, nameof(Child)); }
            }

            private int _Value = -1;
            public int Value
            {
                get { return _Value; }
                set { Set(ref _Value, value, nameof(Value)); }
            }
        }

        [Fact]
        public async Task TwoWay_should_unlisten_when_setting_a_null_property()
        {
            var child = new BasicVm();
            var datacontext = new BasicVm();
            datacontext.Child = child;

            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

                    DoSafeUI(() => datacontext.Child = null);
                    await Task.Delay(300);

                    var third = new BasicVm();

                    //If still listening to child, this will raise an exception
                    //for changing property on the wrong thread
                    Action safe = () => child.Child = third;
                    AssertionExtensions.ShouldNotThrow(safe);
                }
            };

            await RunAsync(test);
        }

        //[Fact]
        //public async Task TwoWay_should_listen_to_all_changes()
        //{
        //    var child = new BasicVm();
        //    var datacontext = new BasicVm();
        //    datacontext.Child = child;

        //    var test = new TestInContextAsync()
        //    {
        //        Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
        //        Test = async (mb) =>
        //        {
        //            var js = mb.JSRootObject;

        //            DoSafeUI(() => datacontext.Child = null);

        //            await Task.Delay(300);

        //            var third = new BasicVm();
        //            child.Child = third;

        //            DoSafeUI(() => datacontext.Child = child);

        //            await Task.Delay(300);

        //            DoSafeUI(() => third.Value = 3);

        //            await Task.Delay(300);

        //            var child1 = GetAttribute(js, "Child");
        //            var child2 = GetAttribute(child1, "Child");

        //            var value = GetIntAttribute(child2, "Value");
        //            value.Should().Be(3);

        //            var newvalue = 44;
        //            var intJS = _WebView.Factory.CreateInt(newvalue);
        //            SetAttribute(child2, "Value", intJS);

        //            await Task.Delay(300);

        //            DoSafeUI(() =>
        //            {
        //                third.Value.Should().Be(newvalue);
        //            });
        //        }
        //    };

        //    await RunAsync(test);
        //}

        private class SmartVM : ViewModelBase
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
                    Set(ref _MagicNumber, value, nameof(MagicNumber));
                }
            }
        }

        [Fact]
        public async Task TwoWay_ShouldRespectPropertyValidation()
        {
            var datacontext = new SmartVM
            {
                MagicNumber = 8
            };

            var test = new TestInContextAsync()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;

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

