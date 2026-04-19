using System;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.DateTimeTest;


[MemoryDiagnoser]
[DisassemblyDiagnoser(printSource: true)]
public class UnixTimeSecondsTest
{
	private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	[Params(128, 1024)]
	public int Count { get; set; }


	[Benchmark(Baseline = true)]
	public void ToUnixTimeSeconds1()
	{
		var sum = 0L;
		for (int i = 0; i < Count; i++)
		{
			var s = (long)(DateTime.UtcNow - Jan1st1970).TotalSeconds;
			if (s > 0)
			{
				sum += 1;
			}

		}
	}

	[Benchmark]
	public void ToUnixTimeSeconds2()
	{
		var sum = 0L;
		for (int i = 0; i < Count; i++)
		{
			var s = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			if (s > 0)
			{
				sum += 1;
			}
		}
	}

	//通过使用DateTimeOffset.UtcNow.ToUnixTimeSeconds()方法，可以直接获取当前时间的Unix时间戳，和使用DateTime.UtcNow减去Unix纪元时间再转换为秒数的方法相比，更加简洁和高效。
	//性能方面相差不大
}
