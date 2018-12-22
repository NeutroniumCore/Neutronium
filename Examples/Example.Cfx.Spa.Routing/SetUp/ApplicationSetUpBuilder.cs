using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Example.Cfx.Spa.Routing.SetUp
{
    public class ApplicationSetUpBuilder
    {
        private const string Mode = "mode";
        private const string Live = "live";
        private const string Dev = "dev";
        private const string Prod = "prod";
        private const string Port = "port";
        private const string Url = "url";

        private static readonly Regex _Switch = new Regex("^-", RegexOptions.Compiled);
        private static readonly Regex _SwitchWithValue = new Regex("^-(\\w+)=(.*)$", RegexOptions.Compiled);

        private readonly Uri _ProductionUri;
        private readonly ApplicationMode _Default;
        private readonly int _DefaultPort;

        private static readonly Dictionary<string, ApplicationMode> _Modes = new Dictionary<string, ApplicationMode>
        {
            [Live] = ApplicationMode.Live,
            [Dev] = ApplicationMode.Dev,
            [Prod] = ApplicationMode.Production
        };

        public ApplicationSetUpBuilder(Uri productionUri, ApplicationMode @default = ApplicationMode.Dev, int defaultPort = 8080)
        {
            _ProductionUri = productionUri;
            _DefaultPort = defaultPort;
            _Default = @default;
        }

        public ApplicationSetUp BuildForProduction()
        {
            return new ApplicationSetUp(ApplicationMode.Production, _ProductionUri);
        }

        public ApplicationSetUp BuildFromApplicationArguments(string[] arguments)
        {
            var argument = ParseArguments(arguments);
            return BuildFromArgument(argument);
        }

        private ApplicationSetUp BuildFromArgument(IDictionary<string, string> argumentsDictionary)
        {
            var mode = GetApplicationMode(argumentsDictionary);
            var uri = BuildDevUri(mode, argumentsDictionary);
            return new ApplicationSetUp(mode, uri);
        }

        private ApplicationMode GetApplicationMode(IDictionary<string, string> argumentsDictionary)
        {
            if (argumentsDictionary.TryGetValue(Mode, out var explicitMode) &&
                _Modes.TryGetValue(explicitMode, out var mode))
                return mode;

            return _Default;
        }

        private Uri BuildDevUri(ApplicationMode mode, IDictionary<string, string> argumentsDictionary)
        {
            if (argumentsDictionary.TryGetValue(Url, out var uri))
                return new Uri(uri);

            if (mode != ApplicationMode.Live)
                return _ProductionUri;

            if (!argumentsDictionary.TryGetValue(Port, out var portString) ||
                !int.TryParse(portString, out var port))
                port = _DefaultPort;

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
    }
}
