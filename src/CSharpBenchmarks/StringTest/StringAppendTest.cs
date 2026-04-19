using System;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.StringTest;

[MemoryDiagnoser]
[DisassemblyDiagnoser(printSource: true)]
public class StringAppendTest
{
	[Params(128, 1024)]
	public int Count { get; set; }

	[Params("Test", "TestAsync")]
	public string Names { get; set; }

	[Benchmark(Baseline = true)]
	public void Kingdee()
	{
		int count = 0;
		for (int i = 0; i < Count; i++)
		{
			var s = ROT13Encode(Names);
			count += s.Length;
		}
	}

	[Benchmark]
	public void New()
	{
		int count = 0;
		for (int i = 0; i < Count; i++)
		{
			var s = ROT13Encode1(Names);
			count += s.Length;
		}
	}

	private static string ROT13Encode(string inputText)
	{
		string text = "";
		for (int i = 0; i < inputText.Length; i++)
		{
			int num = Convert.ToChar(inputText.Substring(i, 1));
			if (num >= 97 && num <= 109)
			{
				num += 13;
			}
			else if (num >= 110 && num <= 122)
			{
				num -= 13;
			}
			else if (num >= 65 && num <= 77)
			{
				num += 13;
			}
			else if (num >= 78 && num <= 90)
			{
				num -= 13;
			}
			text += (char)num;
		}
		return text;
	}

	private static string ROT13Encode1(string inputText)
	{
		StringBuilder builder = new(inputText.Length);
		for (int i = 0; i < inputText.Length; i++)
		{
			int num = inputText[i];
			if (num >= 97 && num <= 109)
			{
				num += 13;
			}
			else if (num >= 110 && num <= 122)
			{
				num -= 13;
			}
			else if (num >= 65 && num <= 77)
			{
				num += 13;
			}
			else if (num >= 78 && num <= 90)
			{
				num -= 13;
			}
			builder.Append((char)num);
		}
		return builder.ToString();
	}
}
