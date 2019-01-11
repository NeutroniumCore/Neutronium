using Neutronium.Core.Infra;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Cfx.Spa.Routing.SetUp.ScriptRunner
{
    public class NpmRunner : IDisposable
    {
        private static readonly Regex _LocalHost = new Regex(@"http:\/\/localhost:(\d{1,4})", RegexOptions.Compiled);
        private static readonly Regex _Key = new Regex(@"\((\w)\/\w\)\?", RegexOptions.Compiled);

        private Process _Process;
        private readonly string _Script;
        private TaskCompletionSource<int> _PortFinderCompletionSource = new TaskCompletionSource<int>();
        private readonly TaskCompletionSource<string> _StopKeyCompletionSource = new TaskCompletionSource<string>();
        private Task<string> StopKeyAsync => _StopKeyCompletionSource.Task;
        private State _State = State.NotStarted;
        private readonly string _WorkingDirectory;

        public event EventHandler<MessageEventArgs> OnMessageReceived;

        private enum State
        {
            NotStarted,
            Initializing,
            Running,
            Cancelling,
            Closing,
            Closed
        };

        public NpmRunner(string directory, string script)
        {
            _Script = script;
            var root = DirectoryHelper.GetCurrentDirectory();
            _WorkingDirectory = Path.Combine(root, directory);

            _Process = CreateProcess();
        }

        private Process CreateProcess()
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "cmd",
                    RedirectStandardInput = true,
                    WorkingDirectory = _WorkingDirectory,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;

            return process;
        }

        public Task<int> GetPortAsync(CancellationToken cancellationToken)
        {
            var currentTask = _PortFinderCompletionSource.Task;
            if (currentTask.IsCompleted && !currentTask.IsCanceled)
                return _PortFinderCompletionSource.Task;

            if (currentTask.IsCanceled)
                _PortFinderCompletionSource = new TaskCompletionSource<int>();

            Start(cancellationToken);
            return _PortFinderCompletionSource.Task;
        }

        private void Start(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => StopIfNeed(cancellationToken));

            if (_State != State.NotStarted)
                return;

            _Process.Start();
            _Process.StandardInput.WriteLine($"npm run {_Script}");
            _Process.BeginErrorReadLine();
            _Process.BeginOutputReadLine();
        }

        private async void StopIfNeed(CancellationToken cancellationToken)
        {
            if (!_PortFinderCompletionSource.TrySetCanceled(cancellationToken))
                return;

            var stopped = await Stop(State.Cancelling, State.NotStarted).ConfigureAwait(false);
            if (!stopped)
                return;

            _Process = CreateProcess();
        }

        public Task<bool> Cancel()
        {
            return Stop(State.Closing, State.Closed);
        }

        private static int _Count = 0;

        private async Task<bool> Stop(State transitionState, State finalState)
        {
            var c = _Count++;
            if ((_State == State.Closed) || (_State == State.Cancelling))
                return false;

            Console.WriteLine($"started {c}");
            if (_State == State.NotStarted)
            {
                CleanProcess();
                _State = finalState;
                Console.WriteLine($"done 0 {c}");
                return true;
            }

            var currentState = _State;
            _State = transitionState;
            if (!_Process.SendControlC())
            {
                _State = currentState;
                return false;
            }

            var standardInput = _Process.StandardInput;
            standardInput.WriteLine();
            standardInput.WriteLine(await StopKeyAsync.ConfigureAwait(false));
            _State = finalState;
            CleanProcess();
            Console.WriteLine($"done {c}");
            return true;
        }

        private void CleanProcess()
        {
            _Process.ErrorDataReceived -= Process_ErrorDataReceived;
            _Process.OutputDataReceived -= Process_OutputDataReceived;
            _Process.Dispose();
            _Process = null;
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_State == State.NotStarted)
                _State = State.Initializing;

            var data = e.Data;
            OnMessageReceived?.Invoke(this, new MessageEventArgs(e.Data, false));

            switch (_State)
            {
                case State.Initializing:
                    TryParsePort(data);
                    break;

                case State.Cancelling:
                case State.Closing:
                    TryParseKey(data);
                    break;
            };
        }

        private void TryParsePort(string data)
        {
            var match = _LocalHost.Match(data);
            if (!match.Success)
                return;

            var portString = match.Groups[1].Value;
            if (!int.TryParse(portString, out var port))
                return;

            _State = State.Running;
            _PortFinderCompletionSource.TrySetResult(port);    
        }

        private void TryParseKey(string data)
        {
            var match = _Key.Match(data);
            if (!match.Success)
                return;

            _StopKeyCompletionSource.TrySetResult(match.Groups[1].Value);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnMessageReceived?.Invoke(this, new MessageEventArgs(e.Data, true));
        }

        public void Dispose()
        {
            Cancel().Wait();
        }

        public override string ToString() => _State.ToString();
    }
}