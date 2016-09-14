using System;

namespace Neutronium.Core.Exceptions
{
    public static class ExceptionHelper
    {
        public static Exception Get(string iMessage)
        {
            return new NeutroniumException(iMessage);
        }

        public static Exception GetUnexpected()
        {
            return Get("Unexpected error");
        }

        public static ArgumentException GetArgument(string iMessage)
        {
            return new NeutroniumArgumentException(iMessage);
        }
    }
}
