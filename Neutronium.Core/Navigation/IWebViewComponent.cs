using System;
using System.Threading.Tasks;

namespace Neutronium.Core.Navigation
{
    /// <summary>
    /// WebView abstraction provided mainly to expose addictional API
    /// usefull on debug context.
    /// </summary>
    public interface IWebViewComponent
    {
        /// <summary>
        /// Reload the current page without changing the bindings, usefull on hot-reload context
        /// </summary>
        /// <returns></returns>
        Task ReloadAsync();

        /// <summary>
        /// Switch the current view using the provided target
        /// using the same binding.
        /// </summary>
        /// <param name="target">
        /// target uri
        /// </param>
        /// <returns></returns>
        Task SwitchViewAsync(Uri target);

        /// <summary>
        /// Execute javascript on the current web view.
        /// </summary>
        /// <param name="code"></param>
        void ExecuteJavascript(string code);
    }
}
