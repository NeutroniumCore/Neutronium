using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chromium.Remote;
using Neutronium.Core;
using Neutronium.Core.WebBrowserEngine.Window;
using System.Diagnostics;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding 
{
    public class ChromiumFxDispatcher : IDispatcher 
    {
        private readonly CfrV8Context _Context;
        private readonly CfrBrowser _Browser;
        private readonly IWebSessionLogger _Logger;
        private readonly object _Locker = new object();
        private readonly HashSet<CfrTask> _Tasks = new HashSet<CfrTask>();

        private CfrTaskRunner TaskRunner { get; }

        public ChromiumFxDispatcher(CfrBrowser browser, CfrV8Context context, IWebSessionLogger logger) 
        {
            _Logger = logger;
            _Browser = browser;
            _Context = context;
            TaskRunner = _Context.TaskRunner;
        }

        public Task RunAsync(Action act) 
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            var action = ToTaskAction(act, taskCompletionSource);
            RunInContext(action);
            return taskCompletionSource.Task;
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
                using (GetContext()) 
                {
                    try 
                    {
                        taskCompletionSource.TrySetResult(perform());
                    }
                    catch (Exception exception) 
                    {
                        _Logger?.Info(()=> $"Exception encountred during task dispatch: {exception.Message}");
                        taskCompletionSource.TrySetException(exception);
                    }
                }
            };
            return result;
        }

        private ChromiumFXContext GetContext() 
        {
            return new ChromiumFXContext(_Context);
        }

        private struct ChromiumFXContext : IDisposable 
        {
            private readonly CfrV8Context _Context;
            public ChromiumFXContext(CfrV8Context context) 
            {
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
                    Trace.WriteLine($"Problem in exiting chromiumFx context {ex}");
                }
            }
        }

        private ChromiumFxCRemoteContext GetRemoteContext() 
        {
            return new ChromiumFxCRemoteContext(_Browser);
        }

        private void RunInContext(Action action) 
        {
            using (var ctx = GetRemoteContext()) 
            {
                if (TaskRunner.BelongsToCurrentThread()) {
                    action();
                    return;
                }

                var task = AddTask(action);
                TaskRunner.PostTask(task);
            }
        }

        private CfrTask AddTask(Action action)
        {
            var task = new CfrTask();
            task.Execute += (o, e) =>
            {
                action();
                RemoveTask(task);
            };

            lock (_Locker)
                _Tasks.Add(task);

            return task;
        }

        private void RemoveTask(CfrTask task)
        {
            task.Dispose();
            lock (_Locker)
                _Tasks.Remove(task);
        }
    }
}
