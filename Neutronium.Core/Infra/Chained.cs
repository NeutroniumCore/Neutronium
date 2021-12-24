using System;

namespace Neutronium.Core.Infra
{
    public class Chained<T>
    {
        public T Value { get; }

        public Chained<T> Next { get; set; }

        public Chained(T current, Chained<T> previous)
        {
            Value = current;
            if (previous != null)
                previous.Next = this;
        }

        public void ForEach(Action<T> perform)
        {
            var current = this;
            while (current != null)
            {
                perform(current.Value);
                current = current.Next;
            }
        }

        public Chained<TTarget> MapFilter<TTarget>(Func<T, TTarget> transform, Predicate<TTarget> filter = null)
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
