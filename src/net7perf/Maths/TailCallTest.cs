﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace net7perf.Maths
{
    [TailCallDiagnoser]
    public class TailCallTest
    {
        [Benchmark]
        public long Calc()
        => FactorialWithoutTailing(7) - FactorialWithTailing(7);

        private static long FactorialWithoutTailing(int depth)
            => depth == 0 ? 1 : depth * FactorialWithoutTailing(depth - 1);

        private static long FactorialWithTailing(int pos, int depth)
            => pos == 0 ? depth : FactorialWithTailing(pos - 1, depth * pos);

        private static long FactorialWithTailing(int depth)
            => FactorialWithTailing(depth - 1, depth);
    }
}
