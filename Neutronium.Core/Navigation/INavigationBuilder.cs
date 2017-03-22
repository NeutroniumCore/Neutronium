using System;

namespace Neutronium.Core.Navigation
{
    /// <summary>
    /// interface used to build navigation beetween ViewModels
    /// based on ViewModel type
    /// </summary>
    public interface INavigationBuilder
    {
        /// <summary>
        /// Register a file relative path to HTML file corresponding to a viewmodel type 
        /// </summary>
        /// <param name="type">
        /// Type of view model to register
        /// </param>
        /// <param name="path">
        /// relative file path to HTML file
        /// </param>
        /// <param name="id">
        /// Optional id information, can be used if same type can resolve to different 
        /// view depending on context
        /// <returns>
        /// the navigation builder instance
        /// </returns>
        INavigationBuilder Register(Type type, string path, string id = null);

        /// <summary>
        /// Register a file relative path to HTML file corresponding to a viewmodel type 
        /// </summary>
        /// <param name="path">
        /// relative file path to HTML file
        /// </param>
        /// <param name="id">
        /// Optional id information, can be used if same type can resolve to different 
        /// view depending on context
        /// </param>
        /// <typeparam name="T">
        /// Type of view model to register
        /// </typeparam>
        /// <returns>
        /// the navigation builder instance
        /// </returns>
        INavigationBuilder Register<T>(string path, string id=null);

        /// <summary>
        /// Register a file absolute path to HTML file corresponding to a viewmodel type 
        /// </summary>
        /// <param name="path">
        /// absolute file path to HTML file
        /// </param>
        /// <param name="id">
        /// Optional id information, can be used if same type can resolve to different 
        /// view depending on context
        /// </param>
        /// <typeparam name="T">
        /// Type of view model to register
        /// </typeparam>
        /// <returns>
        /// the navigation builder instance
        /// </returns>
        INavigationBuilder RegisterAbsolute<T>(string path, string id = null);

        /// <summary>
        /// Register an Uri to HTML resource corresponding to a viewmodel type 
        /// </summary>
        /// <param name="path">
        /// Uri to HTML resource
        /// </param>
        /// <param name="id">
        /// Optional id information, can be used if same type can resolve to different 
        /// view depending on context
        /// </param>
        /// <typeparam name="T">
        /// Type of view model to register
        /// </typeparam>
        /// <returns>
        /// the navigation builder instance
        /// </returns>
        INavigationBuilder Register<T>(Uri path, string id = null);
    }
}
