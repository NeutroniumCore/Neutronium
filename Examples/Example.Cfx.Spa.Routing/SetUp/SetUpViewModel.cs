using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neutronium.Core.Navigation;
using Neutronium.Example.ViewModel.Infra;
using Neutronium.MVVMComponents;
using Neutronium.MVVMComponents.Relay;

namespace Example.Cfx.Spa.Routing.SetUp
{
    public class SetUpViewModel: ViewModelBase
    {
        private Uri _Uri;
        public Uri Uri
        {
            get => _Uri;
            private set => Set(ref _Uri, value, nameof(Uri));
        }

        private bool _Debug;
        public bool Debug
        {
            get => _Debug;
            private set => Set(ref _Debug, value, nameof(Debug));
        }

        private ApplicationMode _Mode;
        public ApplicationMode Mode
        {
            get => _Mode;
            private set => Set(ref _Mode, value, nameof(Mode));
        }

        public IDictionary<string,ICommand<ICompleteWebViewComponent>> DebugCommands { get; } = new Dictionary<string, ICommand<ICompleteWebViewComponent>>();

        private readonly ApplicationSetUpBuilder _Builder;
        private ApplicationSetUp _ApplicationSetUp;

        public SetUpViewModel(ApplicationSetUpBuilder builder)
        {
            _Builder = builder;
        }

        private async void GoLive(IWebViewComponent viewControl)
        {
            if (Mode != ApplicationMode.Dev)
                return;

            UpdateSetUp(await _Builder.BuildFromMode(ApplicationMode.Live));
            await viewControl.SwitchViewAsync(Uri);
        }

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
            Uri = _ApplicationSetUp.Uri;
            Mode = _ApplicationSetUp.Mode;
            Debug = _ApplicationSetUp.Debug;
            DebugCommands.Clear();
            switch (Mode)
            {
                case ApplicationMode.Live:
                    DebugCommands["Reload"] = new RelayToogleCommand<ICompleteWebViewComponent>(htmlView => htmlView.ReloadAsync());
                    break;

                case ApplicationMode.Dev:
                    DebugCommands["ToLive"] = new RelayToogleCommand<ICompleteWebViewComponent>(GoLive);
                    break;
            }
        }
    }
}
