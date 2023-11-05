using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class NewSyntaxTest
	{
		[Params(10000)]
		public int Times { get; set; }

		[Benchmark]
		public void ArrayTest1()
		{
			for (int i = 0; i < Times; i++)
			{
				int[] arr = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
				if (arr.Length > 0)
				{

				}
			}
		}

		[Benchmark]
		public void ArrayTest2()
		{
			for (int i = 0; i < Times; i++)
			{
				int[] arr = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
				if (arr.Length > 0)
				{

				}
			}
		}
	}
}
