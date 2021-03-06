﻿using FluentAssertions;
using Neutronium.Core;
using Neutronium.Core.Test.Helper;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Threading.Tasks;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Universal.HTMLBindingTests
{
    public abstract class BindingFreezeTests : HtmlBindingBase
    {
        protected BindingFreezeTests(IWindowLessHTMLEngineProvider context, ITestOutputHelper output)
            : base(context, output) { }

        private IJavascriptObject _IsFrozenFunction;
        private IJavascriptObject GetIsFrozenFunction()
        {
            if (_IsFrozenFunction != null)
                return _IsFrozenFunction;

            _WebView.Eval("(function(){return Object.isFrozen;})()", out _IsFrozenFunction);
            return _IsFrozenFunction;
        }

        private bool IsFrozen(IJavascriptObject javascriptObject)
        {
            return _WebView.Evaluate(() => GetIsFrozenFunction().ExecuteFunction(_WebView, null, javascriptObject).GetBoolValue());
        }

        [Theory]
        [InlineData(typeof(ReadOnly), true)]
        [InlineData(typeof(None), false)]
        [InlineData(typeof(Observable), false)]
        [InlineData(typeof(ReadOnlyObservable), false)]
        public async Task TwoWay_Freezes_Readonly_Objects(Type type, bool expectedIsFrozen)
        {
            var dataContext = Activator.CreateInstance(type);

            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;
                    var isFrozen = IsFrozen(js);
                    isFrozen.Should().Be(expectedIsFrozen);
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(typeof(ReadOnly), true)]
        [InlineData(typeof(None), false)]
        [InlineData(typeof(Observable), false)]
        [InlineData(typeof(ReadOnlyObservable), false)]
        public async Task TwoWay_Freezes_Readonly_Objects_Deep_Property(Type type, bool expectedIsFozen)
        {
            var child = (Observability)Activator.CreateInstance(type);
            var dataContext = new Composite(child);

            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;
                    var childJs = GetAttribute(js, "Child");
                    var isFrozen = IsFrozen(childJs);
                    isFrozen.Should().Be(expectedIsFozen);
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [InlineData(typeof(ReadOnly), true)]
        [InlineData(typeof(None), false)]
        [InlineData(typeof(Observable), false)]
        [InlineData(typeof(ReadOnlyObservable), false)]
        public async Task TwoWay_Freezes_Readonly_Objects_Deep_Array(Type type, bool expectedIsFrozen)
        {
            var child = (Observability)Activator.CreateInstance(type);
            var dataContext = new Composite(children: new[] { child });

            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JsRootObject;
                    var array = GetCollectionAttribute(js, "Children");
                    var childJs = array.GetArrayElements()[0];
                    var isFrozen = IsFrozen(childJs);
                    isFrozen.Should().Be(expectedIsFrozen);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_Objects_Inside_Frozen_Object_Are_Observable()
        {
            var child = new Observable
            {
                String = "value1"
            };
            var dataContext = new Composite(child);

            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, dataContext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JsRootObject;
                    var isFrozen = IsFrozen(js);
                    isFrozen.Should().BeTrue();

                    var childJs = GetAttribute(js, "Child");
                    await SetAttributeAsync(childJs, "String", _WebView.Factory.CreateString("value2"));

                    await DoSafeAsyncUI(() =>
                    {
                        child.String.Should().Be("value2");
                    });
                }
            };

            await RunAsync(test);
        }
    }
}
