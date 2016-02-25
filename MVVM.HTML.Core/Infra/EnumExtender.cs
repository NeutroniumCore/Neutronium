using System;
using System.ComponentModel;
using System.Reflection;

namespace MVVM.HTML.Core.Infra
{
    public static class EnumExtender
    {
        public static string GetDescription(this System.Enum o)
        {
            if (o == null) return string.Empty;

            string valuename = o.ToString();
            var enumType = o.GetType();
            var fi = enumType.GetField(valuename);
            if (fi == null)
                return valuename;
            
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes != null && attributes.Length > 0) ? attributes[0].Description : valuename;
        }

        public static T[] GetEnums<T>() where T : struct
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
