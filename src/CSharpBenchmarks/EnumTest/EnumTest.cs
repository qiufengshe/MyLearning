using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.EnumTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class EnumTest
	{
		[Params(16, 128)]
		public int Count { get; set; }

		[Params(Status.Initial, Status.Processing, Status.Finished)]
		public Status RecordStatus { get; set; }

		[Benchmark]
		public void HasFlag()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				Status status = RecordStatus;
				if (status.HasFlag(Status.Initial))
				{
					sum += 1;
				}
			}
		}
	}

	public enum Status : byte
	{
		Initial = 0,

		Processing = 1,

		Finished = 2
	}

}
