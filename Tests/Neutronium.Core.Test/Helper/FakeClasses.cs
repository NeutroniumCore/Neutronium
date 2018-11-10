using System.ComponentModel;

namespace Neutronium.Core.Test.Helper
{
    public class ReadOnlyClass
    {
        public int Available1 { get; }

        public int Available3 { get; private set; }

        public int Private2 { private get; set; }

        protected int Protected1 { get; set; }

        public int OnlySet { set { } }
    }

    public class ReadOnlyClassWithNotifyPropertyChanged : INotifyPropertyChanged
    {
        public int Available1 { get; }

        public int Available3 { get; private set; }

        public int Private2 { private get; set; }

        protected int Protected1 { get; set; }

        public int OnlySet { set { } }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ReadWriteClassWithNotifyPropertyChanged : INotifyPropertyChanged
    {
        public int Available1 { get; }

        public int Available3 { get; private set; }

        public int Private2 { private get; set; }

        protected int Protected1 { get; set; }

        public int OnlySet { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ReadOnlyClass2
    {
        public int Property1 { get; private set; }

        public int Property2 { get; private set; }
    }


    public class ClassWithAttributes
    {
        [Bindable(false)]
        public int NotBindable { get; set; }

        [Bindable(true, BindingDirection.OneWay)]
        public int OneWay { get; set; }

        [Bindable(true, BindingDirection.TwoWay)]
        public int TwoWay { get; set; }

        public int NoAttribute { get; set; }
    }

    [Bindable(true, BindingDirection.OneWay)]
    public class ClassWithAttributesAndDefaultAttribute
    {
        [Bindable(false)]
        public int NotBindable { get; set; }

        [Bindable(true, BindingDirection.OneWay)]
        public int OneWay { get; set; }

        [Bindable(true, BindingDirection.TwoWay)]
        public int TwoWay { get; set; }

        public int NoAttribute { get; set; }
    }

    public class FakeClass
    {
        public int Available1 { get; }

        public int Available2 { get; set; }

        public int Available3 { get; private set; }

        private int Private1 { get; set; }

        public int Private2 { private get; set; }

        protected int Protected1 { get; set; }

        public int Protected2 { protected get; set; }

        public int OnlySet { set { } }
    }


}
