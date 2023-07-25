using System;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.Syntax
{
	[DisassemblyDiagnoser(printSource: true, maxDepth: 3)]
	public class FunctionPointer
	{

		static int Add(int a, int b)
		{
			return a + b;
		}

		[Benchmark(Baseline = true)]
		public void DelegateTest()
		{
			Func<int, int, int> func = Add;
			for (int i = 0; i < 100; i++)
			{
				int sum = 0;
				for (int j = 0; j < 100000; j++)
				{
					sum += func(1, 2);
				}
			}
		}


		[Benchmark]
		public unsafe void FunctionPointerTest()
		{
			delegate*<int, int, int> func = &Add;
			for (int i = 0; i < 100; i++)
			{
				int sum = 0;
				for (int j = 0; j < 100000; j++)
				{
					sum += func(1, 2);
				}
			}
		}
	}
}
