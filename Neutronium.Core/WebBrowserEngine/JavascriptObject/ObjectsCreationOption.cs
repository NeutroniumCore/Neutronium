using System;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.WebBrowserEngine.JavascriptObject
{
    /// <summary>
    /// Option to create objects in bulk mode
    /// </summary>
    public struct ObjectsCreationOption
    {
        /// <summary>
        /// Number of none readonly none observable objects to be created
        /// </summary>
        public int NoneObservableNumber { get; }

        /// <summary>
        /// Number of read only objects to be created
        /// </summary>
        public int ReadOnlyNumber { get; }

        /// <summary>
        /// Number of observable objects to be created
        /// </summary>
        public int ObservableNumber { get; }

        /// <summary>
        /// Number of observable read only objects to be created
        /// </summary>
        public int ReadOnlyObservableNumber { get; }

        /// <summary>
        /// Total number of objects to be created
        /// </summary>
        public int TotalNumber => NoneObservableNumber + ObservableNumber + ReadOnlyNumber + ReadOnlyObservableNumber;

        /// <summary>
        /// Visit the diferent number linked to each ObjectObservability
        /// </summary>
        /// <param name="visitor"></param>
        public void Visit(Action<ObjectObservability, int> visitor)
        {
            visitor(ObjectObservability.None, NoneObservableNumber);
            visitor(ObjectObservability.ReadOnly, ReadOnlyNumber);
            visitor(ObjectObservability.Observable, ObservableNumber);
            visitor(ObjectObservability.ReadOnlyObservable, ReadOnlyObservableNumber);
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="noneObservableNumber"></param>
        /// <param name="readOnlyNumber"></param>
        /// <param name="observableNumber"></param>
        /// <param name="readOnlyObservableNumber"></param>
        public ObjectsCreationOption(int noneObservableNumber, int readOnlyNumber, int observableNumber, int readOnlyObservableNumber)
        {
            NoneObservableNumber = noneObservableNumber;
            ReadOnlyNumber = readOnlyNumber;
            ObservableNumber = observableNumber;
            ReadOnlyObservableNumber = readOnlyObservableNumber;
        }
    }
}
