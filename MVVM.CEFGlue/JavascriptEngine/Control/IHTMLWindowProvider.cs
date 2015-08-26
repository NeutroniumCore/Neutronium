using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.HTML.Core.Window;

namespace MVVM.HTML.Core.JavascriptEngine
{
    public interface IHTMLWindowProvider : IDisposable
    {
        IHTMLWindow HTMLWindow { get; }

        IDispatcher UIDispatcher { get; }

        void Show();

        void Hide();
    }
}
