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
			"code": "BFL-0003-015",
			"name": "上光膜（冷敷） 268",
			"status": 0,
			"statusName": "正常",
			"specification": "268",
			"brand": "",
			"unit": "米",
			"materialTypeParentCode": "5",
			"materialTypeParentName": "辅料和配件",
			"materialTypeCode": "5-7",
			"materialTypeName": "预涂膜",
			"isReel": false,
			"minCount": 0,
			"maxCount": 0,
			"remark": "",
			"paperWeight": 0,
			"openness": null,
			"paperMo": 0,
			"paperLength": 0,
			"paperWidth": 0,
			"isEnableRatio": false,
			"length": 0,
			"width": 0,
			"height": 0,
			"ratio": 0,
			"isSquareRatio": "0",
			"subSequenceNumber": 0,
			"isTemplate": false,
			"kisCode": "040400000106",
			"kisName": "上光膜（冷敷） 268",
			"price": 0,
			"tonPrice": 0,
			"squarePrice": 0,
			"editor": null,
			"editTime": null,
			"stopName": null,
			"stopTime": null,
			"makerName": null,
			"makeTime": null,
			"paperSourceType": 0,
			"paperSourceTypeName": "无",
			"gensongSync": false,
			"paperType": 0,
			"paperTypeName": "暂无",
			"isCut": false,
			"isIgnoreBatch": true,
			"id": 1484
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