using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class ArrayTest
	{
		[Params(10000)]
		public int Times { get; set; }

		[Benchmark]
		public void Array1Test()
		{
			const int arrLenth = 10000;
			int[,] arr = new int[arrLenth, arrLenth];
			for (int row = 0; row < arrLenth; row++)
			{
				for (int col = 0; col < arrLenth; col++)
				{
					arr[row, col] = col + 1;
				}
			}

			long total = 0;
			for (int row = 0; row < arrLenth; row++)
			{
				for (int col = 0; col < arrLenth; col++)
				{
					total += arr[row, col];
				}
			}
		}

		[Benchmark]
		public void Array2Test()
		{
			int[][] arr = new int[Times][];
			for (int row = 0; row < Times; row++)
			{
				arr[row] = new int[Times];
				for (int col = 0; col < Times; col++)
				{
					arr[row][col] = col + 1;
				}
			}

			long total = 0;
			for (int row = 0; row < Times; row++)
			{
				for (int col = 0; col < Times; col++)
				{
					total += arr[row][col];
				}
			}
		}
	}
}
