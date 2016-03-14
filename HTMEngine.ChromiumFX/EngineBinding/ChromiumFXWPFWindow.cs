using System.Windows;
using System.Windows.Input;
using HTML_WPF.Component;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace HTMEngine.ChromiumFX.EngineBinding 
{
    internal class ChromiumFXWPFWindow : IWPFWebWindow 
    {
        public void Dispose() 
        {
            throw new System.NotImplementedException();
        }

        public IHTMLWindow HTMLWindow { get; }
        public void Inject(Key keyToInject) 
        {
            throw new System.NotImplementedException();
        }

        public UIElement UIElement { get; set; }
    }
}
