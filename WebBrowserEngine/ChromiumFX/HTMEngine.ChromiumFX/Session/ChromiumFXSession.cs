﻿using Chromium;
using Chromium.Event;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;
using Neutronium.Core;
using Neutronium.Core.Infra;
using Neutronium.WebBrowserEngine.ChromiumFx.WPF;
using System;
using System.Threading;
using Neutronium.WebBrowserEngine.ChromiumFx.Util;

namespace Neutronium.WebBrowserEngine.ChromiumFx.Session
{
    internal class ChromiumFxSession : IDisposable
    {
        private static ChromiumFxSession _Session = null;
        private readonly Action<CfxSettings> _SettingsBuilder;
        private readonly Action<CfxOnBeforeCommandLineProcessingEventArgs> _CommandLineHandler;
        private readonly string _CurrentDirectory;
        private readonly NeutroniumSchemeHandlerFactory _NeutroniumSchemeHandlerFactory;

        private ChromiumFxSession(Action<CfxSettings> settingsBuilder, Action<CfxBrowserSettings> browserSettingsUpdater, Action<CfxOnBeforeCommandLineProcessingEventArgs> commandLineHandler, IWebSessionLogger webSessionLogger)
        {
            _CurrentDirectory = this.GetType().Assembly.GetPath();
            _SettingsBuilder = settingsBuilder;
            _CommandLineHandler = commandLineHandler;
            CfxRuntime.LibCefDirPath = GetPath($@"{CefRepo}\Release");

            ChromiumWebBrowser.OnBeforeCfxInitialize += ChromiumWebBrowser_OnBeforeCfxInitialize;
            ChromiumWebBrowser.OnBeforeCommandLineProcessing += ChromiumWebBrowser_OnBeforeCommandLineProcessing;

            int value = CfxRuntime.ExecuteProcess();
            if (value >= 0)
                Environment.Exit(value);
            CfxRuntime.EnableHighDpiSupport();
            ChromiumWebBrowser.Initialize();

            browserSettingsUpdater?.Invoke(NeutroniumSettings.NeutroniumBrowserSettings);

            _NeutroniumSchemeHandlerFactory = new NeutroniumSchemeHandlerFactory(webSessionLogger);
            //need this to make request interception work
            CfxRuntime.RegisterSchemeHandlerFactory("https", "application", _NeutroniumSchemeHandlerFactory);
        }

        private static string CefRepo => (IntPtr.Size == 8) ? "cef64" : "cef";
        private string GetPath(string relativePath)
        {
            return System.IO.Path.Combine(_CurrentDirectory, relativePath);
        }

        private void ChromiumWebBrowser_OnBeforeCommandLineProcessing(CfxOnBeforeCommandLineProcessingEventArgs e)
        {
            _CommandLineHandler?.Invoke(e);
        }

        private void ChromiumWebBrowser_OnBeforeCfxInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            var settings = e.Settings;
            settings.LogSeverity = CfxLogSeverity.Error;

            _SettingsBuilder?.Invoke(settings);

            settings.LocalesDirPath = GetPath($@"{CefRepo}\Resources\locales");
            settings.Locale = Thread.CurrentThread.CurrentCulture.ToString();
            settings.ResourcesDirPath = GetPath($@"{CefRepo}\Resources");
            settings.BrowserSubprocessPath = GetPath("ChromiumFXRenderProcess.exe");
            settings.MultiThreadedMessageLoop = true;
            settings.NoSandbox = true;
        }

        internal static ChromiumFxSession GetSession(IWebSessionLogger logger, Action<CfxSettings> settingsBuilder, Action<CfxBrowserSettings> browserSettingsUpdater, Action<CfxOnBeforeCommandLineProcessingEventArgs> commandLineHandler)
        {
            if (_Session != null)
                return _Session;

            _Session = new ChromiumFxSession(settingsBuilder, browserSettingsUpdater, commandLineHandler, logger);
            return _Session;
        }

        public void Dispose()
        {
            CfxRuntime.Shutdown();
        }
    }
}
