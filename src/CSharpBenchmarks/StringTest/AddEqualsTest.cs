using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.StringTest
{
    [MemoryDiagnoser]
    [DisassemblyDiagnoser(printSource: true)]
    public class AddEqualsTest
    {
        public static string[] arr1;

        public static string[] arr2;

        static AddEqualsTest()
        {
            arr1 = new string[10000];

            arr2 = new string[10000];

            for (int i = 0; i < 10000; i++)
            {
                arr1[i] = i.ToString();
                arr2[i] = i.ToString();
            }
        }

        [Benchmark]
        public void Slow() => Add1("Hello");


        [Benchmark]
        public void Fast() => Add2("Hello");

        public void Add1(string x)
        {
            var arr = arr1;
            for (int i = 0; i < 10000; i++)
            {
                arr[i] += x;
            }
        }

        public void Add2(string x)
        {
            var arr = arr2;
            for (int i = 0; i < 10000; i++)
            {
                arr[i] = arr[i] + x;
            }
        }
    }
}
