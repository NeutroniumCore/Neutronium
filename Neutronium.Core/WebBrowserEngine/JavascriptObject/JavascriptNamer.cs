using MoreCollection.Extensions;
using Neutronium.Core.Infra;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Neutronium.Core.WebBrowserEngine.JavascriptObject
{
    public static class JavascriptNamer
    {
        private static readonly IDictionary<Type, Func<object, string>> _Builder = new Dictionary<Type, Func<object, string>>();

        static JavascriptNamer()
        {
            Register<bool>(b => b ? "true" : "false");

            Register<char>(s => $"'{s}'");
            Register<string>(GetCreateExpression);

            Register<DateTime>(GetCreateDateTimeString);
            
            Register<Int64>(Raw);
            Register<UInt64>(Raw);           
            Register<Int32>(Raw);
            Register<Int16>(Raw);
            Register<UInt32>(Raw);
            Register<UInt16>(Raw);

            Register<double>(GetCreateDoubleString);
            Register<float>(GetCreateNumberExpression);
            Register<decimal>(GetCreateNumberExpression);
        }

        private static void Register<T>(Func<T, string> Factory)
        {
            _Builder.Add(typeof(T), (o) => Factory((T)o));
        }

        private static string Raw<T>(T value) => $"{value}";

        private static string GetCreateNumberExpression(decimal value) => GetCreateDoubleString((double)value);
        private static string GetCreateNumberExpression(float value) => GetCreateDoubleString((double)value);

        public static string GetCreateDoubleString(double value)
        {
            return value.ToString("E16", CultureInfo.InvariantCulture);
        }

        public static string GetCreateDateTimeString(DateTime value)
        {
            var valueUtc = value.ToUniversalTime();
            return $"new Date(Date.UTC({valueUtc.Year}, {valueUtc.Month - 1}, {valueUtc.Day}, {valueUtc.Hour}, {valueUtc.Minute}, {valueUtc.Second}, {valueUtc.Millisecond}))";
        }

        public static string GetCreateExpression(string value)
        {
            var filtered = value.Replace(@"\", @"\\")
                                .Replace("\n", "\\n")
                                .Replace("\r", "\\r")
                                .Replace("'", @"\'");
            return $"'{filtered}'";
        }

        public static string GetCreateExpression(object @object)
        {
            var type = @object.GetType().GetUnderlyingType();
            var conv = _Builder.GetOrDefault(type, Undefined);
            return conv(@object);
        }

        private static string Undefined(object _) => "undefined";
    }
}
