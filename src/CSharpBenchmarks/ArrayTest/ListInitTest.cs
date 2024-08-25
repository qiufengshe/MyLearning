using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class ListInitTest
	{
		[Benchmark(Baseline = true)]
		public List<string> Old()
		{
			return ["hello", "wolrd", "test"];
		}

		public List<string> New()
		{
			return ["aaaaa", "bbbbb", "cccc"];
		}
	}
}
