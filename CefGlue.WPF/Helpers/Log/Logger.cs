using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Helpers.Log
{
    public static class Logger
    {
        public static ILogger Log { get; set; }

        static Logger()
        {
            Log = new NoLogger();
        }
    }
}
