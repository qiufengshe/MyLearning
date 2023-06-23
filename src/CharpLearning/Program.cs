using CharpLearning.Syntax;

namespace CharpLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //Console.WriteLine("test");
            var api = new GCAddApi();
            api.ArrayTest();
            api.AllocateArrayTest();
            api.AllocateUninitializedArrayTest();
        }
    }
}
