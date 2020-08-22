namespace Neutronium.Core.Binding.Listeners
{
    public interface IEntityUpdater<in T>
    {
        void OnEnter(T item);

        void OnExit(T item);
    }
}
