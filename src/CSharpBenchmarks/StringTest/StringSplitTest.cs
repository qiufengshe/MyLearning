using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.StringTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class StringSplitTest
	{
		string value = "this, is, a, very long string, with some spaces, commas and more spaces";

		string[] separators = new[] { ",", " s" };
		int count = int.MaxValue;
		StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

		string[] resultArray = new[] { "this", "is", "a", "very long", "tring", "with", "ome", "paces", "commas and more", "paces" };

		[Params(1024, 2048)]
		public int Times { get; set; }

		[Benchmark]
		public void SplitTest()
		{
			int sum = 0;
			for (int i = 0; i < Times; i++)
			{
				var arr = value.Split(separators, count, options);
				if (arr.SequenceEqual(resultArray))
				{
					sum += 1;
				}
			}
		}
	}
}
