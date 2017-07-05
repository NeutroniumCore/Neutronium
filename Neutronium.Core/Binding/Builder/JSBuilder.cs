using Neutronium.Core.Builder;
using System;

namespace Neutronium.Core.Binding.Builder
{
    public class JSBuilder
    {
        private readonly Action<IJavascriptObjectBuilder> _BuildItSelf;
        private readonly Action<IJavascriptObjectBuilder> _UpdateAfterChildrenCreation;

        public JSBuilder(Action<IJavascriptObjectBuilder> build)
        {
            _BuildItSelf = build;
        }

        public JSBuilder(Action<IJavascriptObjectBuilder> build, Action<IJavascriptObjectBuilder> update)
        {
            _BuildItSelf = build;
            _UpdateAfterChildrenCreation = update;
        }

        public void BuildItSelf(IJavascriptObjectBuilder builder)
        {
            _BuildItSelf(builder);
        }

        public void UpdateAfterChildrenCreation(IJavascriptObjectBuilder builder)
        {
            _UpdateAfterChildrenCreation?.Invoke(builder);
        }
    }
}