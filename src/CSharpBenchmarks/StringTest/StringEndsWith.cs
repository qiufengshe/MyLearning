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

		public void OkTest()
		{
			//以往这样写的代码
			if (value[0] == Chars)
			{

			}
			if (value[value.Length - 1] == Chars)
			{

			}

			//⬇⬇都可以分别有StartsWith和EndsWith
			if (value.StartsWith(Chars))
			{

			}
			if (value.EndsWith(Chars))
			{

			}
		}
	}
}
