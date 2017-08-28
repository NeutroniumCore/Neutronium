using System.Collections.Generic;
using MoreCollection.Extensions;
using Neutronium.Core.Binding;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Test.Infra 
{
    public class CSharpToJsCacheFake : ICSharpToJsCache 
    {
        private readonly IDictionary<object, IJsCsGlue> _Cache = new Dictionary<object, IJsCsGlue>();
        public void CacheFromCSharpValue(object key, IJsCsGlue value) 
        {
            _Cache.Add(key, value);
        }

        public IJsCsGlue GetCached(object key) 
        {
            return _Cache.GetOrDefault(key);
        }
    }
}
