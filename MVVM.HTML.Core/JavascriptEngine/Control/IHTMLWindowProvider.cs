using System;

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
