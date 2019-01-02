using System;
using System.Threading.Tasks;
using Chromium.Remote;
using Neutronium.Core;
using Neutronium.Core.WebBrowserEngine.Window;
using System.Collections.Concurrent;
using Chromium;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    public class ChromiumFxDispatcher : IDispatcher
    {
        private readonly CfrV8Context _Context;
        private readonly CfrBrowser _Browser;
        private readonly CfxRemoteCallContext _RemoteCallContext;
        private readonly IWebSessionLogger _Logger;
        private readonly ConcurrentQueue<Action> _Actions = new ConcurrentQueue<Action>();
        private volatile bool _IsExecutingActions = false;

        public ChromiumFxDispatcher(CfrBrowser browser, CfrV8Context context, IWebSessionLogger logger)
        {
            _Logger = logger;
            _Browser = browser;
            _Context = context;
            _RemoteCallContext = _Browser.CreateRemoteCallContext();
        }

        private void CfrTask_Execute(object sender, CfrEventArgs e)
        {
            _IsExecutingActions = true;
            using (GetContext())
            {
                while (_Actions.TryDequeue(out var action))
                {
                    try
                    {
                        action();
                    }
                    catch (Exception exception)
                    {
                        LogException(exception);
                    }
                }
                _IsExecutingActions = false;
            }
        }

        public Task RunAsync(Action act)
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            var action = ToTaskAction(act, taskCompletionSource);
            RunInContext(action);
            return taskCompletionSource.Task;
        }

        public void Dispatch(Action act)
        {
            RunInContext(act);
        }

        public void Run(Action act)
        {
            RunAsync(act).Wait();
        }

        public Task<T> EvaluateAsync<T>(Func<T> compute)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            var action = ToTaskAction(compute, taskCompletionSource);
            RunInContext(action);
            return taskCompletionSource.Task;
        }

        public T Evaluate<T>(Func<T> compute)
        {
            return EvaluateAsync(compute).Result;
        }

        public bool IsInContext()
        {
            return CfxRemoteCallContext.IsInContext;
        }

        private Action ToTaskAction(Action perform, TaskCompletionSource<int> taskCompletionSource)
        {
            return ToTaskAction(() => { perform(); return 0; }, taskCompletionSource);
        }

        private Action ToTaskAction<T>(Func<T> perform, TaskCompletionSource<T> taskCompletionSource)
        {
            void Result()
            {
                try
                {
                    var taskResult = perform();
                    taskCompletionSource.TrySetResult(taskResult);
                }
                catch (Exception exception)
                {
                    LogException(exception);
                    taskCompletionSource.TrySetException(exception);
                }
            }

            return Result;
        }

        private void LogException(Exception exception, string message = "Exception encountered during task dispatch")
        {
            _Logger?.Info($"{message}: {exception}");
        }

        private ChromiumFxContext GetContext()
        {
            return new ChromiumFxContext(_Context, this);
        }

        private struct ChromiumFxContext : IDisposable
        {
            private readonly CfrV8Context _Context;
            private readonly ChromiumFxDispatcher _Dispatcher;

            public ChromiumFxContext(CfrV8Context context, ChromiumFxDispatcher dispatcher)
            {
                _Dispatcher = dispatcher;
                _Context = context;
                _Context.Enter();
            }

            public void Dispose()
            {
                try
                {
                    _Context.Exit();
                }
                catch (Exception ex)
                {
                    _Dispatcher.LogException(ex, "Problem in exiting chromiumFx context ");
                }
            }
        }

        private ChromiumFxCRemoteContext GetRemoteContext()
        {
            return new ChromiumFxCRemoteContext(_RemoteCallContext);
        }

        private void RunInContext(Action action)
        {
            try
            {
                RunInContextUnsafe(action);
            }
            catch (Exception ex)
            {
                LogException(ex, "Unable to dispatch action on ChromiumFx context");
            }
        }

        private void RunInContextUnsafe(Action action)
        {
            if (CfxRemoteCallContext.IsInContext)
            {
                action();
                return;
            }

            _Actions.Enqueue(action);

            if (_IsExecutingActions)
                return;

            using (GetRemoteContext())
            {
                var task = new CfrTask();
                task.Execute += CfrTask_Execute;
                CfrRuntime.PostTask(CfxThreadId.Renderer, task);
            }
        }
    }
}
