using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace net6perf.LinqTest
{
    [DisassemblyDiagnoser(printSource: true, maxDepth: 2)]
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class TakTest
    {
        public int[] bytes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 42, 48, 98, 10 };

        [Params(1024, 2048)]
        public int Times { get; set; }

        [Benchmark]
        public int Take()
        {
            int sum = 0;
            for (int i = 0; i < Times; i++)
            {
                var array = bytes.Take(5).ToArray();
                sum += array.Length;
            }

            return sum;
        }


        [Benchmark]
        public int AsSpan()
        {
            int sum = 0;
            for (int i = 0; i < Times; i++)
            {
                var array = bytes.AsSpan().Slice(0, 5);
                sum += array.Length;
            }

            return sum;
        }
    }
}
