using System.Reflection;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Bogus;

namespace CSharpBenchmarks.Syntax
{
	[DisassemblyDiagnoser(printSource: true, maxDepth: 3)]
	[MemoryDiagnoser]
	public class ReflectionTest
	{
		[Params(128)]
		public int Count { get; set; }

		[Benchmark]
		public void Refection()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				Person person = new Person();
				sum += ReflectionTest1(person);
			}
		}

		[Benchmark]
		public void UnsafeTest()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var person = new Person();
				ref var age = ref GetAge(person);
				age = 1;
				sum += age;
			}
		}

		public static int ReflectionTest1(Person person)
		{
			var field = typeof(Person).GetField("age", BindingFlags.NonPublic | BindingFlags.Instance);
			field.SetValue(person, 1);
			return (int)field.GetValue(person);
		}

		[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "age")]
		public extern static ref int GetAge(Person person);
	}
}
