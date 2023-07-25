using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;

namespace CSharpBenchmarks.MathTest
{
	[DisassemblyDiagnoser(printSource: true, maxDepth: 3)]
	[MemoryDiagnoser]
	[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.NativeAot70)]
	[HideColumns(Column.Gen2)]  //隐藏GC2代列
								//[AotFilter("不支持NativeAot")]
	public class RoundTest
	{
		[Params(1.5)]
		public double Value { get; set; }

		//[Benchmark]
		//public double ToEvenTest() => Math.Round(Value, MidpointRounding.ToEven);


		//[Benchmark]
		//public double AwayFromZeroTest() => Math.Round(Value, MidpointRounding.AwayFromZero);


		[Benchmark(Baseline = true)]
		public double RoundDefault() => Math.Round(Value);
	}

	//; CSharpBenchmarks.MathTest.RoundTest.RoundDefault()
	//;         public double RoundDefault() => Math.Round(Value);
	//;                                         ^^^^^^^^^^^^^^^^^
	//       vzeroupper
	//       vroundsd  xmm0,xmm0,qword ptr[rcx + 8],4
	//       ret
	//; Total bytes of code 11
}
