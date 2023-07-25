using System;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.SpanTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class IndexOfAnyExceptTest
	{
		[Params(1024, 2048)]
		public int Times { get; set; }


		[Params("hello world\t", "\thello csharp")]
		public string Chars { get; set; }

		[Benchmark]
		public int ForIndexOfTest()
		{
			int sum = 0;
			for (int i = 0; i < Times; i++)
			{
				for (int j = 0; j < Chars?.Length; j++)
				{
					char c = Chars[j];
					if (c == '\t' || c == '\n' || c == '\r')
					{
						sum += 1;   //这里只是不让JIT将代码优化掉
						break;
					}
				}
			}
			return sum;
		}

		[Benchmark]
		public int SpanIndexOfAnyTest()
		{
			int sum = 0;
			for (int i = 0; i < Times; i++)
			{
				if (Chars.AsSpan().IndexOfAny("\t\r\n") > -1)
				{
					sum += 1;       //这里只是不让JIT将代码优化掉
				}
			}
			return sum;
		}

		[Benchmark]
		public int ForNoContainsTest()
		{
			int sum = 0;
			for (int i = 0; i < Times; i++)
			{
				for (int j = 0; j < Chars?.Length; j++)
				{
					char c = Chars[j];
					if (c != '\t' || c != '\n' || c != '\r')
					{
						sum += 1;    //这里只是不让JIT将代码优化掉
						break;
					}
				}
			}
			return sum;
		}

		[Benchmark]
		public int SpanIndexOfAnyExceptTest()
		{
			int sum = 0;
			for (int i = 0; i < Times; i++)
			{
				if (Chars.AsSpan().IndexOfAnyExcept("\t\r\n") > -1)
				{
					sum += 1;       //这里只是不让JIT将代码优化掉
				}
			}
			return sum;
		}
	}
}
