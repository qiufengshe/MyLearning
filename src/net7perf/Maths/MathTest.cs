using BenchmarkDotNet.Attributes;

namespace net7perf.Maths
{
    [DisassemblyDiagnoser]
    public class MathTest
    {

        //private int _a = 42, _b = 84;

        //[Benchmark]
        //public int MinTest() => Math.Min(_a, _b);

        [Benchmark]
        public void CalcTest()
        {
            for (int i = 0; i < 10; i++)
            {
                _ = Calc(1000000000);
            }
        }


        [Benchmark]
        public long Calc(long num)
        {
            long sum = 0;
            for (int i = 0; i < num; i++)
            {
                sum += i;
            }
            return sum;
        }
    }
}
