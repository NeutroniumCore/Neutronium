using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.V8JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.Binding.Mapping
{
    internal class Mapper : IJSCBridgeCache
    {

        public void Cache(object key, IJSCSGlue value)
        {
            throw new NotImplementedException();
        }

        public void CacheLocal(object key, IJSCSGlue value)
        {
            throw new NotImplementedException();
        }

        public IJSCSGlue GetCached(object key)
        {
            throw new NotImplementedException();
        }

        public IJSCSGlue GetCached(IJavascriptObject key)
        {
            throw new NotImplementedException();
        }
    }
}
