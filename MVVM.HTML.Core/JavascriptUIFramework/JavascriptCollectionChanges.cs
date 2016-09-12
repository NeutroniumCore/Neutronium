using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.JavascriptEngine.JavascriptObject;

namespace Neutronium.Core.JavascriptUIFramework
{
    /// <summary>
    /// Describe a set of modifications related to a collection
    /// </summary>
    public class JavascriptCollectionChanges
    {
        public JavascriptCollectionChanges(IJavascriptObject collection,  
                                            IEnumerable<IndividualJavascriptCollectionChange> changes)
        {
            Collection = collection;
            Changes = changes.ToArray();
        }

        /// <summary>
        /// Modified collection
        /// </summary>
        public IJavascriptObject Collection { get; }

        /// <summary>
        /// Atomic changes related to the collectuions
        /// </summary>
        public IndividualJavascriptCollectionChange[] Changes { get; }       
    }
}
