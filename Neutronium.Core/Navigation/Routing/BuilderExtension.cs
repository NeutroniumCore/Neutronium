using System;

namespace Neutronium.Core.Navigation.Routing
{
    public static class BuilderExtension
    {
        /// <summary>
        /// Get a convention router based on a template
        /// <seealso cref="IConventionRouter"/>
        /// </summary>
        /// <param name="builder">
        /// </param>
        /// <param name="template">
        /// string template to build relative path to html file
        /// {vm} will be replaced by ViewModel name (excluding ending ViewModel)
        /// {id} will be replaced by id
        /// Ex: @"View\{vm}\dist\index.html"
        ///     @"View\{vm}\{id}\dist\index.html"
        /// </param>
        /// <returns>
        /// corresponding IConventionRouter
        /// </returns>
        public static IConventionRouter GetTemplateConvention(this INavigationBuilder builder, string template)
        {
            return new ConventionRouter(builder, template);
        }

        /// <summary>
        ///  Get a convention router based on a function
        /// </summary>
        /// <param name="builder">
        /// </param>
        /// <param name="pathBuilder">
        /// a function receiving type and id and returning relative path to
        /// the html file
        /// </param>
        /// <returns>
        /// corresponding IConventionRouter
        /// </returns>
        public static IConventionRouter GetConvention(this INavigationBuilder builder, Func<Type, string, string> pathBuilder)
        {
            return new FuncionalRouter(builder, pathBuilder);
        }
    }
}
