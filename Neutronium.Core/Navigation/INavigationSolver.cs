using System;
using System.Threading.Tasks;

namespace  Neutronium.Core.Navigation
{
    /// <summary>
    /// interface used to navigate beetween ViewModel type
    /// </summary>
    public interface INavigationSolver : IDisposable
    {
        /// <summary>
        /// if true navigation solver will try to set Navigation reference
        /// on view model implementing <see cref="INavigable"/> 
        /// </summary>
        bool UseINavigable { get; set; }


        /// <summary>
        /// Navigate to the view corresponding the specified viewModel <seealso cref="INavigationBuilder"/> 
        /// </summary>
        /// <param name="viewModel">
        /// ViewModel to bring to view
        /// </param>
        /// <param name="id">
        /// Optional id information, can be used if same type can resolve to different 
        /// view depending on context <seealso cref="INavigationBuilder"/>
        /// </param>
        /// <param name="mode">
        /// Binding mode
        /// </param>
        Task<IHTMLBinding> NavigateAsync(object viewModel, string id = null, JavascriptBindingMode mode = JavascriptBindingMode.TwoWay);

        /// <summary>
        /// Event fired when navigation ended <seealso cref="NavigationEvent"/> 
        /// </summary>
        event EventHandler<NavigationEvent> OnNavigate;

        /// <summary>
        /// Event fired after navigation and entering animation ended<seealso cref="DisplayEvent"/> 
        /// </summary>
        event EventHandler<DisplayEvent> OnDisplay;

        /// <summary>
        /// Event fired on first load
        /// </summary>
        event EventHandler OnFirstLoad;
    }
}
