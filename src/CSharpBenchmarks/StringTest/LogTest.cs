using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
namespace CSharpBenchmarks.StringTest
{
	[MemoryDiagnoser]
	public class LogTest
	{
		//[Benchmark]
		public void Log()
		{
			var v = Vector64.Create(1, 2, 3, 4);
			var v2 = Vector64.Create(5, 6, 7, 8);
			_ = Vector64.Add(v, v2);
		}
	}

	public static partial class LoggerMessageTest
	{
		// 源代码生成器 会生成一个静态委托,这样不会产生装箱和拆箱的开销,所以性能比较好
		// 委托中使用LoggerMessage.Define
		[LoggerMessage(LogLevel.Information, "Could not open socket to `{hostName}`")]
		public static partial void WriteLog(this ILogger logger, string hostName);

		//private static readonly global::System.Action<ILogger, string, global::System.Exception?> __WriteLogCallback =
		//   LoggerMessage.Define<string>(LogLevel.Information, new EventId(1302319554, nameof(WriteLog)), "Could not open socket to `{hostName}`", new LogDefineOptions() { SkipEnabledCheck = true });
		//public static partial void WriteLog(this global::Microsoft.Extensions.Logging.ILogger logger, global::System.String hostName)
		//{
		//	if (logger.IsEnabled(global::Microsoft.Extensions.Logging.LogLevel.Information))
		//	{
		//		__WriteLogCallback(logger, hostName, null);
		//	}
		//}
	}
}
