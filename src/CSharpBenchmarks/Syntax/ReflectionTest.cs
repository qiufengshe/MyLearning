using System;
using System.Linq.Expressions;
using System.Net.Http;
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
		private static Func<HttpContent, bool>? isBuffered;

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

		public void ExpressionTest()
		{
			var property = typeof(HttpContent).GetProperty("IsBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
			if (property != null)
			{
				isBuffered = CreateGetFunc<HttpContent, bool>(property.DeclaringType, property.Name, property.PropertyType);
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

		public static Func<TDeclaring, TProperty> CreateGetFunc<TDeclaring, TProperty>(Type declaringType, string propertyName, Type? propertyType = null)
		{
			var paramInstance = Expression.Parameter(typeof(TDeclaring));
			var bodyInstance = (Expression)paramInstance;
			if (typeof(TDeclaring) != declaringType)
			{
				bodyInstance = Expression.Convert(bodyInstance, declaringType);
			}
			var bodyProperty = (Expression)Expression.Property(bodyInstance, propertyName);
			if (typeof(TProperty) != propertyType)
			{
				bodyProperty = Expression.Convert(bodyProperty, propertyType);
			}
			return Expression.Lambda<Func<TDeclaring, TProperty>>(bodyProperty, paramInstance).Compile();
		}
	}
}
