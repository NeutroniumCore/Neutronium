using System;

namespace Neutronium.Core.Navigation.Routing
{
    public abstract class Router
    {
        private readonly INavigationBuilder _Builder;

        protected Router(INavigationBuilder builder)
        {
            _Builder = builder;
        }

        protected abstract string BuildPath(Type type, string id);

        public void Register<T>(string id = null)
        {
            _Builder.Register<T>(BuildPath(typeof(T), id), id);
        }
    }
}
