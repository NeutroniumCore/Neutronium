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
         void Register<T>(string id = null);
    }
}