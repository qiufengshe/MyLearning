using System;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.StringTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class GuidToStringTest
	{
		[Params(128, 1024)]
		public int Count { get; set; }

		[Benchmark(Baseline = true)]
		public void ToString1()
		{
			var sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var s = Guid.NewGuid().ToString().Replace("-", "");
				sum += s.Length;
			}
		}

		[Benchmark]
		public void ToString2()
		{
			var sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var s = Guid.NewGuid().ToString("N");
				sum += s.Length;
			}
		}
	}
}
