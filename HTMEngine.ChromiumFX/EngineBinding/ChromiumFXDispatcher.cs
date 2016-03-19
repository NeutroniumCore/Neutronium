using System;
using System.Threading.Tasks;
using Chromium;
using Chromium.Remote;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace HTMEngine.ChromiumFX.EngineBinding 
{
    public class ChromiumFXDispatcher : IDispatcher 
    {
        private readonly CfrV8Context _Context;
        private readonly CfrBrowser _Browser;

        private CfrTaskRunner TaskRunner { get; set; }

        public ChromiumFXDispatcher(CfrBrowser browser, CfrV8Context context) 
        {
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

        private Action ToTaskAction(Action perform, TaskCompletionSource<int> taskCompletionSource) 
        {
            return ToTaskAction( () => { perform(); return 0; } , taskCompletionSource);
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
                        taskCompletionSource.TrySetException(exception);
                    }
                }
            };
            return result;
        }

        private IDisposable GetContext() 
        {
            return new ChromiumFXContext(_Context);
        }

        private class ChromiumFXContext : IDisposable 
        {
            private readonly CfrV8Context _Context;
            public ChromiumFXContext(CfrV8Context context) 
            {
                _Context = context;
                _Context.Enter();
            }
            public void Dispose() 
            {
                _Context.Exit();
            }
        }

        private IDisposable GetRemoteContext() 
        {
            return new ChromiumFXCRemoteContext(_Browser);
        }

        private class ChromiumFXCRemoteContext : IDisposable 
        {
            private readonly CfxRemoteCallContext _CfxRemoteCallContext;
            public ChromiumFXCRemoteContext(CfrBrowser browser) 
            {
                if (CfxRemoteCallContext.IsInContext)
                    return;

                _CfxRemoteCallContext = browser.CreateRemoteCallContext();
                _CfxRemoteCallContext.Enter();
            }
            public void Dispose() 
            {
                if (_CfxRemoteCallContext!=null)
                    _CfxRemoteCallContext.Exit();
            }
        }

        private static CfrTask GetTask(Action perform) 
        {
            var task = new CfrTask();
            task.Execute += (sender, args) => perform();
            return task;
        }

        private void RunInContext(Action action) 
        {
            using (GetRemoteContext()) 
            {
                if (TaskRunner.BelongsToCurrentThread()) {
                    action();
                    return;
                }

                var task = GetTask(action);
                TaskRunner.PostTask(task);
            }
        }
    }
}
