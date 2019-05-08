using System;

namespace Neutronium.Core.Exceptions
{
    /// <summary>
    /// Exception factory
    /// </summary>
    public static class ExceptionHelper
    {
        public static Exception Get(string message)
        {
            return new NeutroniumException(message);
        }

        public static Exception GetUnexpected()
        {
            return Get("Unexpected error");
        }

        public static ArgumentException GetArgument(string message)
        {
            return new NeutroniumArgumentException(message);
        }
    }
}
