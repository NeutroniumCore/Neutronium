using System;

namespace Neutronium.Core.Navigation
{
    public interface INavigationBuilder
    {
        void Register<T>(string iPath,string id=null);

        void RegisterAbsolute<T>(string iPath, string id = null);

        void Register<T>(Uri iPath, string id = null);
    }
}
