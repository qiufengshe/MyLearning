using System;
using System.Threading;

namespace CharpLearning.Syntax
{
    public class TimeProviderTest
    {
        public void Test()
        {
            //TimeProvider.System.CreateTimer() //创建一个定时器
            var timeProvider = TimeProvider.System;
            var start = timeProvider.GetTimestamp();

            Thread.Sleep(3000);

            var timeSpan = timeProvider.GetElapsedTime(start);
            Console.WriteLine($"{timeSpan.Milliseconds}毫秒数");

        }
    }
}
