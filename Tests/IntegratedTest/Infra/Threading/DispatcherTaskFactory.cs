using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace IntegratedTest.Infra.Threading 
{
    public class DispatcherTaskFactory : TaskFactory
    {
        public DispatcherTaskFactory(IDispatcher dispatcher) 
            : base( new DispatcherContextTaskScheduler(dispatcher))
        {       
        }
    }
}
