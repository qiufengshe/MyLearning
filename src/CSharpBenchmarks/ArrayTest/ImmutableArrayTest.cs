using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
    [MemoryDiagnoser]
    [DisassemblyDiagnoser(printSource: true)]
    public class ImmutableArrayTest
    {
        public Person[] p1;
        [GlobalSetup]
        public void Setup()
        {
            p1 = new Person[100];
        }

        [Params(100, 1000)]
        public int Count { get; set; }

        [Benchmark]
        public void NewImmutableArray()
        {
            p1[0] = null;
        }

        [GlobalCleanup]
        public void Cleanup()
        {
        }
    }
}
