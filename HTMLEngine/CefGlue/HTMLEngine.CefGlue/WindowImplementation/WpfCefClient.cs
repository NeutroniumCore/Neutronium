using System;
using HTMLEngine.CefGlue.WindowImplementation;
using Neutronium.WebBrowserEngine.CefGlue.Helpers.Log;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.WindowImplementation
{
    internal sealed class WpfCefClient : CefClient
    {
        private WpfCefBrowser _owner;

        private readonly WpfCefLifeSpanHandler _lifeSpanHandler;
        private readonly WpfCefDisplayHandler _displayHandler;
        private readonly WpfCefRenderHandler _renderHandler;
        private readonly WpfCefLoadHandler _loadHandler;
        private readonly CefNoContextMenuHandler _CefNoContextMenuHandler;

        public WpfCefClient(WpfCefBrowser owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
       
            _lifeSpanHandler = new WpfCefLifeSpanHandler(owner);
            _displayHandler = new WpfCefDisplayHandler(owner);
            _CefNoContextMenuHandler = new CefNoContextMenuHandler();
            _renderHandler = new WpfCefRenderHandler(owner, Logger.Log, new UiHelper(Logger.Log));
            _loadHandler = new WpfCefLoadHandler(owner);
        }

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _lifeSpanHandler;
        }

        protected override CefDisplayHandler GetDisplayHandler()
        {
            return _displayHandler;
        }

        protected override CefContextMenuHandler GetContextMenuHandler()
        {
            return _CefNoContextMenuHandler;
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return _renderHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _loadHandler;
        }
    }
}
