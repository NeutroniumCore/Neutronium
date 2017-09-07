using System;
using System.Diagnostics;
using Neutronium.Core.Infra;
using Xunit.Abstractions;

namespace Neutronium.Core.Test.Helper
{
    public class PerformanceHelper : IDisposable {
        private readonly Stopwatch _Stopwatch;
        private readonly ITestOutputHelper _Output;
        private readonly Func<double, string> _DescriptionBuilder;
        public double DiscountTime { get; set; }

        public PerformanceHelper(ITestOutputHelper output, string description) :
            this(output, ts => $"{description}: {ts / 1000} sec") {
        }

        public static PerformanceHelper OperationPerSec(ITestOutputHelper output, string description, int operationNumber) =>
            new PerformanceHelper(output, ts => $"{description}: {((operationNumber * 1000) / ts)} operations/ sec");

        public static PerformanceHelper TimePerOperation(ITestOutputHelper output, string description, int operationNumber) =>
            new PerformanceHelper(output, ts => $"{description}: {ts / 1000 / operationNumber} sec per operations");

        public IDisposable Stop() 
        {
            _Stopwatch.Stop();
            return new DisposableAction(() => _Stopwatch.Start());
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
