using MVVM.HTML.Core.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HTML_WPF.Component
{
    public interface IWPFWebWindow : IDisposable
    {
        IHTMLWindow IHTMLWindow { get; }

        void Inject(Key KeyToInject);

        UIElement UIElement { get; }
    }
}
