using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace CSharpBenchmarks.LinqTest
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class CountTest
    {
        private static List<int> Numbers = new(1000);

        static CountTest()
        {
            Random random = new Random(10);
            for (int i = 0; i < 1000; i++)
            {
                Numbers.Add(random.Next(int.MinValue, int.MaxValue));
            }
        }

        [Benchmark]
        public int LinqCount()
        {
            return Numbers.Count(num => num > int.MaxValue / 2);
        }

        [Benchmark]
        public int LinqWhereCount()
        {
            return Numbers.Where(num => num > int.MaxValue / 2).Count();
        }

        [Benchmark]
        public int ForeachCount()
        {
            int count = 0;
            int val = int.MaxValue / 2;  //计算放到循环外
            foreach (var num in Numbers)
            {
                if (num > val)
                {
                    count += 1;
                }
            }
            return count;
        }
    }
}
