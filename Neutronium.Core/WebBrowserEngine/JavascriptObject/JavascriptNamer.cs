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

            Register<string>(s => $"'{s.Replace(@"\", @"\\")}'");
            Register<char>(s => $"'{s}'");

            Register<Int64>(Raw);
            Register<UInt64>(Raw);           
            Register<Int32>(Raw);
            Register<Int16>(Raw);
            Register<UInt32>(Raw);
            Register<UInt16>(Raw);

            Register<double>(d => d.ToString("E16", CultureInfo.InvariantCulture));

            //Register<float>((source) => CfrV8Value.CreateDouble((double)source));       
            //Register<decimal>((source) => CfrV8Value.CreateDouble((double)source));       
            //Register<DateTime>((source) => CfrV8Value.CreateDate(CfrTime.FromUniversalTime(source.ToUniversalTime())));
        }

        private static void Register<T>(Func<T, string> Factory)
        {
            _Builder.Add(typeof(T), (o) => Factory((T)o));
        }

        private static string Raw<T>(T value) => $"{value}";

        public static string GetCreateExpression(object @object)
        {
            var type = @object.GetType().GetUnderlyingType();
            var conv = _Builder.GetOrDefault(type, _ => "undefined");
            return conv(@object);
        }
    }
}
