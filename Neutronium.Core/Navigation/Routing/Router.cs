using MoreCollection.Extensions;
using System;
using System.Collections.Generic;

namespace Neutronium.Core.Navigation.Routing
{
    internal abstract class Router : IConventionRouter
    {
        private readonly INavigationBuilder _Builder;

        protected Router(INavigationBuilder builder)
        {
            _Builder = builder;
        }

        protected abstract string BuildPath(Type type, string id);

        public IConventionRouter Register<T>(string id = null)
        {
            _Builder.Register<T>(BuildPath(typeof(T), id), id);
            return this;
        }

        public IConventionRouter Register(Type type, string id = null)
        {
            _Builder.Register(type, BuildPath(type, id), id);
            return this;
        }

        public IConventionRouter Register(IEnumerable<Type> types)
        {
            types.ForEach(t => Register(t));
            return this;
        }
    }
}
