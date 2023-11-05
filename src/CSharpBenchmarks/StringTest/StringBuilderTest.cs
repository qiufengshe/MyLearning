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

		public string Append()
		{
			StringBuilder sb = new StringBuilder(16);
			_ = sb.Append("hello world!");
			return sb.ToString();
		}
	}
}
