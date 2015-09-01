using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_WPF.Component
{
    public interface IHTMLEngineFactory : IDisposable
    {

        IWPFWebWindowFactory Resolve(string EngineName);

        void Register(IWPFWebWindowFactory iWPFWebWindowFactory);

    }
}
