using System;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.StringTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class StringBuilderTest
	{
		[Params(16, 128)]
		public int Count { get; set; }

		[Benchmark]
		public void AppendTest()
		{
			for (int i = 0; i < Count; i++)
			{
				_ = Append();
			}
		}

		[Benchmark]
		public void RandomAppendTest()
		{
			for (int i = 0; i < Count; i++)
			{
				var val = Random.Shared.Next(1, 1024);
				_ = RandomAppend(val);
			}
		}

		public string Append()
		{
			StringBuilder sb = new StringBuilder(16);
			_ = sb.Append("hello world!");
			return sb.ToString();
		}

		public string RandomAppend(int val)
		{
			StringBuilder sb = new StringBuilder(16);
			_ = sb.Append($"hello world!{val}");
			return sb.ToString();
		}
	}
}
