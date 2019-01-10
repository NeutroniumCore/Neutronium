using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Example.Cfx.Spa.Routing.SetUp.ScriptRunner;

namespace Example.Cfx.Spa.Routing.SetUp
{
    public class ApplicationSetUpBuilder : IDisposable
    {
        public Uri Uri { get; set; }

        private const string Mode = "mode";
        private const string Live = "live";
        private const string Dev = "dev";
        private const string Prod = "prod";
        private const string Url = "url";

        private static readonly Regex _Switch = new Regex("^--", RegexOptions.Compiled);
        private static readonly Regex _SwitchWithValue = new Regex("^--(\\w+)=(.*)$", RegexOptions.Compiled);
        private readonly ApplicationMode _Default;
        private readonly NpmRunner _NpmRunner;

        public event EventHandler<MessageEventArgs> OnRunnerMessageReceived
        {
            add => _NpmRunner.OnMessageReceived += value;
            remove => _NpmRunner.OnMessageReceived -= value;
        }

        private static readonly Dictionary<string, ApplicationMode> _Modes = new Dictionary<string, ApplicationMode>
        {
            [Live] = ApplicationMode.Live,
            [Dev] = ApplicationMode.Dev,
            [Prod] = ApplicationMode.Production
        };

        private enum Option
        {
            Mode,
            Url,
        }

        private static readonly Dictionary<Option, Tuple<string, string>> _Options = new Dictionary<Option, Tuple<string, string>>
        {
            [Option.Mode] = Tuple.Create("mode", "m"),
            [Option.Url] = Tuple.Create("url", "u"),
        };

        internal ApplicationSetUpBuilder(string viewDirectory = "View", ApplicationMode @default = ApplicationMode.Dev)
        {
            Uri = new Uri($"pack://application:,,,/{viewDirectory.Replace(@"\", "/")}/dist/index.html");
            _Default = @default;
            _NpmRunner = new NpmRunner(viewDirectory, "live");
        }

        public async Task<ApplicationSetUp> BuildFromMode(ApplicationMode mode, CancellationToken cancellationToken, Action<string> onNpmLog = null)
        {
            var uri = await BuildUri(mode, cancellationToken, onNpmLog).ConfigureAwait(false);
            return new ApplicationSetUp(mode, uri);
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

        private async Task<ApplicationSetUp> BuildFromArgument(IDictionary<string, string> argumentsDictionary)
        {
            var mode = GetApplicationMode(argumentsDictionary);
            var uri = await BuildDevUri(mode, argumentsDictionary).ConfigureAwait(false);
            return new ApplicationSetUp(mode, uri);
        }

        private ApplicationMode GetApplicationMode(IDictionary<string, string> argumentsDictionary)
        {
            if (TryGetValue(argumentsDictionary, Option.Mode, out var explicitMode) &&
                _Modes.TryGetValue(explicitMode, out var mode))
                return mode;

            return _Default;
        }

        private static bool TryGetValue(IDictionary<string, string> argumentsDictionary, Option option, out string explicitMode)
        {
            var (fullName, shortName) = _Options[option];
            return (argumentsDictionary.TryGetValue(fullName, out explicitMode) ||
                argumentsDictionary.TryGetValue(shortName, out explicitMode));
        }

        private async Task<Uri> BuildDevUri(ApplicationMode mode, IDictionary<string, string> argumentsDictionary)
        {
            if (TryGetValue(argumentsDictionary, Option.Url, out var uri))
                return new Uri(uri);

            return await BuildUri(mode, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task<Uri> BuildUri(ApplicationMode mode, CancellationToken cancellationToken, Action<string> onNpmLog = null)
        {
            if (mode != ApplicationMode.Live)
                return Uri;

            void OnDataReceived(object _, MessageEventArgs dataReceived)
            {
                onNpmLog?.Invoke(dataReceived.Message);
            }

            OnRunnerMessageReceived += OnDataReceived;
            try
            {
                var port = await _NpmRunner.GetPortAsync(cancellationToken).ConfigureAwait(false);
                return new Uri($"http://localhost:{port}/index.html");
            }
            finally
            {
                OnRunnerMessageReceived -= OnDataReceived;
            }
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
