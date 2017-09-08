using System.Collections;

namespace Neutronium.Core.Binding.GlueObject
{
    public class AttributeDescription 
    {
        public string Name { get; }
        public IJsCsGlue Glue { get; set; }
        public static IComparer Comparer { get; } = new AttributeDescriptionAsymetricComparer();

        public AttributeDescription(string name, IJsCsGlue glue = null) 
        {
            Name = name;
            Glue = glue;
        }

        private class AttributeDescriptionAsymetricComparer : IComparer
        {
            int IComparer.Compare(object x, object y) 
            {
                var stringX = ((AttributeDescription)x).Name;
                var stringY = (string)y;
                return stringX.CompareTo(stringY);
            }
        }
    }
}
