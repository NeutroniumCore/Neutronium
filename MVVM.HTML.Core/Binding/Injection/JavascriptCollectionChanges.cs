using System.Collections.Generic;
using System.Linq;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class JavascriptCollectionChanges
    {
        public JavascriptCollectionChanges(IJavascriptObject collection,  
                                            IEnumerable<IndividualJavascriptCollectionChange> changes)
        {
            Collection = collection;
            Changes = changes.ToArray();
        }
        public IJavascriptObject Collection {get; private set;}

        public IndividualJavascriptCollectionChange[] Changes {get; private set;}       
    }
}
