using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Neutronium.Core.Infra
{
    public class ResourceReader
    {
        private readonly string _Directory;
        private readonly Assembly _Assembly;

        public ResourceReader(string directory, object objectAssembly):this(directory, objectAssembly.GetType().Assembly)
        {
        }

        public ResourceReader(string directory, Assembly assembly)
        {
            _Assembly = assembly;
            _Directory = $"{_Assembly.GetName().Name}.{directory}";
        }

        public string Load(string fileName)
        {
            using (var stream = _Assembly.GetManifestResourceStream($"{_Directory}.{fileName}"))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        public string LoadJavascript(string file, bool es5, bool minified) 
        {
            return Load($"{file}{ValueIf(".es5", es5)}{ValueIf(".min", minified)}.js");
        }

        private static string ValueIf(string value, bool condition) 
        {
            return condition ? value : String.Empty;
        }

        public string Load(IEnumerable<string> fileNames) 
        {
            return Load(fileNames, Load);
        }

        public string LoadJavascript(IEnumerable<string> fileNames, bool es5, bool minified) 
        {
            return Load(fileNames, file => LoadJavascript(file, es5, minified));
        }

        private string Load(IEnumerable<string> fileNames, Func<string,string> loader) 
        {
            var builder = new StringBuilder();
            fileNames.Select(loader).ForEach(s => builder.Append(s));
            return builder.ToString();
        }
    }
} 
