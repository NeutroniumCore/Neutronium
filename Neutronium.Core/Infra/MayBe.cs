namespace Neutronium.Core.Infra
{
    public struct MayBe<T>
    {
        public bool Success { get; }
        public object TentativeValue { get; }
        public T Value { get; }

        public MayBe(object tentativeValue)
        {
            TentativeValue = tentativeValue;
            Success = (TentativeValue is T) || (typeof(T).IsClass && TentativeValue == null);
            Value = Success ? (T) TentativeValue : default(T);
        }

        public MayBe(T value)
        {
            TentativeValue = value;
            Success = true;
            Value = value;
        }
    }
}
