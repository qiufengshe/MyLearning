using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace CSharpBenchmarks.SpanTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	public class IndexOfTest
	{
		public byte[] bytes = new byte[] { 0, 0, 0, 0, 71, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 71, 0, 1, 0, 0, 0, 0, 1, 1, 0, 2, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0 };

		public byte[] searchBytes = new byte[] { 0, 1, 0, 0 };

		[Params(1024, 2048)]
		public int Times { get; set; }



		[Benchmark]
		public int SpanIndexOf()
		{
			int sum = 0;
			for (int i = 0; i < Times; i++)
			{
				sum += bytes.AsSpan().IndexOf(searchBytes);
			}
			return sum;
		}

		[Benchmark]
		public int SpanLastIndexOf()
		{
			int sum = 0;
			for (int i = 0; i < Times; i++)
			{
				sum += bytes.AsSpan().LastIndexOf(searchBytes);
			}
			return sum;
		}

		[Benchmark]
		public int GetSpanLength()
		{
			var arr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
			return arr.AsSpan().Length;
		}
	}
}

////IndeOfValueType源码:
//internal static unsafe int IndexOfValueType<T>(ref T searchSpace, T value, int length) where T : struct, IEquatable<T>
//{
//    Debug.Assert(length >= 0);

//    nint index = 0; // Use nint for arithmetic to avoid unnecessary 64->32->64 truncations
//    if (Vector.IsHardwareAccelerated && Vector<T>.IsTypeSupported && (Vector<T>.Count * 2) <= length)
//    {
//        Vector<T> valueVector = new Vector<T>(value);
//        Vector<T> compareVector = default;
//        Vector<T> matchVector = default;
//        if ((uint)length % (uint)Vector<T>.Count != 0)
//        {
//            // Number of elements is not a multiple of Vector<T>.Count, so do one
//            // check and shift only enough for the remaining set to be a multiple
//            // of Vector<T>.Count.
//            compareVector = Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref searchSpace, index));
//            matchVector = Vector.Equals(valueVector, compareVector);
//            if (matchVector != Vector<T>.Zero)
//            {
//                goto VectorMatch;
//            }
//            index += length % Vector<T>.Count;
//            length -= length % Vector<T>.Count;
//        }
//        while (length > 0)
//        {
//            compareVector = Unsafe.As<T, Vector<T>>(ref Unsafe.Add(ref searchSpace, index));
//            matchVector = Vector.Equals(valueVector, compareVector);
//            if (matchVector != Vector<T>.Zero)
//            {
//                goto VectorMatch;
//            }
//            index += Vector<T>.Count;
//            length -= Vector<T>.Count;
//        }
//        goto NotFound;
//    VectorMatch:
//        for (int i = 0; i < Vector<T>.Count; i++)
//            if (compareVector[i].Equals(value))
//                return (int)(index + i);
//    }

//    while (length >= 8)
//    {
//        if (value.Equals(Unsafe.Add(ref searchSpace, index)))
//            goto Found;
//        if (value.Equals(Unsafe.Add(ref searchSpace, index + 1)))
//            goto Found1;
//        if (value.Equals(Unsafe.Add(ref searchSpace, index + 2)))
//            goto Found2;
//        if (value.Equals(Unsafe.Add(ref searchSpace, index + 3)))
//            goto Found3;
//        if (value.Equals(Unsafe.Add(ref searchSpace, index + 4)))
//            goto Found4;
//        if (value.Equals(Unsafe.Add(ref searchSpace, index + 5)))
//            goto Found5;
//        if (value.Equals(Unsafe.Add(ref searchSpace, index + 6)))
//            goto Found6;
//        if (value.Equals(Unsafe.Add(ref searchSpace, index + 7)))
//            goto Found7;

//        length -= 8;
//        index += 8;
//    }

