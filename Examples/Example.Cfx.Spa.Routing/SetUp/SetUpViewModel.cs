using System;
using System.Threading.Tasks;
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

        private readonly RelayToogleCommand _ToLive;
        public ICommandWithoutParameter ToLive => _ToLive;

        private readonly ApplicationSetUpBuilder _Builder;
        private ApplicationSetUp _ApplicationSetUp;

        public SetUpViewModel(ApplicationSetUpBuilder builder)
        {
            _Builder = builder;
            _ToLive = new RelayToogleCommand(GoLive)
            {
                ShouldExecute = false
            };
        }

        private async void GoLive()
        {
            if (Mode != ApplicationMode.Dev)
                return;

            UpdateSetUp(await _Builder.BuildFromMode(ApplicationMode.Live));
        }

        public async Task InitFromArgs(string[] args) 
        {
            UpdateSetUp(await _Builder.BuildFromApplicationArguments(args));
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
            _ToLive.ShouldExecute = Mode == ApplicationMode.Dev;
        }
    }
}
