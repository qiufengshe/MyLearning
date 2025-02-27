using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	//dotnet run -c Release -f net9.0 --filter="*StackArrayTest*" --runtimes net9.0 net10.0
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class StackArrayTest
	{
		[Benchmark]
		public int SumTest()
		{
			int[] arr = [1, 2, 3, 5];
			int sum = 0;
			foreach (var num in arr)
			{
				sum += num;
			}
			return sum;
		}
	}
}