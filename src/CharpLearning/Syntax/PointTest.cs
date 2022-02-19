using System;

namespace CharpLearning.Syntax
{
    public struct PointTest
    {
        public int Id { get; set; }
    }

    public class FunctionPointer
    {
        public unsafe void Test()
        {
            Action<string> action1 = Console.WriteLine;
            action1("123");

            delegate*<string, void> action2 = &Console.WriteLine;
            action2("hello pointer");
        }
    }
}
