using System;
using System.Runtime.CompilerServices;

namespace CharpLearning.Syntax
{
    public class GCAddApi
    {
        public void ArrayTest()
        {
            People[] peoples = new People[16];
            Console.WriteLine(peoples.Length);
        }

        public void AllocateArrayTest()
        {
            //从.Net 5开始支持
            //AllocateArray 在托管堆根据长度分配数组,对数组对象分配默认值
            //第一个参数是需要分配数组的长度
            //第二个参数是否在GC中固定
            People[] arr = GC.AllocateArray<People>(16);
            Console.WriteLine(arr.Length);
        }

        /// <summary>
        /// Allocate an array.
        /// </summary>
        /// <typeparam name="T">Specifies the type of the array element.</typeparam>
        /// <param name="length">Specifies the length of the array.</param>
        /// <param name="pinned">Specifies whether the allocated array must be pinned.</param>
        /// <remarks>
        /// If pinned is set to true, <typeparamref name="T"/> must not be a reference type or a type that contains object references.
        /// </remarks>
        //public static T[] AllocateArray<T>(int length, bool pinned = false) // T[] rather than T?[] to match `new T[length]` behavior
        //{
        //    GC_ALLOC_FLAGS flags = GC_ALLOC_FLAGS.GC_ALLOC_NO_FLAGS;

        //    if (pinned)
        //    {
        //        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        //            ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(T));

        //        flags = GC_ALLOC_FLAGS.GC_ALLOC_PINNED_OBJECT_HEAP;     //是否需要在托管堆上固定
        //    }

        //    //内部使用AllocateNewArray,这个API并不是c#实现
        //    return Unsafe.As<T[]>(AllocateNewArray(typeof(T[]).TypeHandle.Value, length, flags));
        //}

        //[MethodImpl(MethodImplOptions.InternalCall)] //表明AllocateNewArray是在CLR内部实现
        //internal static extern Array AllocateNewArray(IntPtr typeHandle, int length, GC_ALLOC_FLAGS flags);

        public void AllocateUninitializedArrayTest()
        {
            //从.Net 5开始支持
            //AllocateUninitializedArray 在托管堆根据长度分配数组,不对对象的属性分配默认值
            //第一个参数是需要分配数组的长度
            //第二个参数是否在GC中固定
            People[] arr = GC.AllocateUninitializedArray<People>(16);
            Console.WriteLine(arr.Length);
        }

        /// <summary>
        /// Allocate an array while skipping zero-initialization if possible.
        /// </summary>
        /// <typeparam name="T">Specifies the type of the array element.</typeparam>
        /// <param name="length">Specifies the length of the array.</param>
        /// <param name="pinned">Specifies whether the allocated array must be pinned.</param>
        /// <remarks>
        /// If pinned is set to true, <typeparamref name="T"/> must not be a reference type or a type that contains object references.
        /// </remarks>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)] // forced to ensure no perf drop for small memory buffers (hot path)
        //public static T[] AllocateUninitializedArray<T>(int length, bool pinned = false) // T[] rather than T?[] to match `new T[length]` behavior
        //{
        //            if (!pinned)            //不固定在堆上固定对象
        //            {
        //                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())  //如果是引用类型,直接根据类型使用直接用分组的方式
        //                {
        //                    return new T[length];
        //                }

        //                // for debug builds we always want to call AllocateNewArray to detect AllocateNewArray bugs
        //#if !DEBUG
        //        // small arrays are allocated using `new[]` as that is generally faster.
        //        if (length < 2048 / Unsafe.SizeOf<T>())                 //数组长度较小的情况,也是使用直接的方式在堆上分配数组
        //        {
        //            return new T[length];
        //        }
        //#endif
        //            }
        //            else if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())   //如果该类型是引用类型并包含其他引用类型,会抛出异常
        //            {
        //                ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(T));
        //            }

        //            // kept outside of the small arrays hot path to have inlining without big size growth
        //            return AllocateNewUninitializedArray(length, pinned);

        //            // remove the local function when https://github.com/dotnet/runtime/issues/5973 is implemented
        //            static T[] AllocateNewUninitializedArray(int length, bool pinned)
        //            {
        //                GC_ALLOC_FLAGS flags = GC_ALLOC_FLAGS.GC_ALLOC_ZEROING_OPTIONAL;
        //                if (pinned)
        //                    flags |= GC_ALLOC_FLAGS.GC_ALLOC_PINNED_OBJECT_HEAP;

        //                return Unsafe.As<T[]>(AllocateNewArray(typeof(T[]).TypeHandle.Value, length, flags)); //重点还是AllocateNewArray
        //            }
        //}

        public void GetAllocatedBytesForCurrentThreadTest()
        {
            long currentThreadAllocateBytes = GC.GetAllocatedBytesForCurrentThread(); //线程的生存期内在托管堆上分配的总字节数
            Console.WriteLine(currentThreadAllocateBytes);
        }

    }
}
