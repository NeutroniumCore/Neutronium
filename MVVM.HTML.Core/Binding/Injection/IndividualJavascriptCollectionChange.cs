using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class IndividualJavascriptCollectionChange
    {
        public IndividualJavascriptCollectionChange(CollectionChangeType iCollectionChange, int iIndex, IJavascriptObject iObject)
        {
            CollectionChangeType=iCollectionChange;
             Index=   iIndex;
             Object = iObject;
        }

        public CollectionChangeType  CollectionChangeType {get;private set;}

        public int Index { get; private set; }

        public IJavascriptObject Object { get; private set; }
    }
}
