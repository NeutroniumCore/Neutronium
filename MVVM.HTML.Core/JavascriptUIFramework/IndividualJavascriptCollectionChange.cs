using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.JavascriptUIFramework
{
    /// <summary>
    /// Describe an atomic modifications related to a collection
    /// </summary>
    public class IndividualJavascriptCollectionChange
    {
        public IndividualJavascriptCollectionChange(CollectionChangeType iCollectionChange, int iIndex, IJavascriptObject iObject)
        {
            CollectionChangeType=iCollectionChange;
             Index=   iIndex;
             Object = iObject;
        }

        /// <summary>
        /// Type of modification
        /// </summary>
        public CollectionChangeType  CollectionChangeType {get;private set;}

        /// <summary>
        /// Index of modification
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Item of collection subject to modification
        /// </summary>
        public IJavascriptObject Object { get; private set; }
    }
}
