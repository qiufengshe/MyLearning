using System;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.MathTest
{
    [DisassemblyDiagnoser(printSource: true, maxDepth: 3)]
    [MemoryDiagnoser]
    public class RoundTest
    {
        [Params(1.5, 2.5, 3.5)]
        public double Value { get; set; }

        [Benchmark]
        public double ToEvenTest() => Math.Round(Value, MidpointRounding.ToEven);


        [Benchmark]
        public double AwayFromZeroTest() => Math.Round(Value, MidpointRounding.AwayFromZero);


        [Benchmark(Baseline = true)]
        public double RoundDefault() => Math.Round(Value);
    }
}
