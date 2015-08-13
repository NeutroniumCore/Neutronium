using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Xunit;
using NSubstitute;
using FluentAssertions;
using Newtonsoft.Json;
using Xilium.CefGlue;

using MVVM.CEFGlue.HTMLBinding;
using MVVM.CEFGlue.Infra;
using MVVM.CEFGlue.ViewModel.Example;
using MVVM.CEFGlue.ViewModel;
using MVVM.CEFGlue.ViewModel.Infra;
using MVVM.CEFGlue.Exceptions;
using MVVM.CEFGlue.Test.ViewModel.Test;
using MVVM.Component;
using MVVM.CEFGlue.CefGlueHelper;


namespace MVVM.CEFGlue.Test
{
    public class Test_HTMLBinding : MVVMCefGlue_Test_Base
    {
        private Person _DataContext;
        private ICommand _ICommand;


        public Test_HTMLBinding()
            : base()
        {
            _ICommand = Substitute.For<ICommand>();
            _DataContext = new Person(_ICommand)
            {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new Local() { City = "Florianopolis", Region = "SC" },
                PersonalState = PersonalState.Married
            };

            _DataContext.Skills.Add(new Skill() { Name = "Langage", Type = "French" });
            _DataContext.Skills.Add(new Skill() { Name = "Info", Type = "C++" });
        }

        [Fact]
        public async Task Test_InvokeAsync_No_Function()
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
        public async Task Test_GetElements_Should_Throw_Exception()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (binding) =>
                {
                    var js = binding.JSRootObject;
                    CefV8Value[] res = null;
                    Action act = () => res = js.GetArrayElements();
                    act.ShouldThrow<ArgumentException>();
                }
            };

