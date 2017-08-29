using System.Threading.Tasks;
using Neutronium.Core;
using Neutronium.Core.Navigation;
using Neutronium.WPF.Internal;
using System;

namespace Neutronium.WPF
{
    public class HTMLWindow : HTMLControlBase, INavigationSolver
    {
        private static int _count = 0;
        public override string UniqueName { get; } = $"HTML Window {_count++}";

        public HTMLWindow() : this(new NavigationBuilder())
        {
        }

        public HTMLWindow(IUrlSolver urlSolver) : base(urlSolver)
        {
            NavigationBuilder = urlSolver as INavigationBuilder;
        }
      
        public INavigationBuilder NavigationBuilder { get; }

        public Task<IHtmlBinding> NavigateAsync(object viewModel, string id = null, JavascriptBindingMode mode = JavascriptBindingMode.TwoWay)
        {
            if (this.IsInitialized)
            {
                return NavigateAsyncBase(viewModel, id, mode);
            }

            var taskCompletion = new TaskCompletionSource<IHtmlBinding>();
            this.Initialized += (o,e) => HTMLWindow_Initialized(taskCompletion, viewModel, id, mode);
            return taskCompletion.Task;
        }

        private async void HTMLWindow_Initialized(TaskCompletionSource<IHtmlBinding> taskCompletion, object viewModel, string id, JavascriptBindingMode mode)
        {
            try
            {
                var res = await NavigateAsyncBase(viewModel, id, mode);
                taskCompletion.TrySetResult(res);
            }
            catch(Exception ex)
            {
                taskCompletion.SetException(ex);
            }       
        }
    }
}
