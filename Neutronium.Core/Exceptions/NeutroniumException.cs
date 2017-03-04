using System;

namespace Neutronium.Core.Exceptions
{
    public class NeutroniumArgumentException : ArgumentException
    {
        public NeutroniumArgumentException(string message) : base(message)
        {
        }
    }

    public class NeutroniumException : Exception
    {
        public NeutroniumException(string message) : base(message)
        {
        }
    }
}
