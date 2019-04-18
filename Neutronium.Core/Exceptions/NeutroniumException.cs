using System;

namespace Neutronium.Core.Exceptions
{
    public class NeutroniumArgumentException : ArgumentException
    {
        public string Url { get; }
        public NeutroniumArgumentException(string message, string url) : base(NeutroniumException.BuildMessage(message, url))
        {
            Url = url;
        }
    }

    public class NeutroniumException : Exception
    {
        public string Url { get; }
        public NeutroniumException(string message, string url=null) : base(BuildMessage(message, url))
        {
            Url = url;
        }

        internal static string BuildMessage(string message, string url = null)
        {
            return (url == null) ? message:  $"{message}. Please check: " + $"https://neutroniumcore.github.io/Neutronium/{url}";
        }
    }
 }

