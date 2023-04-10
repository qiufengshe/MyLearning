using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace net7perf.Maths
{
    [JitStatsDiagnoser]
    public class JitInfoTest
    {
        [Benchmark]
        public void Sleep() => Thread.Sleep(10);
    }
}
