namespace CSharpBenchmarks.StringTest
{
	//[MemoryDiagnoser]
	//[DisassemblyDiagnoser(printSource: true)]
	//public class StringTrimTest
	//{
	//	[Params(4096)]
	//	public int Count { get; set; }

	//	[Params("Test", " TestAsync ")]
	//	public string Names { get; set; }

	//	[Benchmark(Baseline = true)]
	//	public void Trim()
	//	{
	//		var times = 0;
	//		for (int i = 0; i < Count; i++)
	//		{
	//			var str = Names.Substring(2).Trim();
	//			times += str.Length;
	//		}
	//	}

	//	[Benchmark]
	//	public void SpanTrim()
	//	{
	//		for (int i = 0; i < Count; i++)
	//		{

	//		}
	//	}
	//}

	//public static class StringUtils
	//{
	//	public static string SpanTrim(this ReadOnlySpan<char> span)
	//	{
	//		if (span.Length == 0)
	//		{
	//			return string.Empty;
	//		}
	//		int end = span.Length - 1;
	//		int start = 0;

	//		for (start = 0; start < end; start++)
	//		{
	//			if (!char.IsWhiteSpace(span[start]))
	//			{
	//				break;
	//			}
	//		}

	//		for (end = span.Length - 1; end >= start; end--)
	//		{
	//			if (!char.IsWhiteSpace(span[end]))
	//			{
	//				break;
	//			}
	//		}

	//		int len = end - start + 1;

	//	}
	//}
}

