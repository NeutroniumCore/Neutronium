namespace Neutronium.Core.Infra 
{
    public struct IndexDescriptor 
    {
        public int Index { get; set; }
        public bool Insert { get; set; }

        public IndexDescriptor(int index, bool insert = false) 
        {
            Index = index;
            Insert = insert;
        }
    }
}
