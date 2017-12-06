using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Extension
{
    public struct Result<T>
    {
        public bool Success { get; }
        public object TentativeValue { get; }
        public T Value { get; }

        public Result(IJsCsGlue value = null)
        {
            if (value == null)
            {
                TentativeValue = null;
                Success = false;
            }
            else
            {
                TentativeValue = value.CValue;
                Success = (TentativeValue is T) || (typeof(T).IsClass && TentativeValue == null);
            }
            Value = Success ? (T) TentativeValue : default(T);
        }

        public Result(T value)
        {
            TentativeValue = value;
            Success = true;
            Value = value;
        }
    }
}
