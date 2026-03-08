using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class InlineArray2Test
	{
		[Params(10, 100, 1000)]
		public int Count { get; set; }

		[Benchmark(Baseline = true)]
		public void TestArray1()
		{
			for (int i = 0; i < Count; i++)
			{
				Test1 arr = new();  //内联数组 .
				for (int j = 0; j < Test1.Length; j++)
				{
					arr[j] = j;
				}
			}
		}

#if NET10_0_OR_GREATER
		[Benchmark]
		public void TestArray2()
		{
			for (int i = 0; i < Count; i++)
			{
				InlineArray10<int> arr = new(); //.Net 10 内置数组长度2-16的泛型内敛数组
				for (int j = 0; j < 10; j++)
				{
					arr[j] = j;
				}
			}
		}
#endif

	}

#if NET8_0_OR_GREATER
	[InlineArray(Length)]    //内联数组,并指定数组的长度,这个用法还是有点怪怪的
	public struct Test1
	{
		public const int Length = 10;
		public int x;
	}
#endif
}
