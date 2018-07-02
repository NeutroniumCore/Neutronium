using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Infra
{
    public static class EnumExtensions
    {
        private static readonly object[] _EmptyArray = new object[0];

        public static string GetDescription(this Enum input)
        {
            if (input == null) return string.Empty;

            var enumType = input.GetType();
            var enumTypeInfo = enumType.GetTypeInfo();

            if (Enum.IsDefined(enumType, input))
                return GetDisplayName(enumType, input);

            return IsBitFieldEnum(enumTypeInfo) ? GetBitFlagComposedDescription(enumType, input) : input.ToString();
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
                builder.Insert(0, GetDisplayName(enumType, enumValue));

                if (result == 0)
                    return builder.ToString();
            }

            return input.ToString();
        }

        private static string GetDisplayName(Type enumType, Enum @enum)
        {
            var valueName = @enum.ToString();
            var field = enumType.GetField(valueName);
            var displayAttribute = field.GetAttribute<DisplayAttribute>();
            var res = GetDisplayName(displayAttribute);
            if (res != null)
                return res;

            var attribute = field.GetAttribute<DescriptionAttribute>();
            return (attribute != null) ? attribute.Description : valueName;
        }

        private static string GetDisplayName(DisplayAttribute attribute)
        {
            try
            {
                const BindingFlags parameter = BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
                var propertyInfo = attribute?.ResourceType?.GetProperty(attribute.Description, parameter);
                if (propertyInfo == null)
                    return null;

                var getter = propertyInfo.GetGetMethod() ?? propertyInfo.GetGetMethod(true);
                return (string)getter?.Invoke(null, _EmptyArray);
            }
            catch
            {
                return null;
            }
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
