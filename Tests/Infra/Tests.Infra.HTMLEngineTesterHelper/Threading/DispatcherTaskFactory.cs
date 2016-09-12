using System.Threading.Tasks;
using Neutronium.Core.JavascriptEngine.Window;

namespace Tests.Infra.HTMLEngineTesterHelper.Threading 
{
    public class DispatcherTaskFactory : TaskFactory
    {
        public DispatcherTaskFactory(IDispatcher dispatcher) 
            : base( new DispatcherContextTaskScheduler(dispatcher))
        {       
        }
    }
}
