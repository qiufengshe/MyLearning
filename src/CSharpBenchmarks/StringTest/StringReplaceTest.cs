using System;

namespace CSharpBenchmarks.StringTest
{
	public class StringReplaceTest
	{


		public string Replace1(string text, int start, int num, char c = '*')
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
			int len = text.Length;
			var oldArray = text.ToCharArray();
			var newArray = new char[len];
			int i = 0, replaceLen = start + num;
			for (; i < start; i++)
			{
				newArray[i] = oldArray[i];
			}

			for (i = start; i < replaceLen; i++)
			{
				newArray[i] = c;
			}

			for (i = replaceLen; i < len; i++)
			{
				newArray[i] = oldArray[i];
			}
			return new string(newArray);
		}

		public unsafe string Replace2(string text, int start, int num, char c = '*')
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
			int len = text.Length;
			var oldArray = text.AsSpan();
			var newArray = new char[len];
			int i = 0, replaceLen = start + num;

			fixed (void* oldArrayPtr = oldArray)
			{
				newArray.AsSpan().Fill(c);
				fixed (void* newArrayPtr = newArray)
				{
					//Unsafe.CopyBlockUnaligned//(oldArrayPtr,newArrayPtr,start-1);
				}
			}

			for (i = start; i < replaceLen; i++)
			{
				newArray[i] = c;
			}

			for (i = replaceLen; i < len; i++)
			{
				newArray[i] = oldArray[i];
			}
			return new string(newArray);
		}
	}
}
