namespace Neutronium.Core.WebBrowserEngine.JavascriptObject
{
    /// <summary>
    /// Option to create objects in bulk mode
    /// </summary>
    public struct ObjectsCreationOption
    {
        /// <summary>
        /// Number of read write objects to be created
        /// </summary>
        public int ReadWriteNumber { get; }

        /// <summary>
        /// Number of read only objects to be created
        /// </summary>
        public int ReadOnlyNumber { get; }

        /// <summary>
        /// Total number of objects to be created
        /// </summary>
        public int TotalNumber => ReadWriteNumber + ReadOnlyNumber;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="readWriteNumber"></param>
        /// <param name="readOnlyNumber"></param>
        public ObjectsCreationOption(int readWriteNumber, int readOnlyNumber)
        {
            ReadWriteNumber = readWriteNumber;
            ReadOnlyNumber = readOnlyNumber;
        }
    }
}
