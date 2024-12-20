﻿using System;
using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;

namespace CSharpBenchmarks.StringTest
{
	[DotTraceDiagnoser]
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
				_ = Array(OrderNum);
			}
		}

		[Benchmark]
		public void CreateTest()
		{
			for (int i = 0; i < Count; i++)
			{
				_ = Create(OrderNum);
			}
		}

		[Benchmark]
		public void LocalCreateTest()
		{
			for (int i = 0; i < Count; i++)
			{
				_ = LocalCreate(OrderNum);
			}
		}

		[Benchmark]
		public void StaticCreateTest()
		{
			for (int i = 0; i < Count; i++)
			{
				_ = StaticCreate(OrderNum);
			}
		}


		[Benchmark]
		public void CreateCacheTest()
		{
			for (int i = 0; i < Count; i++)
			{
				_ = CreateCache(OrderNum);
			}
		}

		public string Array(string input)
		{
			int len = input.Length;
			int position = -1;
			char[] arr = new char[len];
			for (int i = 0; i < len; i++)
			{
				if (input[i] >= 'A' && input[i] <= 'Z')
				{
					arr[i] = input[i];
				}
				else
				{
					position = i;
					break;
				}
			}

			if (position > 0)
			{
				arr[position] = 'R';
			}

			int start = position + 1;
			for (int i = start; i < len; i++)
			{
				arr[i] = input[i];
			}

			return new string(arr, 0, len);
		}

		public string Array1(string input)
		{
			int len = input.Length;
			int position = -1;
			char[] arr = new char[len];
			for (int i = 0; i < len; i++)
			{
				if (input[i] >= 'A' && input[i] <= 'Z')
				{
					arr[i] = input[i];
				}
				else
				{
					position = i;
					break;
				}
			}

			if (position > 0)
			{
				arr[position] = 'R';
			}

			int start = position + 1;
			for (int i = start; i < len; i++)
			{
				arr[i] = input[i];
			}

			return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(arr));
		}


		public string Create(string input)
		{
			int len = input.Length;
			return string.Create(len, input, (target, src) =>
			{
				int position = -1;
				for (int i = 0; i < len; i++)
				{
					if (src[i] >= 'A' && src[i] <= 'Z')
					{
						target[i] = src[i];
					}
					else
					{
						position = i;
						break;
					}
				}

				if (position > 0)
				{
					target[position] = 'R';
				}

				int start = position + 1;
				for (int i = start; i < len; i++)
				{
					target[i] = src[i];
				}
			});
		}

		/// <summary>
		/// 减少使用外部变量
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
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

		/// <summary>
		/// 使用静态匿名函数
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string StaticCreate(string input)
		{
			int len = input.Length;
			//在委托前static关键字修饰,主要避免创建多余的委托对象
			//在C# 9加入,静态匿名函数(lambda和匿名函数)
			return string.Create(len, input, static (target, src) =>
			{
				int position = -1;
				int strLen = target.Length;
				for (int i = 0; i < strLen; i++)
				{
					if (src[i] >= 'A' && src[i] <= 'Z')
					{
						target[i] = src[i];
					}
					else
					{
						position = i;
						break;
					}
				}

				if (position > 0)
				{
					target[position] = 'R';
				}

				int start = position + 1;
				for (int i = start; i < strLen; i++)
				{
					target[i] = src[i];
				}
			});
		}

		public void CreateAction(Span<char> target, string src)
		{
			int position = -1;
			for (int i = 0; i < src.Length; i++)
			{
				if (src[i] >= 'A' && src[i] <= 'Z')
				{
					target[i] = src[i];
				}
				else
				{
					position = i;
					break;
				}
			}

			if (position > 0)
			{
				target[position] = 'R';
			}

			int start = position + 1;
			for (int i = start; i < src.Length; i++)
			{
				target[i] = src[i];
			}
		}

		//给Create所需的委托加缓存,而不是每次都创建新的委托实例
		public SpanAction<char, string> spanAction => CreateAction;
		public string CreateCache(string input)
		{
			int len = input.Length;

			return string.Create(len, input, spanAction);
		}

		////查看反编译源码
		//public string LocalCreate(string input)
		//{
		//    int num2 = input.get_Length();
		//    string str = input;
		//    SpanAction<char, string> u003cu003e9_120 = StringCreateTest.u003cu003ec.u003cu003e9__12_0;
		//    if (u003cu003e9_120 == null)
		//    {
		//        u003cu003e9_120 = new SpanAction<char, string>(StringCreateTest.u003cu003ec.u003cu003e9, (Span<char> target, string src) =>
		//        {
		//            int num = -1;
		//            int length = target.get_Length();
		//            int num1 = 0;
		//            while (num1 < length)
		//            {
		//                if (src[num1] < 'A' || src[num1] > 'Z')
		//                {
		//                    num = num1;
		//                    break;
		//                }
		//                else
		//                {
		//                    *((target.get_Item(num1))) = src[num1];
		//                    num1++;
		//                }
		//            }
		//            if (num > 0)
		//            {
		//                *((target.get_Item(num))) = 'R';
		//            }
		//            for (int i = num + 1; i < length; i++)
		//            {
		//                *((target.get_Item(i))) = src[i];
		//            }
		//        });
		//        StringCreateTest.u003cu003ec.u003cu003e9__12_0 = u003cu003e9_120;
		//    }
		//    return string.Create<string>(num2, str, u003cu003e9_120);
		//}
	}
}
