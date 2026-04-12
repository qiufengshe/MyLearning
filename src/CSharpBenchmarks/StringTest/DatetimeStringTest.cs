using System;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.StringTest;

[DisassemblyDiagnoser(printSource: true, maxDepth: 3)]
[MemoryDiagnoser]
public class DatetimeStringTest
{
	[Params(128, 1024)]
	public int Count { get; set; }


	[Benchmark(Baseline = true)]
	public void Deconstruct()
	{
		var sum = 0;
		for (var i = 0; i < Count; i++)
		{
			var ((year, month, day), (hour, minute, second)) = DateTime.Now;
			var s = $"{year}-{month}-{day} {hour}:{minute}:{second}";
			sum += s.Length;
		}
	}

	[Benchmark]
	public void String()
	{
		var sum = 0;
		for (var i = 0; i < Count; i++)
		{
			var s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			sum += s.Length;
		}
	}
}