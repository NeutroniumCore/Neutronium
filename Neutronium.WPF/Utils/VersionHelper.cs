using System;

namespace Neutronium.WPF.Utils
{
    public static class VersionHelper
    {
        public static Version GetVersion(object value)
        {
            return GetVersion(value.GetType());
        }

        public static Version GetVersion(Type type)
        {
            return type.Assembly.GetName().Version;
        }

        public static string GetDisplayName(this Version version)
        {
            return $"{version.Major}.{version.Minor}.{version.Revision}";
        }
    }
}
