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
        public string Environment => CfxRuntime.PlatformArch.ToString();
        public CfxBrowserSettings BrowserSettings => ChromiumWebBrowser.DefaultBrowserSettings;
        public IWebSessionLogger WebSessionLogger { get; set; }

        public ChromiumFXWPFWebWindowFactory(IWebSessionLogger logger=null, Action<CfxSettings> settingsUpdater=null, Action<CfxOnBeforeCommandLineProcessingEventArgs> commadLineHandler=null)
        {
            WebSessionLogger = logger;
            _Session = ChromiumFxSession.GetSession(WebSessionLogger, (settings) => 
            {
                settingsUpdater?.Invoke(settings);
                Settings = settings;
            }, commadLineHandler);
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
