using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class ParamsTest
	{
		public int[][] ints;

		public Random random;

		[GlobalSetup]
		public void Setup()
		{
			ints = [[1, 2, 3, 4], [5, 6, 7, 8], [9, 10, 11, 12], [13, 14, 15, 16]];
			random = Random.Shared;
		}

		[Params(64, 128, 512)]
		public int Count { get; set; }

		[Benchmark]
		public void LinqTest()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var index = random.Next(0, 3);
				var arr = ints[index];
				sum += LinqSum(arr);
			}
		}

		[Benchmark]
		public void ForTest()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var index = random.Next(0, 3);
				var arr = ints[index];
				sum += ForSum(arr);
			}
		}

		[Benchmark]
		public void SpanTest()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var index = random.Next(0, 3);
				var arr = ints[index];
				sum += SpanSum(arr.AsSpan());
			}
		}

		public int LinqSum(params int[] values)
		{
			return values.Sum();
		}

		public int ForSum(params int[] values)
		{
			var span = values.AsSpan();
			int sum = 0;
			for (int i = 0; i < span.Length; i++)
			{
				sum += span[i];
			}
			return sum;
		}

		public int SpanSum(Span<int> span)
		{
			int sum = 0;
			for (int i = 0; i < span.Length; i++)
			{
				sum += span[i];
			}
			return sum;
		}
	}
}
