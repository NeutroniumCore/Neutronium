using Example.Cfx.Spa.Routing.SetUp.ScriptRunner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Example.Cfx.Spa.Routing.SetUp
{
    public class ApplicationSetUpBuilder : IDisposable
    {
        private const string Mode = "mode";
        private const string Live = "live";
        private const string Dev = "dev";
        private const string Prod = "prod";
        private const string Url = "url";

        private static readonly Regex _Switch = new Regex("^-", RegexOptions.Compiled);
        private static readonly Regex _SwitchWithValue = new Regex("^-(\\w+)=(.*)$", RegexOptions.Compiled);
        private readonly ApplicationMode _Default;
        private readonly NpmRunner _NpmRunner;

        public Uri Uri { get; set; }

        private static readonly Dictionary<string, ApplicationMode> _Modes = new Dictionary<string, ApplicationMode>
        {
            [Live] = ApplicationMode.Live,
            [Dev] = ApplicationMode.Dev,
            [Prod] = ApplicationMode.Production
        };

        public ApplicationSetUpBuilder(string viewDirectory = "View", ApplicationMode @default = ApplicationMode.Dev,
            string liveScript = "live") :
            this(new Uri($"pack://application:,,,/{viewDirectory}/dist/index.html"),
                @default,
                new NpmRunner(viewDirectory, liveScript))
        {
        }

        internal ApplicationSetUpBuilder(Uri productionUri, ApplicationMode @default, NpmRunner npmRunner)
        {
            Uri = productionUri;
            _Default = @default;
            _NpmRunner = npmRunner;
        }

        public ApplicationSetUp BuildForProduction()
        {
            return new ApplicationSetUp(ApplicationMode.Production, Uri);
        }

        public Task<ApplicationSetUp> BuildFromApplicationArguments(string[] arguments)
        {
            var argument = ParseArguments(arguments);
            return BuildFromArgument(argument);
        }

        public async Task<ApplicationSetUp> BuildFromMode(ApplicationMode mode, Action<string> onNpmLog = null)
        {
            var uri = await BuildUri(mode, onNpmLog).ConfigureAwait(false);
            return new ApplicationSetUp(mode, uri);
        }

        private async Task<ApplicationSetUp> BuildFromArgument(IDictionary<string, string> argumentsDictionary)
        {
            var mode = GetApplicationMode(argumentsDictionary);
            var uri = await BuildDevUri(mode, argumentsDictionary).ConfigureAwait(false);
            return new ApplicationSetUp(mode, uri);
        }

        private ApplicationMode GetApplicationMode(IDictionary<string, string> argumentsDictionary)
        {
            if (argumentsDictionary.TryGetValue(Mode, out var explicitMode) &&
                _Modes.TryGetValue(explicitMode, out var mode))
                return mode;

            return _Default;
        }

        private async Task<Uri> BuildDevUri(ApplicationMode mode, IDictionary<string, string> argumentsDictionary)
        {
            if (argumentsDictionary.TryGetValue(Url, out var uri))
                return new Uri(uri);

            return await BuildUri(mode).ConfigureAwait(false);
        }

        private async Task<Uri> BuildUri(ApplicationMode mode, Action<string> onNpmLog = null)
        {
            if (mode != ApplicationMode.Live)
                return Uri;

            void OnDataReceived(object _, DataReceivedEventArgs dataReceived)
            {
                onNpmLog?.Invoke(dataReceived.Data);
            }

            _NpmRunner.OutputDataReceived += OnDataReceived;
            var port = await _NpmRunner.GetPortAsync().ConfigureAwait(false);
            _NpmRunner.OutputDataReceived -= OnDataReceived;
            return new Uri($"http://localhost:{port}/index.html");
        }

        private static Dictionary<string, string> ParseArguments(IEnumerable<string> arguments)
        {
            var arg = new Dictionary<string, string>();
            if (arguments == null)
                return arg;

            foreach (var rawArgument in arguments)
            {
                var argument = rawArgument.ToLower();
                var match = _SwitchWithValue.Match(argument);
                var switchValue = default(string);
                if (!match.Success)
                {
                    switchValue = _Switch.Replace(argument, String.Empty);
                    arg[switchValue] = "true";
                    continue;
                }

                switchValue = match.Groups[1].Value;
                arg[switchValue] = match.Groups[2].Value; ;
            }
            return arg;
        }

        public void Dispose()
        {
            _NpmRunner?.Dispose();
        }
    }
}
