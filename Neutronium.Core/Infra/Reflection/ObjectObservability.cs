using System;

namespace Neutronium.Core.Infra.Reflection
{
    /// <summary>
    /// Information about object mutability
    /// </summary>
    [Flags]
    public enum ObjectObservability
    {
        /// <summary>
        /// Object doed not implement INotifyPropertyChanged and is not read-only
        /// </summary>
        None = 0,

        /// <summary>
        /// Read only object
        /// </summary>
        ReadOnly = 1,

        /// <summary>
        /// Object implementing INotifyPropertyChanged
        /// </summary>
        ImplementNotifyPropertyChanged = 2
    }
}
