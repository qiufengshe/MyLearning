using System;
using System.Buffers;
using BenchmarkDotNet.Attributes;

namespace net6perf
{
    [MemoryDiagnoser]
    [DisassemblyDiagnoser(printSource: true)]
    public class StringCreateTest
    {
        [Params(4096)]
        public int Count { get; set; }

        public static string OrderNum = "FA210609";

        [Benchmark(Baseline = true)]
        public void ArrayTest()
        {
            for (int i = 0; i < Count; i++)
            {
                Array(OrderNum);
            }
        }

        [Benchmark]
        public void CreateTest()
        {
            for (int i = 0; i < Count; i++)
            {
                Create(OrderNum);
            }
        }

        [Benchmark]
        public void LocalCreateTest()
        {
            for (int i = 0; i < Count; i++)
            {
                LocalCreate(OrderNum);
            }
        }

        [Benchmark]
        public void StaticCreateTest()
        {
            for (int i = 0; i < Count; i++)
            {
                StaticCreate(OrderNum);
            }
        }


        [Benchmark]
        public void CreateCaheTest()
        {
            for (int i = 0; i < Count; i++)
            {
                CreateCache(OrderNum);
            }
        }

        public string Array(string input)
        {
            int len = input.Length;
            int postion = -1;
            char[] arr = new char[len];
            for (int i = 0; i < len; i++)
            {
                if (input[i] >= 'A' && input[i] <= 'Z')
                {
                    arr[i] = input[i];
                }
                else
                {
                    postion = i;
                    break;
                }
            }

            if (postion > 0)
            {
                arr[postion] = 'R';
            }

            int start = postion + 1;
            for (int i = start; i < len; i++)
            {
                arr[i] = input[i];
            }

            return new string(arr, 0, len);
        }

        public string Create(string input)
        {
            int len = input.Length;

            return string.Create(len, input, (target, src) =>
            {
                int postion = -1;
                for (int i = 0; i < len; i++)
                {
                    if (src[i] >= 'A' && src[i] <= 'Z')
                    {
                        target[i] = src[i];
                    }
                    else
                    {
                        postion = i;
                        break;
                    }
                }

                if (postion > 0)
                {
                    target[postion] = 'R';
                }

                int start = postion + 1;
                for (int i = start; i < len; i++)
                {
                    target[i] = src[i];
                }
            });
        }

        public string LocalCreate(string input)
        {
            int len = input.Length;

            return string.Create(len, input, (target, src) =>
            {
                int postion = -1;
                int strLen = target.Length;
                for (int i = 0; i < strLen; i++)
                {
                    if (src[i] >= 'A' && src[i] <= 'Z')
                    {
                        target[i] = src[i];
                    }
                    else
                    {
                        postion = i;
                        break;
                    }
                }

                if (postion > 0)
                {
                    target[postion] = 'R';
                }

                int start = postion + 1;
                for (int i = start; i < strLen; i++)
                {
                    target[i] = src[i];
                }
            });
        }

        public string StaticCreate(string input)
        {
            int len = input.Length;

            return string.Create(len, input, static (target, src) =>
             {
                 int postion = -1;
                 int strLen = target.Length;
                 for (int i = 0; i < strLen; i++)
                 {
                     if (src[i] >= 'A' && src[i] <= 'Z')
                     {
                         target[i] = src[i];
                     }
                     else
                     {
                         postion = i;
                         break;
                     }
                 }

                 if (postion > 0)
                 {
                     target[postion] = 'R';
                 }

                 int start = postion + 1;
                 for (int i = start; i < strLen; i++)
                 {
                     target[i] = src[i];
                 }
             });
        }

        public void CreateAction(Span<char> target, string src)
        {
            int postion = -1;
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] >= 'A' && src[i] <= 'Z')
                {
                    target[i] = src[i];
                }
                else
                {
                    postion = i;
                    break;
                }
            }

            if (postion > 0)
            {
                target[postion] = 'R';
            }

            int start = postion + 1;
            for (int i = start; i < src.Length; i++)
            {
                target[i] = src[i];
            }
        }

        //给Create所需的委托加缓存,而不是每次都创建新的委托实例
        public SpanAction<char, string> spanAction = null;
        public string CreateCache(string input)
        {
            int len = input.Length;

            if (spanAction == null) //只有为空的,才会创建委托实例
            {
                spanAction = CreateAction; //缓存委托实例
            }
            return string.Create(len, input, spanAction);
        }
    }
}
