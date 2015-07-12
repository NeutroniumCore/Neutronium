using System;
using Xilium.CefGlue.Helpers.Log;

namespace Xilium.CefGlue.WPF
{
    internal sealed class WpfCefClient : CefClient
    {
        private WpfCefBrowser _owner;

        private WpfCefLifeSpanHandler _lifeSpanHandler;
        private WpfCefDisplayHandler _displayHandler;
        private WpfCefRenderHandler _renderHandler;
        private WpfCefLoadHandler _loadHandler;
        private CefNoContextMenuHandler _CefNoContextMenuHandler;

        public WpfCefClient(WpfCefBrowser owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
       
            _lifeSpanHandler = new WpfCefLifeSpanHandler(owner);
            _displayHandler = new WpfCefDisplayHandler(owner);
            _CefNoContextMenuHandler = new CefNoContextMenuHandler();
            _renderHandler = new WpfCefRenderHandler(owner, new NLogLogger("WpfCefRenderHandler"), new UiHelper(new NLogLogger("WpfCefRenderHandler")));
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
