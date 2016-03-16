using System;
using System.Threading.Tasks;
using Chromium.Remote;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace HTMEngine.ChromiumFX.EngineBinding 
{
    public class ChromiumFXDispatcher : IDispatcher 
    {
        private readonly CfrTaskRunner _TaskRunner;
        public ChromiumFXDispatcher(CfrTaskRunner taskRunner) 
        {
            _TaskRunner = taskRunner;
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

        private static Action ToTaskAction(Action perform, TaskCompletionSource<int> taskCompletionSource) 
        {
            return ToTaskAction( () => { perform(); return 0; } , taskCompletionSource);
        }

        private static Action ToTaskAction<T>(Func<T> perform, TaskCompletionSource<T> taskCompletionSource) 
        {
            Action result = () => 
            {
                try 
                {
                    taskCompletionSource.TrySetResult(perform());
                }
                catch (Exception exception) 
                {
                    taskCompletionSource.TrySetException(exception);
                }
            };
            return result;
        }

        private static CfrTask GetTask(Action perform) 
        {
            var task = new CfrTask();
            task.Execute += (sender, args) => perform();
            return task;
        }

        private void RunInContext(Action action) 
        {
            if (_TaskRunner.BelongsToCurrentThread()) 
            {
                action();
                return;
            }

            var task = GetTask(action);
            _TaskRunner.PostTask(task);
        }
    }
}
