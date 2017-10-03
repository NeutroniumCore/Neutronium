using System;
using System.ComponentModel;

namespace Neutronium.Core.Infra
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum o)
        {
            if (o == null) return string.Empty;

            var valuename = o.ToString();
            var enumType = o.GetType();
            var fi = enumType.GetField(valuename);
            if (fi == null)
                return valuename;
            
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(Types.DescriptionAttribute, false);
            return (attributes.Length > 0) ? attributes[0].Description : valuename;
        }

        public static T[] GetEnums<T>() where T : struct
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
