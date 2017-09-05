using System;

namespace Neutronium.Core.Binding.GlueObject
{
    public class AttributeDescription : IComparable<AttributeDescription>
    {
        public string Name { get; }
        public IJsCsGlue Glue { get; set; }

        public AttributeDescription(string name, IJsCsGlue glue = null)
        {
            Name = name;
            Glue = glue;
        }

        public int CompareTo(AttributeDescription other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
