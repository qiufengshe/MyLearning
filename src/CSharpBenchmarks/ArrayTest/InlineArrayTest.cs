using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class InlineArrayTest
	{
	}
#if NET8_0_OR_GREATER
	[InlineArray(TestLength)]
	public struct Test1
	{
		public const int TestLength = 10;
		public int x;
	}
#endif
}
