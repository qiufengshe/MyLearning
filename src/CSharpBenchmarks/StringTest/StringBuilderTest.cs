using System.CodeDom;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.StringTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class StringBuilderTest
	{
		[Benchmark]
		public string Append()
		{
			StringBuilder sb = new StringBuilder(16);
			sb.Append("hello world!");
			return sb.ToString();
		}
	}
}
