using System;
using System.Collections.Immutable;

namespace CharpLearning.Syntax
{
    public class ImmutableArrayTest
    {
        public void Test1()
        {
            Action<ImmutableArray<int>> printAction = static (ImmutableArray<int> arr) =>
            {
                foreach (var item in arr)
                {
                    Console.Write($"{item} ");
                }
                Console.WriteLine();
            };

            int[] arr1 = { 1, 2, 3 };
            Span<int> span1 = arr1.AsSpan();

            ImmutableArray<int> immutableArray1 = ImmutableArray.Create(span1); //接受Span类型参数
            printAction(immutableArray1);

            int[] arr2 = new int[] { 4, 5, 6 };
            Span<int> span2 = arr2.AsSpan();
            ImmutableArray<int> immutableArray2 = immutableArray1.InsertRange(immutableArray1.Length, span2); //指定下标,将span加入ImmutableArray中
            printAction(immutableArray2);

            ImmutableArray<int> immutableArray3 = span2.ToImmutableArray();  //Span返回ImmutableArray
            printAction(immutableArray3);

            ImmutableArray<int> immutableArray4 = immutableArray3.Slice(0, 1); //切片取第一个元素
            printAction(immutableArray4);

            int[] arr3 = new int[] { 7, 8, 9 };
            ImmutableArray<int> immutableArray5 = immutableArray3.AddRange(new ReadOnlySpan<int>(arr3)); //将Span添加到ImmutableArray
            printAction(immutableArray5);

            int[] arr4 = new int[3];
            ImmutableArray.Create(1, 2, 3).ToBuilder().CopyTo(arr4); //从ImmutableArray的数据拷贝到数组中
        }
    }
}
