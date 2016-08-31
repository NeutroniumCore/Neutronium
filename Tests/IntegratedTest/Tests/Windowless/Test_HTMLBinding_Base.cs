using System.Collections.Generic;
using System.Windows.Input;
using FluentAssertions;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.ViewModel.Example;
using NSubstitute;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Xunit.Abstractions;

namespace IntegratedTest.Tests.Windowless
 {
    public class Test_HTMLBinding_Base : IntegratedTestBase 
    {
        protected readonly Person _DataContext;
        protected ICommand _ICommand;

        protected Test_HTMLBinding_Base(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
            _ICommand = Substitute.For<ICommand>();
            _DataContext = new Person(_ICommand) {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new Local() { City = "Florianopolis", Region = "SC" },
                PersonalState = PersonalState.Married
            };

            _DataContext.Skills.Add(new Skill() { Name = "Langage", Type = "French" });
            _DataContext.Skills.Add(new Skill() { Name = "Info", Type = "C++" });
        }

        protected void Check(IJavascriptObject coll, IList<Skill> iskill) 
        {
            coll.GetArrayLength().Should().Be(iskill.Count);

            for (int i = 0; i < iskill.Count; i++) 
            {
                var c = coll.GetValue(i);

                (GetSafe(() => GetStringAttribute(c, "Name"))).Should().Be(iskill[i].Name);
                (GetSafe(() => GetStringAttribute(c, "Type"))).Should().Be(iskill[i].Type);
            }
        }
    }
}
