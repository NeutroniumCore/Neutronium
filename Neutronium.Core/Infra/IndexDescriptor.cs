namespace Neutronium.Core.Infra 
{
    public struct IndexDescriptor 
    {
        public int Index { get; }
        public bool Insert { get; }

        public IndexDescriptor(int index, bool insert = false) 
        {
            Index = index;
            Insert = insert;
        }
    }
}
