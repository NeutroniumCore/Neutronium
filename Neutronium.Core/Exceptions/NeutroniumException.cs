using System;

namespace Neutronium.Core.Exceptions
{
    public class NeutroniumArgumentException : ArgumentException
    {
        public NeutroniumArgumentException(string iM) : base(iM)
        {
        }
    }

    public class NeutroniumException : Exception
    {
        public NeutroniumException(string iM) : base(iM)
        {
        }
    }
}
