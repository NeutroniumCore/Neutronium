using System;

namespace Neutronium.Core.Navigation.Routing
{
    public class ConventionRouter : Router
    {
        private readonly string _Pattern;
        private const string ViewModel = "ViewModel";

        public ConventionRouter(INavigationBuilder builder, string pattern) : base(builder)
        {
            _Pattern = pattern.Replace("{vm}", "{0}").Replace("{id}", "{1}");
        }

        protected override string BuildPath(Type type, string id)
        {
            var name = type.Name;
            if (name.EndsWith(ViewModel))
            {
                name = name.Substring(0, name.Length - ViewModel.Length);
            }
            return string.Format(_Pattern, name, id).Replace(@"\\", @"\");
        }
    }
}
