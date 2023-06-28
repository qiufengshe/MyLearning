using System;
using Bogus;
using CharpLearning.Syntax;

namespace CharpLearning
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

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
        }
    }
}
