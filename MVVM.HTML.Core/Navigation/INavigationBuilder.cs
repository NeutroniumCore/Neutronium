using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.HTML.Core
{
    public interface INavigationBuilder
    {
        void Register<T>(string iPath,string Id=null);

        void RegisterAbsolute<T>(string iPath, string Id = null);

        void Register<T>(Uri iPath, string Id = null);
    }
}
