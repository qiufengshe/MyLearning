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


//| Method |        Job |  Runtime | Toolchain | Times |         Mean |       Error |       StdDev | Ratio | RatioSD | Code Size |   Gen 0 | Allocated | Alloc Ratio |
//|------- |----------- |--------- |---------- |------ |-------------:|------------:|-------------:|------:|--------:|----------:|--------:|----------:|------------:|
//|   Take | Job-ZJKRIV | .NET 7.0 |    net7.0 |  1024 |  66,691.3 ns |   247.98 ns |    207.08 ns |  0.74 |    0.02 |     623 B | 31.2500 |   98304 B |        1.00 |
//|   Take | Job-GWPOIS | .NET 6.0 |    net6.0 |  1024 |  72,213.3 ns |   244.29 ns |    216.55 ns |  0.80 |    0.02 |     333 B | 31.2500 |   98304 B |        1.00 |
//|   Take | Job-LEKBGG | .NET 5.0 |    net5.0 |  1024 |  90,891.2 ns | 1,756.83 ns |  2,462.83 ns |  1.00 |    0.00 |     500 B | 31.2500 |   98304 B |        1.00 |
//|        |            |          |           |       |              |             |              |       |         |           |         |           |             |
//| AsSpan | Job-GWPOIS | .NET 6.0 |    net6.0 |  1024 |     847.7 ns |     1.76 ns |      1.37 ns |  1.00 |    0.00 |      68 B |       - |         - |          NA |
//| AsSpan | Job-LEKBGG | .NET 5.0 |    net5.0 |  1024 |     848.4 ns |     1.45 ns |      1.13 ns |  1.00 |    0.00 |      68 B |       - |         - |          NA |
//| AsSpan | Job-ZJKRIV | .NET 7.0 |    net7.0 |  1024 |   1,249.1 ns |     2.31 ns |      2.05 ns |  1.47 |    0.00 |      58 B |       - |         - |          NA |
//|        |            |          |           |       |              |             |              |       |         |           |         |           |             |
//|   Take | Job-ZJKRIV | .NET 7.0 |    net7.0 |  2048 | 137,061.1 ns |   826.11 ns |    732.33 ns |  0.75 |    0.00 |     623 B | 62.5000 |  196608 B |        1.00 |
//|   Take | Job-GWPOIS | .NET 6.0 |    net6.0 |  2048 | 168,411.8 ns | 7,551.80 ns | 20,925.99 ns |  0.92 |    0.21 |     333 B | 62.5000 |  196608 B |        1.00 |
//|   Take | Job-LEKBGG | .NET 5.0 |    net5.0 |  2048 | 182,651.8 ns |   477.35 ns |    423.16 ns |  1.00 |    0.00 |     500 B | 62.5000 |  196608 B |        1.00 |
//|        |            |          |           |       |              |             |              |       |         |           |         |           |             |
//| AsSpan | Job-LEKBGG | .NET 5.0 |    net5.0 |  2048 |   1,676.9 ns |     4.84 ns |      4.04 ns |  1.00 |    0.00 |      68 B |       - |         - |          NA |
//| AsSpan | Job-GWPOIS | .NET 6.0 |    net6.0 |  2048 |   1,678.5 ns |     8.91 ns |      8.34 ns |  1.00 |    0.01 |      68 B |       - |         - |          NA |
//| AsSpan | Job-ZJKRIV | .NET 7.0 |    net7.0 |  2048 |   2,493.8 ns |    11.53 ns |     10.78 ns |  1.49 |    0.01 |      58 B |       - |         - |          NA |