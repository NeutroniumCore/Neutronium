using System;
using System.Collections.Generic;

namespace Neutronium.Core.WebBrowserEngine.Window
{
    /// <summary>
    /// HTML Window with extended capacity
    /// </summary>
    public interface IModernWebBrowserWindow : IWebBrowserWindow
    {
        /// <summary>
        /// event fired when script are ready to be executed
        /// but external javascript have not been executed yet
        /// </summary>
        event EventHandler<BeforeJavascriptExcecutionArgs> BeforeJavascriptExecuted;

        /// <summary>
        /// event fired when client browser try to load an HTML page
        /// this is useful when using webpack hot reloasd in order to plug correct
        /// binding
        /// </summary>
        event EventHandler<ClientReloadArgs> OnClientReload;


        /// <summary>
        /// Add a new item in contextmenu
        /// <param name="contextMenuItens">
        /// items to be added to the contextMenu
        /// </param>
        /// <returns>
        /// current browser
        /// </returns>
        /// </summary>
        IModernWebBrowserWindow RegisterContextMenuItem(IEnumerable<ContextMenuItem> contextMenuItens);
    }
}
