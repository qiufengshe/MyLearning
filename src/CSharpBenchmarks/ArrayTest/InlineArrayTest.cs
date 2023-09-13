using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class InlineArrayTest
	{
		[Params(10, 100, 1000)]
		public int Count { get; set; }

		[Benchmark(Baseline = true)]
		public void TestArray()
		{
			for (int i = 0; i < Count; i++)
			{
				var arr = new int[10];
				for (int j = 0; j < arr.Length; j++)
				{
					arr[j] = j;
				}
			}
		}


		[Benchmark]
		public void TestArray2()
		{
			for (int i = 0; i < Count; i++)
			{
				Test1 arr = new Test1();
				for (int j = 0; j < Test1.Length; j++)
				{
					arr[j] = j;
				}
			}
		}

	}

	//#if NET8_0_OR_GREATER
	[InlineArray(Length)]
	public struct Test1
	{
		public const int Length = 10;
		public int x;
	}
	//#endif
}
