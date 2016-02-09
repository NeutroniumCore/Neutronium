namespace MVVM.HTML.Core.Binding.Listeners
{
    public interface IVisitable
    {
        void Visit(IListenableObjectVisitor visitor);
    }
}
