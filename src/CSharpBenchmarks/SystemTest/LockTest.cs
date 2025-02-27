﻿#if NET9_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace CSharpBenchmarks.SystemTest
{

	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	public class LockTest
	{
		private readonly static object obj = new();
		private readonly static Lock @lock = new(); //新的方式,不再使用object而是使用Lock

		[Benchmark(Baseline = true)]
		public async Task<long> LockObject()
		{
			long count = 0;
			var tasks = new Task[10];
			for (int i = 0; i < 10; i++)
			{
				tasks[i] = Task.Run(() =>
				{
					for (int j = 0; j < 1000000; j++)
					{
						lock (obj)
						{
							count += 1;
						}
					}
				});
			}

			await Task.WhenAll(tasks);
			return count;
		}

		[Benchmark]
		public async Task<long> LockAPI()
		{
			long count = 0;
			var tasks = new Task[10];
			for (int i = 0; i < 10; i++)
			{
				tasks[i] = Task.Run(() =>
				{

					for (var j = 0; j < 1000000; ++j)
					{
						lock (@lock)  //lock 
						{
							count += 1;
						}
					}
				});
			}
			await Task.WhenAll(tasks);
			return count;
		}
	}
}
#endif