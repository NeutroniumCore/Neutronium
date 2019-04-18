using System;

namespace Neutronium.Core.Exceptions
{
    public static class NeutroniumExceptionHelper
    {
        public static Exception Get(string message, string url = null)
        {
            return new NeutroniumException(message, url);
        }

        public static Exception GetUnexpected()
        {
            return Get("Unexpected error", null);
        }

        public static ArgumentException GetArgument(string message, string url = null)
        {
            return new NeutroniumArgumentException(message, url);
        }
    }
}
