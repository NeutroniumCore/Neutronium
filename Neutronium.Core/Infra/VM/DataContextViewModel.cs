using Neutronium.Core.Navigation.Window;

namespace Neutronium.Core.Infra.VM
{
    public class DataContextViewModel
    {
        public HtmlLogicWindow Window { get; } = new HtmlLogicWindow();
        public object ViewModel { get; }

        public DataContextViewModel(object root)
        {
            ViewModel = root;
        }
    }
}
