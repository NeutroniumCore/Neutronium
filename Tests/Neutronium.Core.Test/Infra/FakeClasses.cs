using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neutronium.Core.Test.Infra
{
    public class ReadOnlyClass
    {
        public int Available1 { get; }

        public int Available3 { get; private set; }

        public int Private2 { private get; set; }

        protected int Protected1 { get; set; }

        public int OnlySet { set { } }
    }

    public class ReadOnlyClass2
    {
        public int Property1 { get; private set; }

        public int Property2 { get; private set; }
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
