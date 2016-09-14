namespace Neutronium.Core.Binding.Listeners
{
    public interface IVisitable
    {
        void Visit(IListenableObjectVisitor visitor);
    }
}
