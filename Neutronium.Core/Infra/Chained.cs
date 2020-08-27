using System;

namespace Neutronium.Core.Infra
{
    internal class Chained<T>
    {
        internal T Value { get; }

        internal Chained<T> Next { get; set; }

        internal Chained(T current, Chained<T> previous)
        {
            Value = current;
            if (previous != null)
                previous.Next = this;
        }

        internal void ForEach(Action<T> perform)
        {
            var current = this;
            while (current != null)
            {
                perform(current.Value);
                current = current.Next;
            }
        }

        internal Chained<TTarget> MapFilter<TTarget>(Func<T, TTarget> transform, Predicate<TTarget> filter = null)
        {
            filter = filter ?? True<TTarget>;
            var current = this;
            var result = default(Chained<TTarget>);
            var currentTransform = default(Chained<TTarget>);
            while (current != null)
            {
                var target = transform(current.Value);
                if (filter(target))
                {
                    currentTransform = new Chained<TTarget>(target, currentTransform);
                    result = result ?? currentTransform;
                }
                current = current.Next;
            }

            return result;
        }

        private static bool True<TIn>(TIn value) => true;
    }
}
