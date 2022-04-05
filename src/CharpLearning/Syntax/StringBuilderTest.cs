using System;
using System.Diagnostics;
using System.Text;

namespace CharpLearning.Syntax
{
    public class StringBuilderTest
    {
        private StringBuilder InsertSpanFormattable<T>(int index, T value) where T : ISpanFormattable
        {
            Debug.Assert(typeof(T).Assembly.Equals(typeof(object).Assembly),
                    "Implementation trusts the results of TryFormat because T is expected to be something known");

            Span<char> buffer = stackalloc char[256];  //在栈上分配内存
                                                       //由每个值类型的TryFormat转为字符串
            if (value.TryFormat(buffer, out int charsWritten, format: default, provider: null))
            {
                //通过接收string参数的Insert添加到内部的字符数组
                return Insert(index, buffer.Slice(0, charsWritten), 1);
            }

            return Insert(index, value.ToString(), 1);
        }

        public StringBuilder Insert(int index, string? value, int count) => Insert(index, value.AsSpan(), count);

        private StringBuilder Insert(int index, ReadOnlySpan<char> value, int count)
        {
            return null;
        }
    }
}
