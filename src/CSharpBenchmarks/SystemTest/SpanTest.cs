//using System;
//using System.Runtime.CompilerServices;
//using System.Runtime.Intrinsics;
//using System.Runtime.Intrinsics.X86;
//using BenchmarkDotNet.Attributes;

//namespace CSharpBenchmarks.SystemTest
//{
//    [MemoryDiagnoser]
//    [DisassemblyDiagnoser(printSource: true)]
//    public class SpanTest
//    {
//        public int[] arr = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

//        [Benchmark]
//        public void Reverse()
//        {
//            int sum;

//            for (int i = 0; i < 1024; i++)
//            {
//                Array.Reverse(arr);
//                sum = 0;
//                for (int j = 0; j < arr.Length; j++)
//                {
//                    sum += arr[j];
//                }
//            }
//            //int len = arr.Length;
//            //for (int i = 0; i < 1024; i++)
//            //{
//            //    Reverse(ref arr[0], len);
//            //    sum = 0;
//            //    for (int j = 0; j < arr.Length; j++)
//            //    {
//            //        sum += arr[j];
//            //    }
//            //}

//        }


//        public static void Reverse(ref int buf, nuint length)
//        {
//            if (Avx2.IsSupported && (nuint)Vector256<int>.Count * 2 <= length)
//            {
//                nuint numElements = (nuint)Vector256<int>.Count;
//                nuint numIters = length / numElements / 2;
//                Vector256<int> reverseMask = Vector256.Create(7, 6, 5, 4, 3, 2, 1, 0);
//                for (nuint i = 0; i < numIters; i++)
//                {
//                    nuint firstOffset = i * numElements;
//                    nuint lastOffset = length - ((1 + i) * numElements);

//                    // Load the values into vectors
//                    Vector256<int> tempFirst = LoadUnsafe(ref buf, firstOffset);
//                    Vector256<int> tempLast = LoadUnsafe(ref buf, lastOffset);

//                    // Permute to reverse each vector:
//                    //     +-------------------------------+
//                    //     | A | B | C | D | E | F | G | H |
//                    //     +-------------------------------+
//                    //         --->
//                    //     +-------------------------------+
//                    //     | H | G | F | E | D | C | B | A |
//                    //     +-------------------------------+
//                    tempFirst = Avx2.PermuteVar8x32(tempFirst, reverseMask);
//                    tempLast = Avx2.PermuteVar8x32(tempLast, reverseMask);

//                    // Store the values into final location
//                    tempLast.StoreUnsafe(ref buf, firstOffset);
//                    tempFirst.StoreUnsafe(ref buf, lastOffset);
//                }
//                buf = ref Unsafe.Add(ref buf, numIters * numElements);
//                length -= numIters * numElements * 2;
//            }
//            else if (Sse2.IsSupported && (nuint)Vector128<int>.Count * 2 <= length)
//            {
//                nuint numElements = (nuint)Vector128<int>.Count;
//                nuint numIters = length / numElements / 2;
//                for (nuint i = 0; i < numIters; i++)
//                {
//                    nuint firstOffset = i * numElements;
//                    nuint lastOffset = length - ((1 + i) * numElements);

//                    // Load the values into vectors
//                    Vector128<int> tempFirst = VectorTest.LoadUnsafe(ref buf, firstOffset);
//                    Vector128<int> tempLast = VectorTest.LoadUnsafe(ref buf, lastOffset);

//                    // Shuffle to reverse each vector:
//                    //     +---------------+
//                    //     | A | B | C | D |
//                    //     +---------------+
//                    //          --->
//                    //     +---------------+
//                    //     | D | C | B | A |
//                    //     +---------------+
//                    tempFirst = Sse2.Shuffle(tempFirst, 0b00_01_10_11);
//                    tempLast = Sse2.Shuffle(tempLast, 0b00_01_10_11);

//                    // Store the values into final location
//                    tempLast.StoreUnsafe(ref buf, firstOffset);
//                    tempFirst.StoreUnsafe(ref buf, lastOffset);
//                }
//                buf = ref Unsafe.Add(ref buf, numIters * numElements);
//                length -= numIters * numElements * 2;
//            }

//            // Store any remaining values one-by-one
//            for (nuint i = 0; i < (length / 2); i++)
//            {
//                ref int firstInt = ref Unsafe.Add(ref buf, i);
//                ref int lastInt = ref Unsafe.Add(ref buf, length - 1 - i);
//                (lastInt, firstInt) = (firstInt, lastInt);
//            }
//        }



//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static Vector256<T> LoadUnsafe<T>(ref T source, nuint elementOffset)
//            where T : struct
//        {
//            source = ref Unsafe.Add(ref source, (nint)elementOffset);
//            return Unsafe.ReadUnaligned<Vector256<T>>(ref Unsafe.As<T, byte>(ref source));
//        }

//    }

//    public static class VectorTest
//    {
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static Vector128<T> LoadUnsafe<T>(ref T source, nuint elementOffset)
//            where T : struct
//        {
//            source = ref Unsafe.Add(ref source, (nint)elementOffset);
//            return Unsafe.ReadUnaligned<Vector128<T>>(ref Unsafe.As<T, byte>(ref source));
//        }


//    }
//}

////namespace System.Runtime.Intrinsics
////{
////    public static class Vector2561
////    {
////        [MethodImpl(MethodImplOptions.AggressiveInlining)]
////        public static void StoreUnsafe<T>(this Vector256<T> source, ref T destination, nuint elementOffset)
////            where T : struct
////        {
////            destination = ref Unsafe.Add(ref destination, (nint)elementOffset);
////            Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
////        }

////        public static void StoreUnsafe<T>(this Vector128<T> source, ref T destination, nuint elementOffset)
////          where T : struct
////        {
////            //ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
////            destination = ref Unsafe.Add(ref destination, (nint)elementOffset);
////            Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
////        }
////    }
////}

////namespace System.Runtime.CompilerServices
////{
////    public static unsafe partial class Unsafe
////    {
////        public static ref T Add<T>(ref T source, IntPtr elementOffset)
////        {
////#if CORECLR
////            typeof(T).ToString(); // Type token used by the actual method body
////            throw new PlatformNotSupportedException();
////#else
////            return ref AddByteOffset(ref source, (IntPtr)(elementOffset * (nint)SizeOf<T>()));
////#endif

////            // ldarg .0
////            // ldarg .1
////            // sizeof !!T
////            // mul
////            // add
////            // ret
////        }
////    }
////}