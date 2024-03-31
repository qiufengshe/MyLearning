using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class ArrayTest
	{
		[Params(10000)]
		public int Times { get; set; }

		[Benchmark]
		public void Array1Test()
		{
			const int arrLenth = 10000;
			int[,] arr = new int[arrLenth, arrLenth];
			for (int row = 0; row < arrLenth; row++)
			{
				for (int col = 0; col < arrLenth; col++)
				{
					arr[row, col] = col + 1;
				}
			}

			long total = 0;
			for (int row = 0; row < arrLenth; row++)
			{
				for (int col = 0; col < arrLenth; col++)
				{
					total += arr[row, col];
				}
			}
		}

		[Benchmark]
		public void Array2Test()
		{
			int[][] arr = new int[Times][];
			for (int row = 0; row < Times; row++)
			{
				arr[row] = new int[Times];
				for (int col = 0; col < Times; col++)
				{
					arr[row][col] = col + 1;
				}
			}

			long total = 0;
			for (int row = 0; row < Times; row++)
			{
				for (int col = 0; col < Times; col++)
				{
					total += arr[row][col];
				}
			}
		}

		private static T SumCore<T>(ReadOnlySpan<T> source)
where T : struct, INumber<T>
		{
			T sum = T.Zero;

			if (!Vector128.IsHardwareAccelerated || source.Length < Vector128<T>.Count)
			{
				// Not SIMD supported or small source.
				//1. 当硬件不支持,会退变为for循环
				//2. 集合内的数量小于Vector128的支持的数量,如int->4 long->2,会退变for循环
				unchecked // SIMD operation is unchecked so keep same behaviour
				{
					for (int i = 0; i < source.Length; i++)
					{
						sum += source[i];
					}
				}
			}
			else if (!Vector256.IsHardwareAccelerated || source.Length < Vector256<T>.Count)
			{
				// Only 128bit SIMD supported or small source.
				//满足128bit,不足256bit的,数量少的时候

				//获取开始元素
				ref var begin = ref MemoryMarshal.GetReference(source);

				//获取结尾的元素
				ref var last = ref Unsafe.Add(ref begin, source.Length);
				ref var current = ref begin;
				//获取一个初始值为0的Vector128
				var vectorSum = Vector128<T>.Zero;

				//集合的长度减去Vector128<T>的数量,让开始元素进行偏移
				ref var to = ref Unsafe.Add(ref begin, source.Length - Vector128<T>.Count);
				//开始元素的地址是否和小于一次Vector的地址
				while (Unsafe.IsAddressLessThan(ref current, ref to))
				{
					//如果是int类型, 就一次加载前4个元素
					vectorSum += Vector128.LoadUnsafe(ref current);
					current = ref Unsafe.Add(ref current, Vector128<T>.Count); //如果是int类型,偏移4个元素
				}

				//判断current的地址是否小于结尾元素的地址
				//处理不够一次Vector的时候,退变循环处理
				while (Unsafe.IsAddressLessThan(ref current, ref last))
				{
					unchecked // SIMD operation is unchecked so keep same behaviour
					{
						sum += current;
					}
					current = ref Unsafe.Add(ref current, 1); //每次偏移1个元素
				}

				sum += Vector128.Sum(vectorSum); //进行求和计算
			}
			else
			{
				// 256bit SIMD supported
				ref var begin = ref MemoryMarshal.GetReference(source);
				ref var last = ref Unsafe.Add(ref begin, source.Length);
				ref var current = ref begin;
				var vectorSum = Vector256<T>.Zero;

				ref var to = ref Unsafe.Add(ref begin, source.Length - Vector256<T>.Count);
				while (Unsafe.IsAddressLessThan(ref current, ref to))
				{
					vectorSum += Vector256.LoadUnsafe(ref current);
					current = ref Unsafe.Add(ref current, Vector256<T>.Count);
				}
				while (Unsafe.IsAddressLessThan(ref current, ref last))
				{
					unchecked // SIMD operation is unchecked so keep same behaviour
					{
						sum += current;
					}
					current = ref Unsafe.Add(ref current, 1);
				}

				sum += Vector256.Sum(vectorSum);
			}

			return sum;
		}

	}
}
