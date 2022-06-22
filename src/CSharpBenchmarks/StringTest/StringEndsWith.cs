using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.StringTest
{
    [MemoryDiagnoser]
    [DisassemblyDiagnoser(printSource: true)]
    public class StringEndsWith
    {
        string value = "this, is, a, very long string, with some spaces, commas and more spaces";

        [Params(1024, 2048)]
        public int Times { get; set; }

        [Params('t', 's')]
        public char Chars { get; set; }

        [Benchmark]
        public void EndsWithTest()
        {
            int count = 0;
            for (int i = 0; i < Times; i++)
            {
                if (value.EndsWith(Chars))
                {
                    count += 1;
                }
            }
        }

        [Benchmark]
        public void StartsWithTest()
        {
            int count = 0;
            for (int i = 0; i < Times; i++)
            {
                if (value.StartsWith(Chars))
                {
                    count += 1;
                }
            }
        }
    }
}
