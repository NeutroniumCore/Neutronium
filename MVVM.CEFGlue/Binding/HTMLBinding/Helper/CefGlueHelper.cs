using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using CefGlue.Window;

namespace MVVM.CEFGlue.HTMLBinding
{
    public static class CefGlueHelper
    {
        public static void ExecuteWhenReady(this ICefGlueWindow view, Action ToBeApply)
        {
            Action Check = () =>
                {
                    bool done = false;
                    EventHandler<LoadEndEventArgs> leeh = null;
                    leeh = (o, e) =>
                    {
                        ToBeApply();
                        done = true;
                        view.LoadEnd -= leeh;
                    };

                    view.LoadEnd += leeh;
                    if ((view.IsLoaded) && !done)
                    {
                        ToBeApply();
                        view.LoadEnd -= leeh;
                    }
                };

            view.GetDispatcher().RunAsync(Check);
        }
    }
}
