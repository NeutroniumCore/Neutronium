using System;

namespace Neutronium.Core.Navigation
{
    public interface INavigationBuilder
    {
        void Register<T>(string path, string id=null);

        void RegisterAbsolute<T>(string path, string id = null);

        void Register<T>(Uri path, string id = null);
    }
}
