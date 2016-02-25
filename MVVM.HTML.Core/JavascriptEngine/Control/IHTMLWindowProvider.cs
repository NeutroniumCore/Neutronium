using System;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace MVVM.HTML.Core.JavascriptEngine.Control
{
    public interface IHTMLWindowProvider : IDisposable
    {
        IHTMLWindow HTMLWindow { get; }

        IDispatcher UIDispatcher { get; }

        void Show();

        void Hide();
    }
}
