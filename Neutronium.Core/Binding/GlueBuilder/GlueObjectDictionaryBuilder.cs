using System;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal sealed class GlueObjectDictionaryBuilder : GlueObjectDynamicBuilder
    {
        private readonly Type _TargetType;

        public GlueObjectDictionaryBuilder(ICSharpToGlueMapper converter, Type targetType) : base(converter)
        {
            _TargetType = targetType;
        }

        public IJsCsGlue Convert(IGlueFactory factory, object @object)
        {
            var propertyAcessor = DictionaryPropertyAccessor.FromStringDictionary(@object, _TargetType);
            return Convert(factory, @object, propertyAcessor);
        }
    }
}
