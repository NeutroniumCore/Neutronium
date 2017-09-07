using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.GlueObject.Factory;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueBuilder 
{
    internal sealed class GlueCommandsBuilder
    {
        public GlueCommandBuilder Command { get; }
        public GlueSimpleCommandBuilder SimpleCommand { get; }
        public GlueResultCommandBuilder ResultCommand { get; }
        public GlueCommandsBuilder(IGlueFactory factory) 
        {
            Command = new GlueCommandBuilder(factory);
            SimpleCommand = new GlueSimpleCommandBuilder(factory);
            ResultCommand = new GlueResultCommandBuilder(factory);
        }
    }

    internal sealed class GlueCommandBuilder : ICsToGlueConverter 
    {
        private readonly IGlueFactory _GlueFactory;

        public GlueCommandBuilder(IGlueFactory factory) 
        {
            _GlueFactory = factory;
        }

        public IJsCsGlue Convert(object @object) => _GlueFactory.Build((ICommand)@object);
    }

    internal sealed class GlueSimpleCommandBuilder : ICsToGlueConverter 
    {
        private readonly IGlueFactory _GlueFactory;

        public GlueSimpleCommandBuilder(IGlueFactory factory) 
        {
            _GlueFactory = factory;
        }

        public IJsCsGlue Convert(object @object) => _GlueFactory.Build((ISimpleCommand)@object);
    }

    internal sealed class GlueResultCommandBuilder : ICsToGlueConverter 
    {
        private readonly IGlueFactory _GlueFactory;

        public GlueResultCommandBuilder(IGlueFactory factory) 
        {
            _GlueFactory = factory;
        }

        public IJsCsGlue Convert(object @object) => _GlueFactory.Build((IResultCommand)@object);
    }
}
