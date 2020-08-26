using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.JavascriptFrameworkMapper;

namespace Neutronium.Core.Binding.SessionManagement
{
    internal interface IInternalSessionCache: ISessionCache
    {
        IJavascriptObjectInternalMapper GetMapper(IJsCsMappedBridge root);

         IEnumerable<IJsCsGlue> AllElementsUiContext { get; }
    }
}
