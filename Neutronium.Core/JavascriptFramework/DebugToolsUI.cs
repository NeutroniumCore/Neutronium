namespace Neutronium.Core.JavascriptFramework
{
    /// <summary>
    /// provide UI information about framework specific UI elements
    /// to be used by debug tools
    /// </summary>
    public class DebugToolsUI
    {
        /// <summary>
        /// return window information for toolbar HTML file if any
        /// null otherwise.
        /// </summary>
        public WindowInformation DebugToolbar { get; }

        /// <summary>
        /// return window information for about screen HTML file if any
        /// null otherwise.
        /// </summary>
        public WindowInformation About { get; }

        /// <summary>
        /// Build a DebugToolsUI with corresponding about and debug UI.
        /// </summary>
        /// <param name="debugToolbar"></param>
        /// <param name="about"></param>
        public DebugToolsUI(WindowInformation debugToolbar, WindowInformation about)
        {
            DebugToolbar = debugToolbar;
            About = about;
        }
    }
}
