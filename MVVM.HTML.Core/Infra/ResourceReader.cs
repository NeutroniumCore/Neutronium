using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MVVM.HTML.Core.Infra
{
    public class ResourceReader
    {
        private readonly string _Directory;
        private readonly Assembly _Assembly;

        public ResourceReader(string directory, object objectAssembly)
        {
            _Directory = directory;
            _Assembly = objectAssembly.GetType().Assembly;
        }

        public ResourceReader(string directory, Assembly assembly)
        {
            _Directory = directory;
            _Assembly = assembly;
        }

        public string Load(string fileName)
        {
            using (var stream = _Assembly.GetManifestResourceStream($"{_Directory}.{fileName}"))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        public string Load(IEnumerable<string> fileNames)
        {
            var builder = new StringBuilder();
            fileNames.Select(Load).ForEach(s => builder.Append(s));
            return builder.ToString();
        }

        public string Load(params string[] fileNames)
        {
            return Load((IEnumerable<string>)fileNames);
        }
    }
}
