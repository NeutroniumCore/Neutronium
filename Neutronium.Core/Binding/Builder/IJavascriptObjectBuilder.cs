using System;
using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using Neutronium.Core.Infra.Reflection;

namespace Neutronium.Core.Binding.Builder
{
    public interface IJavascriptObjectBuilder
    {
        void RequestObjectCreation(TypePropertyAccessor attributeDescription, IJsCsGlue[] attributeValue);

        void RequestArrayCreation(IList<IJsCsGlue> children);

        void RequestNullCreation();

        void RequestBoolCreation(bool value);

        void RequestIntCreation(int value);

        void RequestStringCreation(string value);

        void RequestUintCreation(uint value);

        void RequestEnumCreation(Enum value);

        void RequestCommandCreation(bool canExecute);

        void RequestDoubleCreation(double value);

        void RequestJsDateTimeCreation(DateTime value);

        void RequestExecutableCreation();     
    }
}