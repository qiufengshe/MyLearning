using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.Syntax
{
	public partial class RegexReplaceTest
	{
		[DisassemblyDiagnoser(printSource: true, maxDepth: 3)]
		[MemoryDiagnoser]
		public partial class RegexTest
		{
			[Params(1024, 2048, 4096)]
			public int Count { get; set; }

			[Params("SELECT * FROM peoples", "abcdefg")]
			public string Input { get; set; }

			private static readonly Regex TableAliasRegex = new Regex("(?<tableAlias>AS \\[Extent\\d+\\](?! WITH \\(NOLOCK\\)))", RegexOptions.IgnoreCase | RegexOptions.Multiline);

			//在.Net 7加入 正则表达式代码生成,简单不单独说明怎么使用
			//1. 使用RegexGenerator特性
			[GeneratedRegex(@"(?<tableAlias>AS \\[Extent\\d+\\](?! WITH \\(NOLOCK\\)))", RegexOptions.IgnoreCase | RegexOptions.Multiline)]

			//2. 使用static和partial进行声明
			public static partial Regex MyRegex();

			[Benchmark(Baseline = true)]
			public void Before()
			{
				int sum = 0;
				for (int i = 0; i < Count; i++)
				{
					var result = TableAliasRegex.Replace(Input!, "${tableAlias} WITH (NOLOCK)");
					if (!string.IsNullOrWhiteSpace(result))
					{
						sum += result.Length;
					}
				}
			}

			[Benchmark]
			public void After()
			{
				int sum = 0;
				for (int i = 0; i < Count; i++)
				{
					var result = MyRegex().Replace(Input!, "${tableAlias} WITH (NOLOCK)");
					if (string.IsNullOrWhiteSpace(result))
					{
						sum += result.Length;
					}
				}
			}
		}
	}
}
