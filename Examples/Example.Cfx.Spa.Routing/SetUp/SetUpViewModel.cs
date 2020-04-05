using Neutronium.Core.Infra;
using Neutronium.Core.Navigation;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Cfx.Spa.Routing.SetUp
{
    public class SetUpViewModel
    {
        public Uri Uri => _ApplicationSetUp.Uri;
        public bool Debug => _ApplicationSetUp.Debug;
        public ApplicationMode Mode => _ApplicationSetUp.Mode;

        public IDictionary<string, ICommand<ICompleteWebViewComponent>> DebugCommands { get; } = new Dictionary<string, ICommand<ICompleteWebViewComponent>>();

        private readonly ApplicationSetUpBuilder _Builder;
        private ApplicationSetUp _ApplicationSetUp;

        public SetUpViewModel(ApplicationSetUpBuilder builder)
        {
            _Builder = builder;
        }

        private async void GoPreBuild(ICompleteWebViewComponent viewControl)
        {
            await ChangeMode(viewControl, ApplicationMode.Dev, "to prebuilt");
        }

        private async void GoLive(ICompleteWebViewComponent viewControl)
        {
            await ChangeMode(viewControl, ApplicationMode.Live, "to live");
        }

        private async Task ChangeMode(IWebViewComponent viewControl, ApplicationMode destination, string change)
        {
            if (Mode == destination)
                return;

            var resourceLoader = GetResourceReader();
            var createOverlay = resourceLoader.Load("loading.js");
            viewControl.ExecuteJavascript(createOverlay);

            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            DebugCommands.Clear();
            DebugCommands[$"Cancel {change}"] = new RelayToogleCommand<ICompleteWebViewComponent>
            (_ =>
            {
                cancellationTokenSource.Cancel();
                UpdateCommands();
            });

            try
            {
                await Task.Run(() => DoGoLive(viewControl, destination, token), token);
            }
            catch (TaskCanceledException)
            {
                var removeOverlay = resourceLoader.Load("removeOverlay.js");
                viewControl.ExecuteJavascript(removeOverlay);
                return;
            }

            await viewControl.SwitchViewAsync(Uri);
        }

        private async Task DoGoLive(IWebViewComponent viewControl, ApplicationMode destination, CancellationToken token)
        {
            var resourceLoader = GetResourceReader();
            var updateOverlay = resourceLoader.Load("update.js");
            void OnNpmLog(string information)
            {
                if (information == null)
                    return;

                var text = JavascriptNamer.GetCreateExpression(information);
                var code = updateOverlay.Replace("{information}", text);
                viewControl.ExecuteJavascript(code);
            }

            UpdateSetUp(await _Builder.BuildFromMode(destination, token, OnNpmLog));
        }

        private ResourceReader GetResourceReader() => new ResourceReader("SetUp.script", this);

        public async Task InitFromArgs(string[] args)
        {
            var setup = await _Builder.BuildFromApplicationArguments(args).ConfigureAwait(false);
            UpdateSetUp(setup);
        }

        public void InitForProduction()
        {
            UpdateSetUp(_Builder.BuildForProduction());
        }

        private void UpdateSetUp(ApplicationSetUp applicationSetUp)
        {
            _ApplicationSetUp = applicationSetUp;
            UpdateCommands();
        }

        private void UpdateCommands()
        {
            DebugCommands.Clear();
            switch (Mode)
            {
                case ApplicationMode.Live:
                    DebugCommands["Reload"] = new RelayToogleCommand<ICompleteWebViewComponent>(htmlView => htmlView.ReloadAsync());
                    DebugCommands["To Prebuilt"] = new RelayToogleCommand<ICompleteWebViewComponent>(GoPreBuild);
                    break;

                case ApplicationMode.Dev:
                    DebugCommands["To Live"] = new RelayToogleCommand<ICompleteWebViewComponent>(GoLive);
                    break;
            }
        }

        public override string ToString()
        {
            return _ApplicationSetUp?.ToString() ?? "Not initialized";
        }
    }
}