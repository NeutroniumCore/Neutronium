namespace Neutronium.Core.Binding.Listeners
{
    public interface IEntityUpdater<T>
    {
        void OnEnter(T item);

        void OnExit(T item);
    }
}
