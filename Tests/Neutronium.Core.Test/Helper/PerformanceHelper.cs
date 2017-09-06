using System;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Neutronium.Core.Test.Helper
{
    public class PerformanceHelper : IDisposable
    {
        private readonly Stopwatch _Stopwatch;
        private readonly ITestOutputHelper _Output;
        private readonly Func<double, string> _DescriptionBuilder;
        public double DiscountTime { get; set; }

        public PerformanceHelper(ITestOutputHelper output, string description) :
            this(output, ts => $"{description}: {ts / 1000} sec")
        {
        }

        public PerformanceHelper(ITestOutputHelper output, string description, int operationNumber) :
            this(output, ts => $"{description}: {((operationNumber * 1000) / ts)} operations/ sec")
        {
        }

        public PerformanceHelper(ITestOutputHelper output, Func<double, string> descriptionBuilder)
        {
            _DescriptionBuilder = descriptionBuilder;
            _Output = output;
            _Stopwatch = new Stopwatch();
            _Stopwatch.Start();
        }

        public void Dispose()
        {
            _Stopwatch.Stop();
            var ts = _Stopwatch.ElapsedMilliseconds;
            _Output.WriteLine(_DescriptionBuilder((double)ts - DiscountTime));
        }
    }
}
