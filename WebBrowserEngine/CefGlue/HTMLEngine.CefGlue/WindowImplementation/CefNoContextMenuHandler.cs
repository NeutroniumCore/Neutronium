using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.WindowImplementation
{
    class CefNoContextMenuHandler : CefContextMenuHandler
    {
        protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model)
        {
            model.Clear();
            //base.OnBeforeContextMenu(browser, frame, state, model);
        }
    }
}
