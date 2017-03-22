using System;
using System.Collections.Generic;

namespace Neutronium.Core.Navigation.Routing
{
    /// <summary>
    /// Interface to provide automatic class registration based on convention
    /// </summary>
    public interface IConventionRouter 
    {
        /// <summary>
        /// register the corresponding type and id based on the current convention
        /// </summary>
        /// <typeparam name="T">
        /// type to register
        /// </typeparam>
        /// <param name="id">
        /// id to register
        /// </param>
        /// <returns>
        /// the convention router instance
        /// </returns>
        IConventionRouter Register<T>(string id = null);

        /// <summary>
        /// register the corresponding type and id based on the current convention
        /// </summary>
        /// <param name="type">
        /// type to register
        /// </param>
        /// <param name="id">
        /// id to register
        /// </param>
        /// <returns>
        /// the convention router instance
        /// </returns>
        IConventionRouter Register(Type type, string id = null);


        /// <summary>
        /// register the corresponding types without id based on the current convention
        /// </summary>
        /// <param name="types">
        /// types to register
        /// </param>
        /// <returns>
        /// the convention router instance
        /// </returns>
        IConventionRouter Register(IEnumerable<Type> types);
    }
}