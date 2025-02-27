using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Bogus.Bson;
using Bogus;

namespace CSharpBenchmarks.StringTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class StringToReadOnlySpan
	{
		[Params(100, 1000,10000)]
		public int Times { get; set; }

		[Params("Test", "TestAsync")]
		public string Names { get; set; }

		[Benchmark]
		public int ReadOnlySpanTest()
		{
			int count = 0;
			for (int i = 0; i < Times; i++)
			{
				count += HandleUrl(Names).Length;
			}
			return count;
		}

		[Benchmark(Baseline = true)]
		public int StringTest()
		{
			int count = 0;
			for (int i = 0; i < Times; i++)
			{
				count += HandleUrl2(Names).Length;
			}
			return count;
		}

		public ReadOnlySpan<char> HandleUrl(ReadOnlySpan<char> actionName)
		{
			if (actionName.EndsWith("Async", StringComparison.Ordinal))
			{
				actionName = actionName.Slice(0, actionName.Length - 5);
			}

			return actionName;
		}

		public string HandleUrl2(string actionName)
		{
			if (actionName.EndsWith("Async", StringComparison.Ordinal))
			{
				actionName = actionName.Substring(0, actionName.Length - 5);
			}

			return actionName;
		}
	}
}
