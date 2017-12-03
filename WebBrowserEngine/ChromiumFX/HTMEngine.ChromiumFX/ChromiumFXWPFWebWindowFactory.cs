using System;
using Chromium;
using Chromium.WebBrowser;
using Neutronium.Core;
using Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding;
using Neutronium.WebBrowserEngine.ChromiumFx.Session;
using Neutronium.WPF;
using Chromium.Event;

namespace Neutronium.WebBrowserEngine.ChromiumFx
{
    public class ChromiumFXWPFWebWindowFactory : IWPFWebWindowFactory 
    {
        private readonly ChromiumFxSession _Session;

        public CfxSettings Settings { get; private set; }
        public string EngineName => "Chromium";
        public bool IsModern => true;
        public string EngineVersion => CfxRuntime.GetChromeVersion();
        public string Name => "ChromiumFX";
        public CfxBrowserSettings BrtowserSettings => ChromiumWebBrowser.DefaultBrowserSettings;
        public IWebSessionLogger WebSessionLogger { get; set; }

        public ChromiumFXWPFWebWindowFactory(Action<CfxSettings> settingsUpdater=null, Action<CfxOnBeforeCommandLineProcessingEventArgs> commadLineHandler=null)
        {
            _Session = ChromiumFxSession.GetSession((settings) => 
            {
                settingsUpdater?.Invoke(settings);
                Settings = settings;
            }, commadLineHandler, WebSessionLogger);
        }

        public IWPFWebWindow Create()
        {
            return new ChromiumFxWpfWindow(WebSessionLogger);
        }

        public void Dispose()
        {
            _Session.Dispose();
        }
    }
}
