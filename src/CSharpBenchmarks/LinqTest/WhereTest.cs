using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using ZLinq;

namespace CSharpBenchmarks.LinqTest
{
	[DisassemblyDiagnoser(printSource: true, maxDepth: 2)]
	[MemoryDiagnoser]
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	public class WhereTest
	{
		[Params(128)]
		public int Count { get; set; }

		public static List<int> Values1 => [.. Enumerable.Range(0, 16)];

		public static List<int> Values2 => [.. Enumerable.Range(0, 256)];

		[Benchmark(Baseline = true)]
		public int WhereFirstOrDefault16()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var val = Values1.Where(d => d > 0).FirstOrDefault();
				sum += val;
			}

			return sum;
		}

		[Benchmark]
		public int FirstOrDefault16()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var val = Values1.FirstOrDefault(d => d > 0);
				sum += val;
			}

			return sum;
		}



		[Benchmark]
		public int WhereFirstOrDefault32()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var val = Values2.Where(d => d > 0).FirstOrDefault();
				sum += val;
			}

			return sum;
		}

		[Benchmark]
		public int FirstOrDefault32()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var val = Values2.FirstOrDefault(d => d > 0);
				sum += val;
			}

			return sum;
		}

		//[Benchmark]
		//public int ZLinqWhereFirstOrDefault()
		//{
		//	int sum = 0;
		//	for (int i = 0; i < Count; i++)
		//	{
		//		var val = Values2.AsValueEnumerable().Where(d => d > 0).FirstOrDefault();
		//		sum += val;
		//	}

		//	return sum;
		//}

		//[Benchmark]
		//public int ZLinqFirstOrDefault32()
		//{
		//	int sum = 0;
		//	for (int i = 0; i < Count; i++)
		//	{
		//		var val = Values2.AsValueEnumerable().FirstOrDefault(d => d > 0);
		//		sum += val;
		//	}

		//	return sum;
		//}

		[Benchmark]
		public int ListCountExtension()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var val = Values2.Count();
				sum += val;
			}

			return sum;
		}


		[Benchmark]
		public int ListCount()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var val = Values2.Count;
				sum += val;
			}

			return sum;
		}
	}

	//public void Test()
	//	{
	//		int[] arr = [1, 2, 3, 4, 5];
	//		var firstValue = arr.Where(d => d > 0).FirstOrDefault(); //不好

	//		var count = arr.Count();        //不好

	//		var value = arr.FirstOrDefault(d => d > 0);  //减少一次Where调用,性能更好,即使在.Net10 也没有省掉Where调用开销

	//		var lent = arr.Length;  //如果是数组调用Length属性,性能最好,因为Count是一个扩展方法,而Length是数组的内置属性,没有调用开销
	//								//如果是List调用Count属性,性能最好,因为Count是List的内置属性,没有调用开销
	//	}
}
