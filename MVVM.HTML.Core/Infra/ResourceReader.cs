using System;
using System.IO;
using System.Reflection;

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
            using (var stream = _Assembly.GetManifestResourceStream(string.Format("{0}.{1}", _Directory, fileName)))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}
