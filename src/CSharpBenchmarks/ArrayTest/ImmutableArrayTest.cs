using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Bogus;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class ImmutableArrayTest
	{
		public People[] p1;


		[Params(100, 1000)]
		public int Count { get; set; }

		[GlobalSetup]
		public void Setup()
		{
			p1 = new Faker<People>("zh_CN")
					.RuleFor(x => x.Id, x => x.IndexFaker += 1)
					.RuleFor(x => x.Name, y => y.Person.LastName + y.Person.FirstName)
					.UseSeed(1000)
					.Generate(Count)
					.ToArray();
		}


		[Benchmark]
		public ImmutableArray<People> NewImmutableArrayTest1()
		{
			return p1.ToImmutableArray();
		}

		[Benchmark]
		public ImmutableArray<People> NewImmutableArrayTest2()
		{
			return Unsafe.As<People[], ImmutableArray<People>>(ref p1);
		}

#if NET8_0_OR_GREATER
        [Benchmark]
        public ImmutableArray<People> NewImmutableArrayTest3()
        {
            return ImmutableCollectionsMarshal.AsImmutableArray(p1);
        }
#endif

		[GlobalCleanup]
		public void Cleanup()
		{
			p1 = null;
		}
	}
}
