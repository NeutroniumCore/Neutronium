using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.CEFGlue.HTMLBinding
{
    public class IndividualCollectionChange
    {
        public IndividualCollectionChange(CollectionChangeType iCollectionChange, int iIndex, IJSCSGlue iObject)
        {
            CollectionChangeType=iCollectionChange;
             Index=   iIndex;
             Object = iObject;
        }

        public CollectionChangeType  CollectionChangeType {get;private set;}

        public int Index { get; private set; }

        public IJSCSGlue Object { get; private set; }
    }
}
