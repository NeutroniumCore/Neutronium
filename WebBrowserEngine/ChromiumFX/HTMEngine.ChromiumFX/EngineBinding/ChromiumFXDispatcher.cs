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
        private readonly IWebSessionLogger _Logger;
        private readonly ConcurrentQueue<Action> _Actions = new ConcurrentQueue<Action>();
        private readonly CfrTask _CfrTask;

        private bool _IsExecutingActions;
        private CfrTaskRunner TaskRunner { get; }

        public ChromiumFxDispatcher(CfrBrowser browser, CfrV8Context context, IWebSessionLogger logger) 
        {
            _Logger = logger;
            _Browser = browser;
            _Context = context;
            TaskRunner = _Context.TaskRunner;
            _CfrTask = new CfrTask();
            _CfrTask.Execute += CfrTask_Execute;
        }

        private void CfrTask_Execute(object sender, CfrEventArgs e) 
        {
            _IsExecutingActions = true;
            using (GetContext()) 
            {
                Action action;
                while (_Actions.TryDequeue(out action)) 
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

        private void LogException(Exception exception)
        {
            _Logger?.Info($"Exception encountred during task dispatch: {exception.Message}");
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
            return TaskRunner.BelongsToCurrentThread();
        }

        private Action ToTaskAction(Action perform, TaskCompletionSource<int> taskCompletionSource) 
        {
            return ToTaskAction(() => { perform(); return 0; }, taskCompletionSource);
        }

        private Action ToTaskAction<T>(Func<T> perform, TaskCompletionSource<T> taskCompletionSource) 
        {
            Action result = () => 
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
            };
            return result;
        }

        private ChromiumFxContext GetContext() 
        {
            return new ChromiumFxContext(_Context, _Logger);
        }

        private struct ChromiumFxContext : IDisposable 
        {
            private readonly CfrV8Context _Context;
            private readonly IWebSessionLogger _Logger;

            public ChromiumFxContext(CfrV8Context context, IWebSessionLogger logger) 
            {
                _Logger = logger;
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
                    _Logger?.Info($"Problem in exiting chromiumFx context {ex}");
                }
            }
        }

        private ChromiumFxCRemoteContext GetRemoteContext() 
        {
            return new ChromiumFxCRemoteContext(_Browser);
        }

        private void RunInContext(Action action) 
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
                TaskRunner.PostTask(_CfrTask);
            }
        }
    }
}
