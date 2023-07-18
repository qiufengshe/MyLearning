using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class InlineArrayTest
	{
		[Benchmark]
		public void TestArray()
		{
			Span<int> arr = new int[10];
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = i;
			}
		}

		[Benchmark]
		public void TestArray2()
		{
			//Span<int> arr = new Test1();
			//for (int i = 0; i < arr.Length; i++)
			//{
			//	arr[i] = i;
			//}
		}

	}
#if NET8_0_OR_GREATER
	[InlineArray(TestLength)]
	public struct Test1
	{
		public const int TestLength = 10;
		public int x;
	}
#endif
}
