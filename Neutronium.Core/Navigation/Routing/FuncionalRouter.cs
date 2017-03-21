using System;

namespace Neutronium.Core.Navigation.Routing
{
    internal class FuncionalRouter : Router
    {
        private readonly Func<Type, string, string> _PathBuilder;

        public FuncionalRouter(INavigationBuilder builder, Func<Type, string, string> pathBuilder) : base(builder)
        {
            _PathBuilder = pathBuilder;
        }

        protected override string BuildPath(Type type, string id)
        {
            return _PathBuilder(type, id);
        }
    }
}
