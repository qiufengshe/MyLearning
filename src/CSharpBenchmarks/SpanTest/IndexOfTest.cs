using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace CSharpBenchmarks.SpanTest
{
    [MemoryDiagnoser]
    [DisassemblyDiagnoser(printSource: true)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class IndexOfTest
    {
        public byte[] bytes = new byte[] { 0, 0, 0, 0, 71, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 71, 0, 1, 0, 0, 0, 0, 1, 1, 0, 2, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0 };

        public byte[] searchBytes = new byte[] { 0, 1, 0, 0 };

        [Params(1024, 2048)]
        public int Times { get; set; }

        [Benchmark]
        public int SpanIndexOf()
        {
            int sum = 0;
            for (int i = 0; i < Times; i++)
            {
                sum += bytes.AsSpan().IndexOf(searchBytes);
            }
            return sum;
        }

        [Benchmark]
        public int SpanLastIndexOf()
        {
            int sum = 0;
            for (int i = 0; i < Times; i++)
            {
                sum += bytes.AsSpan().LastIndexOf(searchBytes);
            }
            return sum;
        }
    }
    //|          Method |        Job |  Runtime | Toolchain | Times |      Mean |    Error |   StdDev | Ratio | RatioSD | Code Size | Allocated | Alloc Ratio |
    //|---------------- |----------- |--------- |---------- |------ |----------:|---------:|---------:|------:|--------:|----------:|----------:|------------:|
    //|     SpanIndexOf | Job-XRUCSY | .NET 7.0 |    net7.0 |  1024 |  15.36 us | 0.108 us | 0.096 us |  0.28 |    0.00 |     845 B |         - |          NA |
    //|     SpanIndexOf | Job-IWVJPW | .NET 5.0 |    net5.0 |  1024 |  54.19 us | 0.754 us | 0.705 us |  1.00 |    0.00 |     268 B |         - |          NA |
    //|     SpanIndexOf | Job-VTSQQJ | .NET 6.0 |    net6.0 |  1024 |  61.68 us | 0.464 us | 0.434 us |  1.14 |    0.02 |     268 B |         - |          NA |
    //|                 |            |          |           |       |           |          |          |       |         |           |           |             |
    //| SpanLastIndexOf | Job-XRUCSY | .NET 7.0 |    net7.0 |  1024 |  16.75 us | 0.045 us | 0.035 us |  0.69 |    0.01 |     832 B |         - |          NA |
    //| SpanLastIndexOf | Job-IWVJPW | .NET 5.0 |    net5.0 |  1024 |  24.30 us | 0.257 us | 0.215 us |  1.00 |    0.00 |     268 B |         - |          NA |
    //| SpanLastIndexOf | Job-VTSQQJ | .NET 6.0 |    net6.0 |  1024 |  25.22 us | 0.069 us | 0.054 us |  1.04 |    0.01 |     268 B |         - |          NA |
    //|                 |            |          |           |       |           |          |          |       |         |           |           |             |
    //|     SpanIndexOf | Job-XRUCSY | .NET 7.0 |    net7.0 |  2048 |  29.66 us | 0.560 us | 0.575 us |  0.26 |    0.01 |     845 B |         - |          NA |
    //|     SpanIndexOf | Job-IWVJPW | .NET 5.0 |    net5.0 |  2048 | 112.78 us | 1.757 us | 1.644 us |  1.00 |    0.00 |     268 B |         - |          NA |
    //|     SpanIndexOf | Job-VTSQQJ | .NET 6.0 |    net6.0 |  2048 | 117.57 us | 1.186 us | 0.926 us |  1.05 |    0.01 |     268 B |         - |          NA |
    //|                 |            |          |           |       |           |          |          |       |         |           |           |             |
    //| SpanLastIndexOf | Job-XRUCSY | .NET 7.0 |    net7.0 |  2048 |  29.58 us | 0.213 us | 0.199 us |  0.61 |    0.00 |     832 B |         - |          NA |
    //| SpanLastIndexOf | Job-IWVJPW | .NET 5.0 |    net5.0 |  2048 |  48.54 us | 0.168 us | 0.148 us |  1.00 |    0.00 |     268 B |         - |          NA |
    //| SpanLastIndexOf | Job-VTSQQJ | .NET 6.0 |    net6.0 |  2048 |  50.59 us | 0.254 us | 0.212 us |  1.04 |    0.01 |     268 B |         - |          NA |
}