            await RunAsync(test);
        }

        

        [Fact]
        public async Task Test_HTMLBinding_Basic_OneWay_JSON_ToString()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (binding) =>
                {
                    var jsbridge = (binding as HTML_Binding).JSBrideRootObject;
                    var js = binding.JSRootObject;

                    string JSON = JsonConvert.SerializeObject(_DataContext);
                    string alm = jsbridge.ToString();

                    JSArray arr = (JSArray)jsbridge.GetAllChildren().Where(c => c is JSArray).First();

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
        public async Task Test_HTMLBinding_Basic_HTML_Withoutko_ShouldThrowException()
        {
            using (Tester("javascript\\empty_with_js.html"))
            {
                var vm = new object();
                MVVMCEFGlueException ex = null;

                try
                {
                    await HTML_Binding.Bind(_ICefGlueWindow, new object(), JavascriptBindingMode.OneTime);
                }
                catch (MVVMCEFGlueException myex)
                {
                    ex = myex;
                }

                ex.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_HTML_Without_Correct_js_ShouldThrowException()
        {
            using (Tester("javascript\\almost_empty.html"))
            {
                var vm = new object();
                MVVMCEFGlueException ex = null;

                try
                {
                    await HTML_Binding.Bind(_ICefGlueWindow, _DataContext, JavascriptBindingMode.OneTime);
                }
                catch (MVVMCEFGlueException myex)
                {
                    ex = myex;
                }

                ex.Should().NotBeNull();
            }
        }

          [Fact]
        public async Task Test_HTMLBinding_Basic_OneTime_CheckContext()
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
        public async Task Test_HTMLBinding_Basic_OneTime()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneTime),
                Test = (mb) =>
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
                    Thread.Sleep(200);

                    string res3 = GetStringAttribute(js, "Name");
                    res3.Should().Be("O Monstro");

                    string res4 = GetSafe(() => js.Invoke("Local", this._WebView).Invoke("City", this._WebView).GetStringValue());
                    res4.Should().Be("Florianopolis");

                    _DataContext.Local.City = "Paris";
                    Thread.Sleep(200);

                    //onetime does not update javascript from  C# 
                    res4 = GetSafe(() => js.Invoke("Local", this._WebView).Invoke("City", this._WebView).GetStringValue());
                    res4.Should().Be("Florianopolis");

                    string res5 = GetSafe(() =>
                        js.Invoke("Skills", this._WebView).ExecuteFunction().GetValue(0).Invoke("Name", this._WebView).GetStringValue()
                        );
                    res5.Should().Be("Langage");

                    _DataContext.Skills[0].Name = "Ling";
                    Thread.Sleep(200);

                    //onetime does not update javascript from  C# 
                    res5 = GetSafe(() => js.Invoke("Skills", this._WebView).ExecuteFunction().GetValue(0).Invoke("Name", this._WebView).GetStringValue());
                    res5.Should().Be("Langage");

                    //onetime does not update C# from javascript
                    this.Call(js, "Name", () => CefV8Value.CreateString("resName"));
                    Thread.Sleep(200);
                    _DataContext.Name.Should().Be("23");
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_OneWay()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (mb) =>
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
                    Thread.Sleep(200);

                    string res3 = GetStringAttribute(js, "Name");
                    res3.Should().Be("23");

                    string res4 = GetSafe(() => js.Invoke("Local", this._WebView).Invoke("City", this._WebView).GetStringValue());
                    res4.Should().Be("Florianopolis");

                    _DataContext.Local.City = "Paris";
                    Thread.Sleep(200);

                    res4 = GetSafe(() => js.Invoke("Local", this._WebView).Invoke("City", this._WebView).GetStringValue());
                    ((string)res4).Should().Be("Paris");

                    string res5 = GetSafe(() => js.Invoke("Skills", this._WebView).ExecuteFunction().GetValue(0).Invoke("Name", this._WebView).GetStringValue());
                    res5.Should().Be("Langage");

                    _DataContext.Skills[0].Name = "Ling";
                    Thread.Sleep(200);

                    res5 = GetSafe(() => GetSafe(() => js.Invoke("Skills", this._WebView).ExecuteFunction().GetValue(0).Invoke("Name", this._WebView).GetStringValue()));
                    res5.Should().Be("Ling");


                    this.Call(js, "Name", () => CefV8Value.CreateString("resName"));
                    Thread.Sleep(200);
                    _DataContext.Name.Should().Be("23");
                }
            };

            await RunAsync(test);
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
        public async Task Test_HTMLBinding_Basic_OneWay_Property_With_Exception()
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
        public async Task Test_HTMLBinding_Basic_Regsiter_Additional_property()
        {
            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
                Test = (mb) =>
                {
                    var jsbridge = (mb as HTML_Binding).JSBrideRootObject;
                    var js = mb.JSRootObject;

                    string res = GetStringAttribute(js, "completeName");
                    res.Should().Be("O Monstro Desmaisons");
                }
            };

            await RunAsync(test);
        }



        [Fact]
        public async Task Test_HTMLBinding_Basic_Null_Property()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContext()
              {
                  Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;

                      CefV8Value res = GetAttribute(js, "MainSkill");
                      res.IsNull.Should().BeTrue();

                      DoSafe(() =>
                      _DataContext.MainSkill = new Skill() { Name = "C++", Type = "Info" });
                      Thread.Sleep(100);

                      res = GetAttribute(js, "MainSkill");
                      res.IsNull.Should().BeFalse();
                      res.IsObject.Should().BeTrue();

                      string inf = GetStringAttribute(res, "Type");
                      inf.Should().Be("Info");

                      DoSafe(() =>
                      _DataContext.MainSkill = null);
                      Thread.Sleep(100);

                      res = GetAttribute(js, "MainSkill");
                      res.IsNull.Should().BeTrue();

                  }
              };
            await RunAsync(test);
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_Circular_reference()
        {
            var datacontext = new MVVM.CEFGlue.ViewModel.Example.ForNavigation.Couple();
            var my = new MVVM.CEFGlue.ViewModel.Example.ForNavigation.Person()
            {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new MVVM.CEFGlue.ViewModel.Example.Local() { City = "Florianopolis", Region = "SC" }
            };
            my.Couple = datacontext;
            datacontext.One = my;

            var test = new TestInContext()
              {
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;

                      CefV8Value One = GetAttribute(js, "One");

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
        public async Task Test_HTMLBinding_Basic_TwoWay()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContext()
              {
                  Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;

                      string res = GetStringAttribute(js, "Name");
                      res.Should().Be("O Monstro");

                      string res2 = GetStringAttribute(js, "LastName");
                      res2.Should().Be("Desmaisons");

                      _DataContext.Name = "23";

                      Thread.Sleep(50);
                      string res3 = GetStringAttribute(js, "Name");
                      res3.Should().Be("23");

                      string res4 = GetSafe(() => js.Invoke("Local", this._WebView).Invoke("City", this._WebView).GetStringValue());
                      res4.Should().Be("Florianopolis");

                      _DataContext.Local.City = "Paris";
                      Thread.Sleep(50);

                      res4 = GetSafe(() => js.Invoke("Local", this._WebView).Invoke("City", this._WebView).GetStringValue());
                      ((string)res4).Should().Be("Paris");

                      string res5 = GetSafe(() => js.Invoke("Skills", this._WebView).ExecuteFunction().GetValue(0).Invoke("Name", this._WebView).GetStringValue());
                      res5.Should().Be("Langage");

                      _DataContext.Skills[0].Name = "Ling";
                      Thread.Sleep(50);

                      res5 = GetSafe(() => js.Invoke("Skills", this._WebView).ExecuteFunction().GetValue(0).Invoke("Name", this._WebView).GetStringValue());
                      res5.Should().Be("Ling");

                      //Teste Two Way
                      this.Call(js, "Name", () => CefV8Value.CreateString("resName"));

                      string resName = GetStringAttribute(js, "Name");
                      resName.Should().Be("resName");

                      Thread.Sleep(500);

                      _DataContext.Name.Should().Be("resName");

                      _DataContext.Name = "nnnnvvvvvvv";

                      Thread.Sleep(50);
                      res3 = GetStringAttribute(js, "Name");
                      ((string)res3).Should().Be("nnnnvvvvvvv");
                  }
              };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Nested()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContext()
              {
                  Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;

                      string res = GetStringAttribute(js, "Name");
                      res.Should().Be("O Monstro");

                      string res2 = GetStringAttribute(js, "LastName");
                      res2.Should().Be("Desmaisons");

                      CefV8Value local = GetAttribute(js, "Local");
                      string city = GetStringAttribute(local, "City");
                      city.Should().Be("Florianopolis");

                      _DataContext.Local.City = "Foz de Iguaçu";

                      Thread.Sleep(100);
                      CefV8Value local3 = GetAttribute(js, "Local");
                      string city3 = GetStringAttribute(local, "City");
                      city3.Should().Be("Foz de Iguaçu");

                      _DataContext.Local = new Local() { City = "Paris" };

                      Thread.Sleep(50);
                      CefV8Value local2 = GetAttribute(js, "Local");
                      string city2 = GetStringAttribute(local2, "City");
                      city2.Should().Be("Paris");
                  }
              };

            await RunAsync(test);
        }


        [Fact]
        public async Task Test_HTMLBinding_TwoWay_Enum()
        {
            _DataContext.MainSkill.Should().BeNull();

            var test = new TestInContext()
              {
                  Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;

                      CefV8Value res = GetAttribute(js, "PersonalState");
                      string dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                      dres.Should().Be("Married");

                      _DataContext.PersonalState = PersonalState.Single;
                      Thread.Sleep(50);

                      res = GetAttribute(js, "PersonalState");
                      dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                      dres.Should().Be("Single");
                  }
              };

            await RunAsync(test);

        }

        [Fact]
        public async Task Test_HTMLBinding_TwoWay_Enum_Round_Trip()
        {
            _DataContext.Name = "toto";

            var test = new TestInContext()
              {
                  Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;

                      CefV8Value res = GetAttribute(js, "PersonalState");
                      string dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                      dres.Should().Be("Married");

                      _DataContext.PersonalState = PersonalState.Single;
                      Thread.Sleep(50);

                      res = GetAttribute(js, "PersonalState");
                      dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                      dres.Should().Be("Single");

                      var othervalue = GetSafe(() => js.Invoke("States", _WebView).ExecuteFunction());
                      //JSValue[] coll = (JSValue[])othervalue;
                      CefV8Value di = othervalue.GetValue(2);
                      string name = GetSafe(() => di.GetValue("displayName").GetStringValue());
                      name.Should().Be("Divorced");


                      this.DoSafe(() => js.Invoke("PersonalState", _WebView, di));
                      Thread.Sleep(100);

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
        public async Task Test_HTMLBinding_TwoWay_Enum_NotMapped()
        {
            var datacontext = new SimplePerson();
            datacontext.PersonalState = PersonalState.Single;

            var test = new TestInContext()
              {
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;

                      CefV8Value res = GetAttribute(js, "PersonalState");
                      string dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                      dres.Should().Be("Single");


                      datacontext.PersonalState = PersonalState.Married;
                      Thread.Sleep(50);

                      res = GetAttribute(js, "PersonalState");
                      dres = GetSafe(() => res.GetValue("displayName").GetStringValue());
                      dres.Should().Be("Married");
                  }

              };

            await RunAsync(test);
        }


        [Fact]
        public async Task Test_HTMLBinding_TwoWay_Set_Object_From_Javascipt()
        {
            var datacontext = new Couple();
            var p1 = new Person() { Name = "David" };
            datacontext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            datacontext.Two = p2;

            var test = new TestInContext()
              {
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;

                      CefV8Value res1 = GetAttribute(js, "One");
                      string n1 = GetStringAttribute(res1, "Name");
                      n1.Should().Be("David");

                      CefV8Value res2 = GetAttribute(js, "Two");
                      res2.Should().NotBeNull();
                      var n2 = GetStringAttribute(res2, "Name");
                      n2.Should().Be("Claudia");

                      DoSafe(() => Call(js, "One", () => GetAttribute(js, "Two")));
                      Thread.Sleep(100);

                      CefV8Value res3 = GetAttribute(js, "One");
                      res3.Should().NotBeNull();
                      string n3 = GetStringAttribute(res3, "Name");
                      n3.Should().Be("Claudia");

                      Thread.Sleep(100);

                      datacontext.One.Should().Be(p2);

                      CefV8Value res4 = GetAttribute(res3, "ChildrenNumber");
                      res4.IsNull.Should().BeTrue();

                      //CefV8Value five = get new JSValue(5);
                      DoSafe(() => Call(res3, "ChildrenNumber", CefV8Value.CreateInt(5)));
                      Thread.Sleep(100);

                      datacontext.One.ChildrenNumber.Should().Be(5);
                  }
              };

            await RunAsync(test);
        }




        [Fact]
        public async Task Test_HTMLBinding_TwoWay_Set_Null_From_Javascipt()
        {
            var datacontext = new Couple();
            var p1 = new Person() { Name = "David" };
            datacontext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            datacontext.Two = p2;

            var test = new TestInContext()
              {
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;

                      CefV8Value res1 = GetAttribute(js, "One");
                      res1.Should().NotBeNull();
                      string n1 = GetStringAttribute(res1, "Name");
                      n1.Should().Be("David");

                      CefV8Value res2 = GetAttribute(js, "Two");
                      res2.Should().NotBeNull();
                      string n2 = GetStringAttribute(res2, "Name");
                      n2.Should().Be("Claudia");

                      DoSafe(() => Call(js, "One", CefV8Value.CreateNull()));

                      CefV8Value res3 = GetAttribute(js, "One");
                      res3.IsNull.Should().BeTrue();

                      Thread.Sleep(100);

                      datacontext.One.Should().BeNull();
                  }
              };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_TwoWay_Set_Object_From_Javascipt_Survive_MissUse()
        {
            var datacontext = new Couple();
            var p1 = new Person() { Name = "David" };
            datacontext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            datacontext.Two = p2;

            var test = new TestInContext()
              {
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;
                      CefV8Value res1 = GetAttribute(js, "One");
                      res1.Should().NotBeNull();
                      string n1 = GetStringAttribute(res1, "Name");
                      n1.Should().Be("David");

                      CefV8Value res2 = GetAttribute(js, "Two");
                      res2.Should().NotBeNull();
                      string n2 = GetStringAttribute(res2, "Name");
                      n2.Should().Be("Claudia");

                      DoSafe(() => Call(js, "One", CefV8Value.CreateString("Dede")));

                      string res3 = GetStringAttribute(js, "One");
                      res3.Should().Be("Dede");

                      Thread.Sleep(100);

                      datacontext.One.Should().Be(p1);
                  }
              };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_TwoWay_Set_Object_From_Javascipt_Survive_MissUse_NoReset_OnAttribute()
        {
            var datacontext = new Couple();
            var p1 = new Person() { Name = "David" };
            datacontext.One = p1;
            var p2 = new Person() { Name = "Claudia" };
            datacontext.Two = p2;

            var test = new TestInContext()
              {
                  Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                  Test = (mb) =>
                  {
                      var js = mb.JSRootObject;
                      CefV8Value res1 = GetAttribute(js, "One");
                      res1.Should().NotBeNull();
                      string n1 = GetStringAttribute(res1, "Name");
                      n1.Should().Be("David");

                      CefV8Value res2 = GetAttribute(js, "Two");
                      res2.Should().NotBeNull();
                      string n2 = GetStringAttribute(res2, "Name");
                      n2.Should().Be("Claudia");

                      DoSafe(() => Call(js, "One", CefV8Value.CreateObject(null)));

                      CefV8Value res3 = GetAttribute(js, "One");
                      res3.IsObject.Should().BeTrue();

                      Thread.Sleep(100);

                      datacontext.One.Should().Be(p1);
                  }
              };
            await RunAsync(test);
        }




        private void Check(CefV8Value coll, IList<Skill> iskill)
        {
            coll.GetArrayLength().Should().Be(iskill.Count);

            for (int i = 0; i < iskill.Count; i++)
            {
                var c = coll.GetValue(i);

                (GetSafe(() => GetStringAttribute(c, "Name"))).Should().Be(iskill[i].Name);
                (GetSafe(() => GetStringAttribute(c, "Type"))).Should().Be(iskill[i].Type);
            }
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
        public async Task Test_HTMLBinding_Basic_Property_Test()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new ViewModelTest() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    string res = GetStringAttribute(js, "Name");
                    res.Should().Be("NameTest");

                    DoSafe(() => Call(js, "Name", CefV8Value.CreateString("NewName")));
                    res = GetStringAttribute(js, "Name");
                    res.Should().Be("NewName");

                    Thread.Sleep(100);
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
        public async Task Test_HTMLBinding_Basic_TwoWay_Command_Basic()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new ViewModelTest() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = (mb as HTML_Binding).JSBrideRootObject as JSGenericObject;

                    var mycommand = js.Attributes["Command"] as JSCommand;
                    mycommand.Should().NotBeNull();
                    mycommand.ToString().Should().Be("{}");
                    mycommand.Type.Should().Be(JSCSGlueType.Command);
                    mycommand.MappedJSValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Command()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new ViewModelTest() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute"));
                    Thread.Sleep(100);
                    command.Received().Execute(Arg.Any<object>());
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Command_With_Parameter()
        {
            var command = Substitute.For<ICommand>();
            var datacontexttest = new ViewModelTest() { Command = command };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;
                    CefV8Value mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute", js));
                    Thread.Sleep(100);
                    command.Received().Execute(datacontexttest);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Command_CanExecute_False()
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

                    CefV8Value mycommand = GetAttribute(js, "Command");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");

                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Command_CanExecute_True()
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

                    CefV8Value mycommand = GetAttribute(js, "Command");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");

                    res.Should().BeTrue();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Command_Uptate_From_Null()
        {
            var command = Substitute.For<ICommand>();
            command.CanExecute(Arg.Any<object>()).Returns(true);
            var datacontexttest = new ViewModelTest();

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value mycommand = GetAttribute(js, "Command");
                    mycommand.IsNull.Should().BeTrue();

                    DoSafe(() => datacontexttest.Command = command);
                    Thread.Sleep(200);

                    mycommand = GetAttribute(js, "Command");
                    DoSafe(() => Call(mycommand, "Execute", js));
                    Thread.Sleep(100);
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
        public async Task Test_HTMLBinding_Basic_TwoWay_SimpleCommand_Without_Parameter()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontexttest = new ViewModelSimpleCommandTest() { SimpleCommand = command };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;
                    CefV8Value mycommand = GetAttribute(js, "SimpleCommand");
                    DoSafe(() => Call(mycommand, "Execute"));
                    Thread.Sleep(100);
                    command.Received().Execute(null);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_SimpleCommand_With_Parameter()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontexttest = new ViewModelSimpleCommandTest() { SimpleCommand = command };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;
                    CefV8Value mycommand = GetAttribute(js, "SimpleCommand");
                    DoSafe(() => Call(mycommand, "Execute", js));
                    Thread.Sleep(100);
                    command.Received().Execute(datacontexttest);
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_SimpleCommand_Name()
        {
            var command = Substitute.For<ISimpleCommand>();
            var datacontexttest = new ViewModelSimpleCommandTest() { SimpleCommand = command };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontexttest, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {

                    mb.ToString().Should().Be(@"{""SimpleCommand"":{}}");

                    var js = (mb as HTML_Binding).JSBrideRootObject as JSGenericObject;

                    var mysimplecommand = js.Attributes["SimpleCommand"] as JSSimpleCommand;
                    mysimplecommand.Should().NotBeNull();
                    mysimplecommand.ToString().Should().Be("{}");
                    mysimplecommand.Type.Should().Be(JSCSGlueType.SimpleCommand);
                    mysimplecommand.MappedJSValue.Should().NotBeNull();
                }
            };

            await RunAsync(test);
        }



        #endregion

        private void CheckIntValue(CefV8Value js, string pn, int value)
        {
            CefV8Value res = GetAttribute(js, pn);
            res.Should().NotBeNull();
            res.IsInt.Should().BeTrue();
            res.GetIntValue().Should().Be(0);
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_CLR_Type_FromCtojavascript()
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

                    //CheckIntValue(js, "Char", 0);
                    CheckIntValue(js, "Double", 0);
                    CheckIntValue(js, "Decimal", 0);
                    CheckIntValue(js, "Float", 0);
                }
            };

            await RunAsync(test);
        }


        private void SetValue(CefV8Value js, string pn, CefV8Value value)
        {
            this.Call(js, pn, value);
            //DoSafe(() => js.Invoke(pn, value));
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_CLR_Type_FromjavascripttoCto()
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

                    SetValue(js, "int64", CefV8Value.CreateInt(32));
                    Thread.Sleep(200);
                    datacontext.int64.Should().Be(32);

                    SetValue(js, "uint64", CefV8Value.CreateInt(456));
                    Thread.Sleep(200);
                    datacontext.uint64.Should().Be(456);

                    SetValue(js, "int32", CefV8Value.CreateInt(5));
                    Thread.Sleep(200);
                    datacontext.int32.Should().Be(5);

                    SetValue(js, "uint32", CefV8Value.CreateInt(67));
                    Thread.Sleep(200);
                    datacontext.uint32.Should().Be(67);

                    SetValue(js, "int16", CefV8Value.CreateInt(-23));
                    Thread.Sleep(200);
                    datacontext.int16.Should().Be(-23);

                    SetValue(js, "uint16", CefV8Value.CreateInt(9));
                    Thread.Sleep(200);
                    datacontext.uint16.Should().Be(9);

                    SetValue(js, "Float", CefV8Value.CreateDouble(888.78));
                    Thread.Sleep(200);
                    datacontext.Float.Should().Be(888.78f);

                    //SetValue(js, "Char", 128);
                    //Thread.Sleep(200);
                    //datacontext.Char.Should().Be((char)128);

                    SetValue(js, "Double", CefV8Value.CreateDouble(866.76));
                    Thread.Sleep(200);
                    datacontext.Double.Should().Be(866.76);

                    SetValue(js, "Decimal", CefV8Value.CreateDouble(0.5));
                    Thread.Sleep(200);
                    datacontext.Decimal.Should().Be(0.5m);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Command_CanExecute_Refresh_Ok()
        {
            bool canexecute = true;
            _ICommand.CanExecute(Arg.Any<object>()).ReturnsForAnyArgs(x => canexecute);


            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;
                    CefV8Value mycommand = GetAttribute(js, "TestCommand");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    canexecute = false;
                    _ICommand.CanExecuteChanged += Raise.EventWith(_ICommand, new EventArgs());

                    Thread.Sleep(100);

                    mycommand = GetAttribute(js, "TestCommand");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    ((bool)res).Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Command_CanExecute_Refresh_Ok_Argument()
        {
            bool canexecute = true;
            _ICommand.CanExecute(Arg.Any<object>()).ReturnsForAnyArgs(x => canexecute);


            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value mycommand = GetAttribute(js, "TestCommand");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeTrue();

                    _ICommand.Received().CanExecute(_DataContext);

                    canexecute = false;
                    _ICommand.ClearReceivedCalls();

                    _ICommand.CanExecuteChanged += Raise.EventWith(_ICommand, new EventArgs());

                    Thread.Sleep(100);

                    _ICommand.Received().CanExecute(_DataContext);

                    mycommand = GetAttribute(js, "TestCommand");
                    res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Command_CanExecute_Refresh_Ok_Argument_Exception()
        {
            _ICommand.CanExecute(Arg.Any<object>()).Returns(x => { if (x[0] == null) throw new Exception(); return false; });

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    _ICommand.Received().CanExecute(Arg.Any<object>());
                    var js = mb.JSRootObject;

                    CefV8Value mycommand = GetAttribute(js, "TestCommand");
                    bool res = GetBoolAttribute(mycommand, "CanExecuteValue");
                    res.Should().BeFalse();

                    _ICommand.Received().CanExecute(_DataContext);
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Command_Received_javascript_variable()
        {
            _ICommand.CanExecute(Arg.Any<object>()).Returns(true);

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value mycommand = GetAttribute(js, "TestCommand");
                    Call(mycommand, "Execute", CefV8Value.CreateString("titi"));

                    Thread.Sleep(150);
                    _ICommand.Received().Execute("titi");
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Command_Complete()
        {
            _ICommand = new RelayCommand(() =>
                {
                    _DataContext.MainSkill = new Skill();
                    _DataContext.Skills.Add(_DataContext.MainSkill);
                });
            _DataContext.TestCommand = _ICommand;

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    _DataContext.Skills.Should().HaveCount(2);

                    DoSafe(() =>
                    _ICommand.Execute(null));

                    Thread.Sleep(100);

                    CefV8Value res = GetSafe(() =>
                        js.Invoke("Skills", this._WebView).ExecuteFunction());

                    res.Should().NotBeNull();
                    res.GetArrayLength().Should().Be(3);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Command_With_Null_Parameter()
        {
            var command = Substitute.For<ICommand>();
            var test = new ViewModelTest() { Command = command };

            var testR = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, test, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value mycommand = GetAttribute(js, "Command");
                    Call(mycommand, "Execute", CefV8Value.CreateNull());

                    Thread.Sleep(150);
                    command.Received().Execute(null);
                }
            };

            await RunAsync(testR);
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_ResultCommand_Should_have_ToString()
        {
            var command = Substitute.For<ICommand>();
            var test = new ViewModelTest() { Command = command };

            var testR = new TestInContext()
            {
                Path = @"javascript\index_promise.html",
                Bind = (win) => HTML_Binding.Bind(win, test, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    mb.ToString().Should().NotBeNull();
                }
            };

            await RunAsync(testR);
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_ResultCommand_Received_javascript_variable_and_not_crash_withoutcallback()
        {
            var function = NSubstitute.Substitute.For<Func<int, int>>();
            var dc = new FakeFactory<int, int>(function);

            var test = new TestInContext()
            {
                Path = @"javascript\index_promise.html",
                Bind = (win) => HTML_Binding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;
                    CefV8Value mycommand = GetAttribute(js, "CreateObject");
                    Call(mycommand, "Execute", CefV8Value.CreateInt(25));

                    Thread.Sleep(700);
                    function.Received(1).Invoke(25);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_ResultCommand_Received_javascript_variable()
        {
            var function = NSubstitute.Substitute.For<Func<int, int>>();
            function.Invoke(Arg.Any<int>()).Returns(255);
            var dc = new FakeFactory<int, int>(function);

            var test = new TestInContext()
            {
                Path = @"javascript\index_promise.html",
                Bind = (win) => HTML_Binding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                { 
                  
                    {
                        var glueobj = (mb as HTML_Binding).JSBrideRootObject as JSGenericObject;
                        var mysimplecommand = glueobj.Attributes["CreateObject"] as JSResultCommand;
                        mysimplecommand.Should().NotBeNull();
                        mysimplecommand.ToString().Should().Be("{}");
                        mysimplecommand.Type.Should().Be(JSCSGlueType.ResultCommand);
                        mysimplecommand.MappedJSValue.Should().NotBeNull();
                    }
                   
                    var js = mb.JSRootObject;
                    CefV8Value mycommand = GetAttribute(js, "CreateObject");
                    //CefV8Exception ex = null;
                    CefV8Value cb = null;
                    bool res = _WebView.Eval("(function(){return { fullfill: function (res) {window.res=res; }, reject: function(err){window.err=err;}}; })();", out cb);

                    res.Should().BeTrue();
                    //ex.Should().BeNull();
                    cb.Should().NotBeNull();
                    cb.IsObject.Should().BeTrue();

                    CefV8Value resdummy = this.CallWithRes(mycommand, "Execute", CefV8Value.CreateInt(25), cb);

                    Thread.Sleep(100);
                    function.Received(1).Invoke(25);
                },
                Then = (mb) =>
                   {
                       var error = _WebView.GetGlobal().GetValue("err");
                       error.IsUndefined.Should().BeTrue();

                       CefV8Value resvalue = _WebView.GetGlobal().GetValue("res");
                       int intres = resvalue.GetIntValue();
                       intres.Should().Be(255);


                   }
            };

            await RunAsync(test);
        }



        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_ResultCommand_Received_javascript_variable_should_fault_Onexception()
        {
            string errormessage = "original error message";
            var function = NSubstitute.Substitute.For<Func<int, int>>();
            function.When(f => f.Invoke(Arg.Any<int>())).Do(_ => { throw new Exception(errormessage); });
            var dc = new FakeFactory<int, int>(function);

            var test = new TestInContext()
            {
                Path = @"javascript\index_promise.html",
                Bind = (win) => HTML_Binding.Bind(win, dc, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;


                    CefV8Value mycommand = GetAttribute(js, "CreateObject");
                    CefV8Exception ex = null;
                    CefV8Value cb = null;
                    bool res = _WebView.Eval("(function(){return { fullfill: function (res) {window.res=res; }, reject: function(err){window.err=err;}}; })();", out cb);

                    res.Should().BeTrue();
                    ex.Should().BeNull();
                    cb.Should().NotBeNull();
                    cb.IsObject.Should().BeTrue();

                    CefV8Value resdummy = this.CallWithRes(mycommand, "Execute", CefV8Value.CreateInt(25), cb);
                    Thread.Sleep(200);
                    function.Received(1).Invoke(25);
                },
                Then = (mb) =>
                {
                    var error = _WebView.GetGlobal().GetValue("err").GetStringValue();
                    error.Should().Be(errormessage);

                    CefV8Value resvalue = _WebView.GetGlobal().GetValue("res");
                    resvalue.IsUndefined.Should().BeTrue();
                }
            };

            await RunAsync(test);
        }

        private CefV8Value UnWrapCollection(CefV8Value root, string att)
        {
            return root.Invoke(att, this._WebView).ExecuteFunction();
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Collection()
        {

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value col = UnWrapCollection(js, "Skills");
                    col.Should().NotBeNull();
                    col.GetArrayLength().Should().Be(2);

                    Check(col, _DataContext.Skills);

                    _DataContext.Skills.Add(new Skill() { Name = "C++", Type = "Info" });

                    Thread.Sleep(100);
                    col = UnWrapCollection(js, "Skills");
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);


                    _DataContext.Skills.Insert(0, new Skill() { Name = "C#", Type = "Info" });
                    Thread.Sleep(100);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    _DataContext.Skills.RemoveAt(1);
                    Thread.Sleep(100);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    _DataContext.Skills[0] = new Skill() { Name = "HTML", Type = "Info" };
                    Thread.Sleep(100);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    _DataContext.Skills[0] = new Skill() { Name = "HTML5", Type = "Info" };
                    _DataContext.Skills.Insert(0, new Skill() { Name = "HTML5", Type = "Info" });
                    Thread.Sleep(100);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);


                    _DataContext.Skills.Clear();
                    Thread.Sleep(100);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Stress_TwoWay_Collection()
        {

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value col = GetSafe(() => UnWrapCollection(js, "Skills")); ;
                    col.GetArrayLength().Should().Be(2);

                    Check(col, _DataContext.Skills);

                    _DataContext.Skills.Add(new Skill() { Name = "C++", Type = "Info" });

                    Thread.Sleep(150);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    Check(col, _DataContext.Skills);

                    _DataContext.Skills[0] = new Skill() { Name = "HTML5", Type = "Info" };
                    int iis = 500;
                    for (int i = 0; i < iis; i++)
                    {
                        _DataContext.Skills.Insert(0, new Skill() { Name = "HTML5", Type = "Info" });
                    }

                    bool notok = true;
                    int tcount = _DataContext.Skills.Count;

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    Thread.Sleep(10);

                    while (notok)
                    {
                        col = GetSafe(() => UnWrapCollection(js, "Skills"));
                        notok = col.GetArrayLength() != tcount;
                    }
                    stopWatch.Stop();
                    var ts = stopWatch.ElapsedMilliseconds;

                    Console.WriteLine("Perf: {0} sec for {1} items", ((double)(ts)) / 1000, iis);
                    Check(col, _DataContext.Skills);

                    //TimeSpan.FromMilliseconds(ts).Should().BeLessThan(TimeSpan.FromSeconds(4.5));
                    TimeSpan.FromMilliseconds(ts).Should().BeLessThan(TimeSpan.FromSeconds(4.7));

                }
            };

            await RunAsync(test);
        }




        private class TwoList
        {
            public TwoList()
            {
                L1 = new List<Skill>();
                L2 = new List<Skill>();
            }
            public List<Skill> L1 { get; private set; }
            public List<Skill> L2 { get; private set; }
        }

        [Fact]
        public Task Test_HTMLBinding_Stress_TwoWay_Collection_CreateBinding()
        {
            return Test_HTMLBinding_Stress_Collection_CreateBinding(JavascriptBindingMode.TwoWay, 1.5, "javascript/simple.html");
        }

        [Fact]
        public Task Test_HTMLBinding_Stress_OneWay_Collection_CreateBinding()
        {
            return Test_HTMLBinding_Stress_Collection_CreateBinding(JavascriptBindingMode.OneWay, 1.5, "javascript/simple.html");
        }
        public Task Test_HTMLBinding_Stress_Collection_CreateBinding(JavascriptBindingMode imode, double excpected, string ipath = null)
        {
            int r = 100;
            var datacontext = new TwoList();
            datacontext.L1.AddRange(Enumerable.Range(0, r).Select(i => new Skill()));

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var test = new TestInContext()
            {
                Path = ipath,
                Bind = (win) => HTML_Binding.Bind(win, datacontext, imode),
                Test = (mb) =>
                {
                    stopWatch.Stop();
                    long ts = stopWatch.ElapsedMilliseconds;

                    Console.WriteLine("Perf: {0} sec for {1} items", ((double)(ts)) / 1000, r);

                    var js = mb.JSRootObject;

                    CefV8Value col = GetSafe(() => UnWrapCollection(js, "L1"));
                    col.GetArrayLength().Should().Be(r);

                    TimeSpan.FromMilliseconds(ts).Should().BeLessThan(TimeSpan.FromSeconds(excpected));
                }
            };
            return RunAsync(test);

        }


        [Fact]
        public async Task Test_HTMLBinding_Stress_Collection_Update_From_Javascript()
        {
            int r = 100;
            var datacontext = new TwoList();
            datacontext.L1.AddRange(Enumerable.Range(0, r).Select(i => new Skill()));


            var test = new TestInContext()
            {
                Path = "javascript/simple.html",
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value col1 = GetSafe(() => UnWrapCollection(js, "L1"));
                    col1.GetArrayLength().Should().Be(r);

                    CefV8Value col2 = GetSafe(() => UnWrapCollection(js, "L2"));
                    col2.GetArrayLength().Should().Be(0);

                    CefV8Value l2c = GetAttribute(js, "L2");
                    //(JSObject)GetSafe(() => js.Invoke("L2"));
                    l2c.Should().NotBeNull();

                    string javascript = "window.app = function(value,coll){var args = []; args.push(0);args.push(0);for (var i = 0; i < value.length; i++) { args.push(value[i]);} coll.splice.apply(coll, args);};";
                    CefV8Value res = null;
                    CefV8Exception ex = null;
                    bool ok = _WebView.Eval(javascript, out res);
                    ok.Should().BeTrue();
                    //JSObject win = null;
                    //win = GetSafe(() => (JSObject)_WebView.ExecuteJavascriptWithResult("window"));

                    bool notok = true;
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    DoSafe(() => Call(_WebView.GetGlobal(), "app", col1, l2c));
                    Thread.Sleep(100);
                    while (notok)
                    {
                        //Thread.Sleep(30);
                        notok = datacontext.L2.Count != r;
                    }
                    stopWatch.Stop();
                    long ts = stopWatch.ElapsedMilliseconds;

                    Console.WriteLine("Perf: {0} sec for {1} items", ((double)(ts)) / 1000, r);
                }
            };

            await RunAsync(test);
        }



        [Fact]
        public Task Test_HTMLBinding_Stress_OneTime_Collection_CreateBinding()
        {
            return Test_HTMLBinding_Stress_Collection_CreateBinding(JavascriptBindingMode.OneTime, 1.5, "javascript/simple.html");
        }

        [Fact]
        public async Task Test_HTMLBinding_Stress_TwoWay_Int()
        {


            var test = new TestInContext()
            {
                Path = "javascript/simple.html",
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;
                    int iis = 500;
                    for (int i = 0; i < iis; i++)
                    {
                        _DataContext.Age += 1;
                    }

                    bool notok = true;
                    var tg = _DataContext.Age;
                    Thread.Sleep(700);

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    while (notok)
                    {
                        Thread.Sleep(100);
                        int doublev = GetIntAttribute(js, "Age");
                        notok = doublev != tg;
                    }
                    stopWatch.Stop();
                    var ts = stopWatch.ElapsedMilliseconds;

                    Console.WriteLine("Perf: {0} sec for {1} iterations", ((double)(ts)) / 1000, iis);

                    TimeSpan.FromMilliseconds(ts).Should().BeLessOrEqualTo(TimeSpan.FromSeconds(3.1));

                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Collection_FromJSUpdate()
        {


            var test = new TestInContext()
            {
                Path = "javascript/simple.html",
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var root = (mb as HTML_Binding).JSBrideRootObject as JSGenericObject;
                    var js = mb.JSRootObject;

                    CefV8Value col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    col.GetArrayLength().Should().Be(2);

                    Check(col, _DataContext.Skills);

                    CefV8Value coll = GetAttribute(js, "Skills");
                    Call(coll, "push", (root.Attributes["Skills"] as JSArray).Items[0].GetJSSessionValue());

                    Thread.Sleep(5000);
                    _DataContext.Skills.Should().HaveCount(3);
                    _DataContext.Skills[2].Should().Be(_DataContext.Skills[0]);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    Check(col, _DataContext.Skills);

                    Call(coll, "pop");

                    Thread.Sleep(100);
                    _DataContext.Skills.Should().HaveCount(2);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    Call(coll, "shift");

                    Thread.Sleep(100);
                    _DataContext.Skills.Should().HaveCount(1);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    Check(col, _DataContext.Skills);


                    Call(coll, "unshift",
                          (root.Attributes["Skills"] as JSArray).Items[0].GetJSSessionValue());

                    Thread.Sleep(150);
                    _DataContext.Skills.Should().HaveCount(2);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    Check(col, _DataContext.Skills);

                    _DataContext.Skills.Add(new Skill() { Type = "Langage", Name = "French" });
                    Thread.Sleep(150);
                    _DataContext.Skills.Should().HaveCount(3);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    col.Should().NotBeNull();
                    Check(col, _DataContext.Skills);

                    Call(coll, "reverse");

                    Thread.Sleep(150);
                    _DataContext.Skills.Should().HaveCount(3);
                    col = GetSafe(() => UnWrapCollection(js, "Skills"));
                    Check(col, _DataContext.Skills);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Collection_JSUpdate_Should_Survive_ViewChanges()
        {


            var test = new TestInContext()
            {
                Path = "javascript/simple.html",
                Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var root = (mb as HTML_Binding).JSBrideRootObject as JSGenericObject;
                    var js = mb.JSRootObject;

                    CefV8Value col = UnWrapCollection(js, "Skills");
                    col.GetArrayLength().Should().Be(2);

                    Check(col, _DataContext.Skills);

                    CefV8Value coll = GetAttribute(js, "Skills");
                    Call(coll, "push", CefV8Value.CreateString("Whatever"));

                    Thread.Sleep(150);
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


        private void Checkstring(CefV8Value coll, IList<string> iskill)
        {
            coll.GetArrayLength().Should().Be(iskill.Count);

            for (int i = 0; i < iskill.Count; i++)
            {
                var c = coll.GetValue(i).GetStringValue();
                c.Should().Be(iskill[i]);
            }
        }

        private void Checkdecimal(CefV8Value coll, IList<decimal> iskill)
        {
            coll.GetArrayLength().Should().Be(iskill.Count);

            for (int i = 0; i < iskill.Count; i++)
            {
                var c = (decimal)coll.GetValue(i).GetDoubleValue();
                c.Should().Be(iskill[i]);
            }
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Decimal_ShouldOK()
        {
            var datacontext = new VMwithdecimal();

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    int res = GetIntAttribute(js, "decimalValue");
                    res.Should().Be(0);

                    Call(js, "decimalValue", CefV8Value.CreateDouble(0.5));

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
        public async Task Test_HTMLBinding_Basic_TwoWay_Long_ShouldOK()
        {
            var datacontext = new VMwithlong() { longValue = 45 };

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;
                    var doublev = GetDoubleAttribute(js, "longValue");
                    doublev.Should().Be(45);

                    Call(js, "longValue", CefV8Value.CreateInt(24524));
                    Thread.Sleep(100);

                    datacontext.longValue.Should().Be(24524);

                    doublev = GetDoubleAttribute(js, "longValue");
                    long half = 24524;
                    doublev.Should().Be(half);
                }
            };

            await RunAsync(test);
        }



        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Collection_string()
        {
            var datacontext = new VMWithList<string>();

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value col = GetSafe(() => UnWrapCollection(js, "List"));
                    col.GetArrayLength().Should().Be(0);

                    Checkstring(col, datacontext.List);

                    datacontext.List.Add("titi");

                    Thread.Sleep(100);
                    col = GetSafe(() => UnWrapCollection(js, "List"));

                    Checkstring(col, datacontext.List);

                    datacontext.List.Add("kiki");
                    datacontext.List.Add("toto");

                    Thread.Sleep(100);
                    col = GetSafe(() => UnWrapCollection(js, "List"));

                    Checkstring(col, datacontext.List);

                    Thread.Sleep(100);
                    col = GetSafe(() => UnWrapCollection(js, "List"));

                    Checkstring(col, datacontext.List);

                    var comp = new List<string>(datacontext.List);
                    comp.Add("newvalue");

                    col = GetSafe(() => UnWrapCollection(js, "List"));
                    var chcol = GetAttribute(js, "List");
                    Call(chcol, "push", CefV8Value.CreateString("newvalue"));

                    Thread.Sleep(350);

                    col = GetSafe(() => UnWrapCollection(js, "List"));

                    datacontext.List.Should().Equal(comp);
                    Checkstring(col, datacontext.List);

                    datacontext.List.Clear();
                    Thread.Sleep(100);
                    col = GetSafe(() => UnWrapCollection(js, "List"));

                    Checkstring(col, datacontext.List);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Collection_should_be_observable_attribute()
        {
            var datacontext = new ChangingCollectionViewModel();

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value col = GetSafe(() => UnWrapCollection(js, "Items"));
                    col.GetArrayLength().Should().NotBe(0);


                    DoSafe(() => datacontext.Replace.Execute(null));

                    datacontext.Items.Should().BeEmpty();

                    Thread.Sleep(300);
                    col = GetSafe(() => UnWrapCollection(js, "Items"));
                    col.GetArrayLength().Should().Be(0);

                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Collection_NoneGenericList()
        {
            var datacontext = new VMWithListNonGeneric();
            datacontext.List.Add(888);

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value col = GetSafe(() => UnWrapCollection(js, "List"));
                    col.GetArrayLength().Should().Be(1);

                    CefV8Value res = GetAttribute(js, "List");
                    Call(res, "push", CefV8Value.CreateString("newvalue"));

                    col = GetSafe(() => UnWrapCollection(js, "List"));
                    col.GetArrayLength().Should().Be(2);

                    Thread.Sleep(350);

                    datacontext.List.Should().HaveCount(2);
                    datacontext.List[1].Should().Be("newvalue");
                }
            };

            await RunAsync(test);
        }


        [Fact]
        public async Task Test_HTMLBinding_Basic_TwoWay_Collection_decimal()
        {
            var datacontext = new VMWithList<decimal>();

            var test = new TestInContext()
            {
                Bind = (win) => HTML_Binding.Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;

                    CefV8Value col = GetSafe(() => UnWrapCollection(js, "List"));
                    col.GetArrayLength().Should().Be(0);

                    Checkdecimal(col, datacontext.List);

                    datacontext.List.Add(3);

                    Thread.Sleep(150);
                    col = GetSafe(() => UnWrapCollection(js, "List"));

                    Checkdecimal(col, datacontext.List);

                    datacontext.List.Add(10.5m);
                    datacontext.List.Add(126);

                    Thread.Sleep(150);
                    col = GetSafe(() => UnWrapCollection(js, "List"));

                    Checkdecimal(col, datacontext.List);

                    Thread.Sleep(100);
                    col = GetSafe(() => UnWrapCollection(js, "List"));

                    Checkdecimal(col, datacontext.List);

                    var comp = new List<decimal>(datacontext.List);
                    comp.Add(0.55m);

                    CefV8Value res = GetAttribute(js, "List");
                    Call(res, "push", CefV8Value.CreateDouble(0.55));

                    Thread.Sleep(350);

                    col = GetSafe(() => UnWrapCollection(js, "List"));

                    comp.Should().Equal(datacontext.List);
                    Checkdecimal(col, datacontext.List);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task Test_HTMLBinding_stringBinding()
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


                    string res4 = CallWithRes(GetAttribute(js, "Local"), "City").GetStringValue();
                    res4.Should().Be("Florianopolis");

                    CefV8Value res45 = UnWrapCollection(js, "Skills");
                    string res5 = CallWithRes(res45.GetValue(0), "Name").GetStringValue();
                    res5.Should().Be("Langage");
                }
            };

            await RunAsync(test);
        }


   



        //[Fact]
        //public async Task Test_HTMLBinding_BasicAlreadyLoaded_OneWay()
        //{
        //    var datacontext = new VMWithList<decimal>();

        //    var test = new TestInContext()
        //    {
        //        Bind = (win) => HTML_Binding.Bind(win, _DataContext, JavascriptBindingMode.OneWay),
        //        Test = (mb) =>
        //        {
        //            mb.Should().NotBeNull();
        //        }
        //    };

        //    await RunAsync(test);
        //}




        [Fact]
        public async Task Test_HTMLBinding_Basic_HTML_Without_Correct_js_ShouldThrowException_2()
        {
            using (Tester("javascript/almost_empty.html"))
            {
                var vm = new object();
                MVVMCEFGlueException ex = null;

                try
                {
                    await HTML_Binding.Bind(_ICefGlueWindow, vm, JavascriptBindingMode.OneTime);
                }
                catch (MVVMCEFGlueException myex)
                {
                    ex = myex;
                }

                ex.Should().NotBeNull();
            }
        }
    }
}

