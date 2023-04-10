using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.Syntax
{
#if NET7_0_OR_GREATER
    [DisassemblyDiagnoser(printSource: true, maxDepth: 3)]
    [MemoryDiagnoser]
    public partial class RegexTest
    {
        [Params(1024, 2048, 4096)]
        public int Count { get; set; }

        [Params("qwe", "abc", "xyz")]
        public string Input { get; set; }

        public Regex regex = new Regex(@"abc|def", RegexOptions.IgnoreCase);

        //在.Net 7加入 正则表达式代码生成,简单不单独说明怎么使用
        //1. 使用RegexGenerator特性
        [GeneratedRegex(@"abc|def", RegexOptions.IgnoreCase)]

        //2. 使用static和partial进行声明
        public static partial Regex MyRegex();



        [Benchmark(Baseline = true)]
        public void Before()
        {
            int sum = 0;
            for (int i = 0; i < Count; i++)
            {
                if (regex.IsMatch(Input!))
                {
                    sum += i;
                }
            }
        }

        [Benchmark]
        public void After()
        {
            int sum = 0;
            for (int i = 0; i < Count; i++)
            {
                if (MyRegex().IsMatch(Input!))
                {
                    sum += i;
                }
            }
        }
    }
#endif
}
