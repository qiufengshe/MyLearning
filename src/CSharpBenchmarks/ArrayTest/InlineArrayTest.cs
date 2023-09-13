using System;
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
		public void TestArray1()
		{
			for (int i = 0; i < Count; i++)
			{
				var arr = new int[10];      //在托管堆分配int类型数组,长度为10
				for (int j = 0; j < arr.Length; j++)
				{
					arr[j] = j;
				}
			}
		}

		[Benchmark]
		public unsafe void TestArray2()
		{
			for (int i = 0; i < Count; i++)
			{
				Span<int> arr = stackalloc int[10];  //在栈上分配int类型数组,长度为10
				for (int j = 0; j < 10; j++)
				{
					arr[j] = j;
				}
			}
		}

		[Benchmark]
		public void TestArray3()
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

#if NET8_0_OR_GREATER
	[InlineArray(Length)]    //内联数组,并指定数组的长度,这个用法还是有点怪怪的
	public struct Test1
	{
		public const int Length = 10;
		public int x;
	}
#endif
}
