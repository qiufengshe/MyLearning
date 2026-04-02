using System;
using System.Text.Json;
using BenchmarkDotNet.Attributes;

[DisassemblyDiagnoser(printSource: true, maxDepth: 3)]
[MemoryDiagnoser]
public class JsonDocumentTest
{
	[Params(128, 1024)]
	public int Count { get; set; }

	private string json;

	[GlobalSetup]
	public void Setup()
	{
		json = """
		{
			"code": "Tst-0003-015",
			"name": "测试物料",
			"status": 0,
			"statusName": "正常",
			"specification": "268",
			"brand": "",
			"unit": "米",
			"isReel": false,
			"minCount": 0,
			"maxCount": 0,
			"remark": "",
			"paperWeight": 0,
			"openness": null,
			"paperMo": 0,
			"paperLength": 0,
			"paperWidth": 0,
			"length": 0,
			"width": 0,
			"height": 0,
			"ratio": 0,
			"price": 0,
			"tonPrice": 0,
			"squarePrice": 0,
			"id": 2000
		}
		""";
	}

	[Benchmark]
	public unsafe void Dispose()
	{
		int sum = 0;
		for (int i = 0; i < Count; i++)
		{
			using var jsonDocument = JsonDocument.Parse(json, new JsonDocumentOptions() { AllowTrailingCommas = true });
			if (jsonDocument is not null)
			{
				sum += 1;
			}
		}

	}

	[Benchmark(Baseline = true)]
	public unsafe void NoDispose()
	{
		int sum = 0;
		for (int i = 0; i < Count; i++)
		{
			var jsonDocument = JsonDocument.Parse(json, new JsonDocumentOptions() { AllowTrailingCommas = true }); 
			if (jsonDocument is not null)
			{
				sum += 1;
			}
		}
	}
}