//    while (length >= 4)
//    {
//        if (value.Equals(Unsafe.Add(ref searchSpace, index)))
//            goto Found;
//        if (value.Equals(Unsafe.Add(ref searchSpace, index + 1)))
//            goto Found1;
//        if (value.Equals(Unsafe.Add(ref searchSpace, index + 2)))
//            goto Found2;
//        if (value.Equals(Unsafe.Add(ref searchSpace, index + 3)))
//            goto Found3;

//        length -= 4;
//        index += 4;
//    }

//    while (length > 0)
//    {
//        if (value.Equals(Unsafe.Add(ref searchSpace, index)))
//            goto Found;

//        index += 1;
//        length--;
//    }
//NotFound:
//    return -1;

//Found: // Workaround for https://github.com/dotnet/runtime/issues/8795
//    return (int)index;
//Found1:
//    return (int)(index + 1);
//Found2:
//    return (int)(index + 2);
//Found3:
//    return (int)(index + 3);
//Found4:
//    return (int)(index + 4);
//Found5:
//    return (int)(index + 5);
//Found6:
//    return (int)(index + 6);
//Found7:
//    return (int)(index + 7);
//}

//BenchmarkDotNet = v0.13.1.1796 - nightly, OS = Microsoft Windows 10.0.22000 Microsoft Windows NT 10.0.22000.0
//Intel Core i9-10900 CPU 2.50GHz, 1 CPU, 20 logical and 10 physical cores
//.NET SDK=7.0.100-preview.4.22252.9
//  [Host]     : .NET 6.0.5(6.0.522.21309), X64 RyuJIT
//  Job-DEQWDN : .NET 7.0.0(7.0.22.22904), X64 RyuJIT
//  Job-JXNRNG : .NET 6.0.5(6.0.522.21309), X64 RyuJIT


//|          Method |        Job |  Runtime | Toolchain | Times |      Mean |     Error |    StdDev | Ratio    | Code Size  | Allocated | Alloc Ratio |
//|---------------- |----------- |--------- |---------- |------ |----------:| ----------:| ----------:| ------:| ----------:| ----------:| ------------:|
//| SpanIndexOf     |Job - DEQWDN| .NET 7.0 | net7.0    | 1024 | 7.945 us   | 0.0209 us | 0.0174 us   | 0.29   | 845 B      | - | NA |
//| SpanIndexOf     |Job - JXNRNG| .NET 6.0 | net6.0    | 1024 | 27.764 us  | 0.0717 us | 0.0599 us   | 1.00   | 268 B      | - | NA |
//|                 |            |          |           |            |           |           |             |         |            |           |             |
//| SpanLastIndexOf |Job - DEQWDN| .NET 7.0 | net7.0    | 1024 | 7.944 us   | 0.0419 us | 0.0392 us   | 0.64   | 832 B      | - | NA |
//| SpanLastIndexOf |Job - JXNRNG| .NET 6.0 | net6.0    | 1024 | 12.323 us  | 0.0187 us | 0.0156 us   | 1.00   | 268 B      | - | NA |
//|                 |            |          |           |       |           |           |             |         |                |           |             |
//| SpanIndexOf     |Job - DEQWDN| .NET 7.0 | net7.0    | 2048 | 16.271 us  | 0.0943 us | 0.0836 us   | 0.29   | 845 B      | - | NA |
//| SpanIndexOf     |Job - JXNRNG| .NET 6.0 | net6.0    | 2048 | 56.439 us  | 0.1949 us | 0.1727 us   | 1.00   | 268 B      | - | NA |
//|                 |            |          |           |       |           |           |             |         |                |           |             |
//| SpanLastIndexOf |Job - DEQWDN| .NET 7.0 | net7.0    | 2048 | 15.856 us  | 0.0475 us | 0.0397 us   | 0.64   | 832 B      | - | NA |
//| SpanLastIndexOf |Job - JXNRNG| .NET 6.0 | net6.0    | 2048 | 24.737 us  | 0.3226 us | 0.2694 us   | 1.00   | 268 B      | - | NA |
