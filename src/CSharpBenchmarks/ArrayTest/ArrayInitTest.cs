using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class ArrayInitTest
	{
		[Benchmark(Baseline = true)]
		public string[] Old()
		{
			return new string[] { "hello", "wolrd", "test" };
		}

		[Benchmark]
		public string[] New()
		{
			return ["aaaaa", "bbbbb", "cccc"];
		}

	}
}
