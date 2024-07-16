using System.Reflection;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.UnsafeTest
{
	public class ReflectionTest
	{
		[Params(10240, 20480)]
		public int Count { get; set; }

		[Benchmark]
		public void Test1()
		{
			People people = new();
			FieldInfo fieldInfo = typeof(People).GetField("age", BindingFlags.NonPublic | BindingFlags.Instance);
			for (int i = 0; i < Count; i++)
			{
				int age = (int)fieldInfo.GetValue(people);
				age += 1;
				fieldInfo.SetValue(people, age);
			}
		}

		[Benchmark]
		public void Test2()
		{
			People people = new();
			for (int i = 0; i < Count; i++)
			{
				ref int age = ref GetAge(people);
				age += 1;
			}
		}

		[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "age")]
		extern ref int GetAge(People people);
	}

	public class People
	{
		private int age = 0;
	}
}
