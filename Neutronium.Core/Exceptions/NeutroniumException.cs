using System;

namespace Neutronium.Core.Exceptions
{
    /// <summary>
    /// Neutronium argument exception
    /// </summary>
    public class NeutroniumArgumentException : ArgumentException
    {
        public NeutroniumArgumentException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Neutronium exception
    /// </summary>
    public class NeutroniumException : Exception
    {
        public NeutroniumException(string message) : base(message)
        {
        }
    }
}
