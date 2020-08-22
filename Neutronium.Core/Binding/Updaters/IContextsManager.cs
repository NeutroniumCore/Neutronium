using System;
using System.Threading.Tasks;

namespace Neutronium.Core.Binding.Updaters
{
    internal interface IContextsManager
    {
        bool isInUiContext { get; }
        void CheckUiContext();     
        void DispatchInJavascriptContext(Action action);
        void DispatchInUiContext(Action action);
        void DispatchInUiContextBindingPriority(Action action);
        Task<T> EvaluateOnUiContextAsync<T>(Func<T> compute);
        Task RunOnJavascriptContextAsync(Action execute);
    }
}
