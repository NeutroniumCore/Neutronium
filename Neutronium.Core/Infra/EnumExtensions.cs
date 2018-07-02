using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Infra
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum input)
        {
            if (input == null) return string.Empty;

            var valueName = input.ToString();
            var enumType = input.GetType();
            var enumTypeInfo = enumType.GetTypeInfo();

            if (!Enum.IsDefined(enumType, input))
            {
                if (!IsBitFieldEnum(enumTypeInfo))
                    return valueName;

                return GetBitFlagComposedDescription(enumType, input);
            }


            var field = enumType.GetField(valueName);
            var attribute = field.GetAttribute<DescriptionAttribute>();
            return (attribute != null) ? attribute.Description : valueName;
        }

        private static string GetBitFlagComposedDescription(Type enumType, Enum input)
        {
            var enumValues = Enum.GetValues(enumType).Cast<Enum>().OrderByDescending(v => v);
            var result = (int)(object)input;
            var builder = new StringBuilder();
            var first = true;

            foreach (var enumValue in enumValues)
            {
                var enumIntValue = (int)(object)enumValue;
                if ((result & enumIntValue) != enumIntValue)
                    continue;

                result -= enumIntValue;
                if (!first)
                    builder.Insert(0, " ");
                first = false;
                builder.Insert(0, enumValue.GetDescription());

                if (result == 0)
                    return builder.ToString();
            }

            return input.ToString();
        }


        private static bool IsBitFieldEnum(TypeInfo typeInfo)
        {
            return typeInfo.GetCustomAttribute(typeof(FlagsAttribute)) != null;
        }

        public static T[] GetEnums<T>() where T : struct
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
