using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Bogus;
using CharpLearning.Syntax;

namespace CharpLearning
{
    class Program
    {
        public Lock @lock = new();
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            StringBuilder sb = new(16);
            sb.Append("helloworld");

            var p1 = new Faker<People>("zh_CN")
                .RuleFor(x => x.Id, x => x.IndexFaker += 1)
                .RuleFor(x => x.Name, y => y.Person.LastName + y.Person.FirstName)
                .UseSeed(1000)
                .Generate(1000)
                .ToArray();

            foreach (var p in p1)
            {
                Console.WriteLine($"{p.Id} == {p.Name}");
            }

            var api = new GCAddApi();
            api.ArrayTest();
            api.AllocateArrayTest();
            api.AllocateUninitializedArrayTest();


            //c# 12 new synatx
            // Create an array:
            int[] a = [1, 2, 3, 4, 5, 6, 7, 8];

            // Create a span
            Span<int> b = ['a', 'b', 'c', 'd', 'e', 'f', 'h', 'i'];

            // Create a 2 D array:
            int[][] twoD = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];

            // create a 2 D array from variables:
            int[] row0 = [1, 2, 3];
            int[] row1 = [4, 5, 6];
            int[] row2 = [7, 8, 9];
            int[][] twoDFromVariables = [row0, row1, row2];
        }

        static int AddNum(int a, int b)
        {
            return a + b;
        }

        static int GetCount()
        {
            IReadOnlyCollection<int> list = new int[] { 1, 2, 3 };
            return list.Count;
        }
    }
}
