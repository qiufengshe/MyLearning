using BenchmarkDotNet.Running;

namespace CSharpBenchmarks
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			_ = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
		}
	}
}
