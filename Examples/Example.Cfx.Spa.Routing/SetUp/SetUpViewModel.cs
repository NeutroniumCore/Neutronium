using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Neutronium.Core.Infra;
using Neutronium.Core.Navigation;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;

namespace Example.Cfx.Spa.Routing.SetUp {
    public class SetUpViewModel {
        public Uri Uri => _ApplicationSetUp.Uri;
        public bool Debug => _ApplicationSetUp.Debug;
        public ApplicationMode Mode => _ApplicationSetUp.Mode;

        public IDictionary<string, ICommand<ICompleteWebViewComponent>> DebugCommands { get; } = new Dictionary<string, ICommand<ICompleteWebViewComponent>>();

        private readonly ApplicationSetUpBuilder _Builder;
        private ApplicationSetUp _ApplicationSetUp;

        public SetUpViewModel(ApplicationSetUpBuilder builder) {
            _Builder = builder;
        }

        private async void GoLive(IWebViewComponent viewControl) {
            if (Mode != ApplicationMode.Dev)
                return;

            var resourceLoader = new ResourceReader("SetUp.script", this);
            var createOverlay = resourceLoader.Load("loading.js");
            viewControl.ExecuteJavascript(createOverlay);

            var updateOverlay = resourceLoader.Load("update.js");
            var messageCount = 0;
            void OnNpmLog(string information) {
                if (messageCount++ < 2)
                    return;

                var text = JavascriptNamer.GetCreateExpression(information);
                var code = updateOverlay.Replace("{information}", text);
                viewControl.ExecuteJavascript(code);
            }

            var cancellationTokenSource = new CancellationTokenSource();

            DebugCommands.Clear();
            DebugCommands["Cancel to live"] = new RelayToogleCommand<ICompleteWebViewComponent>(_ => {
                cancellationTokenSource.Cancel();
                UpdateCommands();
            });

            UpdateSetUp(await _Builder.BuildFromMode(ApplicationMode.Live, cancellationTokenSource.Token, OnNpmLog));
            await viewControl.SwitchViewAsync(Uri);
        }

        public async Task InitFromArgs(string[] args) {
            var setup = await _Builder.BuildFromApplicationArguments(args).ConfigureAwait(false);
            UpdateSetUp(setup);
        }

        public void InitForProduction() {
            UpdateSetUp(_Builder.BuildForProduction());
        }

        private void UpdateSetUp(ApplicationSetUp applicationSetUp) {
            _ApplicationSetUp = applicationSetUp;
            UpdateCommands();
        }

        private void UpdateCommands() {
            DebugCommands.Clear();
            switch (Mode) {
                case ApplicationMode.Live:
                    DebugCommands["Reload"] = new RelayToogleCommand<ICompleteWebViewComponent>(htmlView => htmlView.ReloadAsync());
                    break;

                case ApplicationMode.Dev:
                    DebugCommands["To Live"] = new RelayToogleCommand<ICompleteWebViewComponent>(GoLive);
                    break;
            }
        }

        public override string ToString() {
            return _ApplicationSetUp?.ToString() ?? "Not initialized";
        }
    }
}