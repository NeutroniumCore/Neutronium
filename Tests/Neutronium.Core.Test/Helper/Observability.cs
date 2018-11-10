using System.ComponentModel;

namespace Neutronium.Core.Test.Helper
{
    public class Observability
    {
    }

    public class None : Observability
    {
        public int Int { get; set; }
    }

    public class ReadOnly : Observability
    {
        public int Int { get; }
    }

    public class Observable : ReadOnlyObservable
    {
        public string String { get; set; }
    }

    public class ReadOnlyObservable : Observability, INotifyPropertyChanged
    {
        public int Int { get; }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Composite : Observability
    {
        public Observability[] Children { get; }

        public Observability Child { get; }

        public Composite(Observability child = null, Observability[] children = null)
        {
            Child = child;
            Children = children;
        }
    }
}
