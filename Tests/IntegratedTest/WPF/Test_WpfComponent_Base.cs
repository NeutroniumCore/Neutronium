using System;
using System.Windows.Controls;
using HTML_WPF.Component;
using IntegratedTest.WPF.Infra;
using MVVM.HTML.Core.Navigation;
using Xunit;

namespace IntegratedTest.WPF
{
    public abstract class Test_WpfComponent_Base<T> : IClassFixture<WpfThread> where T : HTMLControlBase
    {
        private readonly IWindowTestEnvironment _WindowTestEnvironment;
        protected Test_WpfComponent_Base(IWindowTestEnvironment windowTestEnvironment, WpfThread wpfThread) 
        {
            _WindowTestEnvironment = windowTestEnvironment;
            windowTestEnvironment.WpfThread = wpfThread;
        }

        private WindowTest BuildWindow(Func<T> iWebControlFac, bool iManageLifeCycle)
        {
            return new WindowTest(_WindowTestEnvironment,
                (w) =>
                {
                    var stackPanel = new StackPanel();
                    w.Content = stackPanel;
                    var iWebControl = iWebControlFac();
                    if (iManageLifeCycle)
                        w.Closed += (o, e) => { iWebControl.Dispose(); };
                    stackPanel.Children.Add(iWebControl);
                });
        }

        protected abstract T GetNewHTMLControlBase(bool iDebug);

        internal void Test(Action<T, WindowTest> Test, bool iDebug = false, bool iManageLifeCycle = true)
        {
            AssemblyHelper.SetEntryAssembly();
            T wc1 = null;
            Func<T> iWebControlFac = () =>
            {
                return wc1 = GetNewHTMLControlBase(iDebug);
            };

            using (var wcontext = BuildWindow(iWebControlFac, iManageLifeCycle))
            {
                Test(wc1, wcontext);
            }
        }
    }
}
