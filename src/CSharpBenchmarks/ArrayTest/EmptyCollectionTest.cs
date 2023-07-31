using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class EmptyCollectionTest
	{
		[Params(10000)]
		public int Count { get; set; }

		[Benchmark]
		public void ArrayEmpty1()
		{
			for (int i = 0; i < Count; i++)
			{
				var arr = new int[0];
				if (arr.Length == 0)
				{

				}
			}
		}

		[Benchmark]
		public void ArrayEmpty2()
		{
			for (int i = 0; i < Count; i++)
			{
				var arr = Array.Empty<int>();
				if (arr.Length == 0)
				{

				}
			}
		}

		[Benchmark]
		public void ListEmpty1()
		{
			for (int i = 0; i < Count; i++)
			{
				var list = new List<int>(0);
				if (list.Count == 0)
				{

				}
			}
		}

		[Benchmark]
		public void ListEmpty2()
		{
			for (int i = 0; i < Count; i++)
			{
				var list = Enumerable.Empty<int>();
				if (list.Any())
				{

				}
			}
		}
	}
}
