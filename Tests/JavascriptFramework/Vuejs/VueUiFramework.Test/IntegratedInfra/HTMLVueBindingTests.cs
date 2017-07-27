using FluentAssertions;
using Neutronium.Core;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Universal.HTMLBindingTests;
using Tests.Universal.HTMLBindingTests.Helper;
using Xunit;
using Xunit.Abstractions;

namespace VueFramework.Test.IntegratedInfra
{
    public abstract class HTMLVueBindingTests : HTMLBindingTests
    {
        public HTMLVueBindingTests(IWindowLessHTMLEngineProvider context, ITestOutputHelper output)
            : base(context, output)
        {
        }

        private class ReadOnlyClass : ViewModelTestBase
        {
            private int _ReadOnly;
            public int ReadOnly 
            {
                get { return _ReadOnly; }
                private set { Set(ref _ReadOnly, value); }
            }

            public void SetReadOnly(int newValue) => ReadOnly = newValue;
        }

        private class ReadWriteClass : ReadOnlyClass
        {
            private int _ReadWrite;
            public int ReadWrite 
            {
                get { return _ReadWrite; }
                set { Set(ref _ReadWrite, value); }
            }
        }
        public static IEnumerable<object> ReadWriteTestData 
        {
            get 
            {
                yield return new object[] { typeof(ReadOnlyClass), true };
                yield return new object[] { typeof(ReadWriteClass), false };
            }
        }

        [Theory]
        [MemberData(nameof(ReadWriteTestData))]
        public async Task TwoWay_should_create_listener_only_for_write_property(Type type, bool readOnly)
        {
            var datacontext = Activator.CreateInstance(type);

            var test = new TestInContext()
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = (mb) =>
                {
                    var js = mb.JSRootObject;
                    var isReadOnly = GetBoolAttribute(js, NeutroniumConstants.ReadOnlyFlag);
                    isReadOnly.Should().Be(readOnly);

                    var silenter = GetAttribute(js, "__silenter");
                    if (isReadOnly)
                    {
                        silenter.IsUndefined.Should().Be(true);
                    }
                    else
                    {
                        silenter.IsObject.Should().Be(true);
                    }
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(ReadWriteTestData))]
        public async Task TwoWay_should_update_from_csharp_readonly_property(Type type, bool readOnly)
        {
            var datacontext = Activator.CreateInstance(type) as ReadOnlyClass;

            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;
                    var newValue = 55;
                    DoSafeUI(() => datacontext.SetReadOnly(newValue));
                    
                    await Task.Delay(150);
                    var readOnlyValue = GetIntAttribute(js, "ReadOnly");

                    readOnlyValue.Should().Be(newValue);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_should_update_from_csharp_readwrite_property()
        {
            var datacontext = new ReadWriteClass();

            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;
                    var newValue = 550;
                    DoSafeUI(() => datacontext.ReadWrite = newValue);

                    await Task.Delay(150);
                    var readOnlyValue = GetIntAttribute(js, "ReadWrite");

                    readOnlyValue.Should().Be(newValue);
                }
            };

            await RunAsync(test);
        }

        [Fact]
        public async Task TwoWay_should_update_from_js_readwrite_property()
        {
            var datacontext = new ReadWriteClass();

            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;
                    var newValue = 1200;
                    var jsValue = _WebView.Factory.CreateInt(newValue);
                    SetAttribute(js, "ReadWrite", jsValue);
                    await Task.Delay(150);

                    DoSafeUI(() => datacontext.ReadWrite.Should().Be(newValue));
                }
            };

            await RunAsync(test);
        }

        [Theory]
        [MemberData(nameof(BasicVmData))]
        public async Task TwoWay_should_clean_javascriptObject_listeners_when_object_is_not_part_of_the_graph(BasicVm remplacementChild)
        {
            var datacontext = new BasicFatherVm();
            var child = new BasicVm();
            datacontext.Child = child;

            var test = new TestInContextAsync()
            {
                Bind = (win) => Bind(win, datacontext, JavascriptBindingMode.TwoWay),
                Test = async (mb) =>
                {
                    var js = mb.JSRootObject;
                    var childJs = GetAttribute(js, "Child");

                    var silenterBeforeClean = GetAttribute(childJs, "__silenter");
                    silenterBeforeClean.IsObject.Should().Be(true);

                    DoSafeUI(() => datacontext.Child = remplacementChild);
                    await Task.Delay(150);

                    await Task.Delay(150);

                    var silenterAfterClean = GetAttribute(childJs, "__silenter");
                    silenterAfterClean.IsUndefined.Should().Be(true);
                }
            };

            await RunAsync(test);
        }
    }
}
