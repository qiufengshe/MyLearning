using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBenchmarks.LinqTest
{
    public class ForTest
    {

        public static Person[] arr;

        static ForTest()
        {
            arr = Enumerable.Range(0, 10).Select(x => new Person("h" + x)).ToArray();
        }

        public void Test1(Person[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                Console.WriteLine(arr[i]);
            }
        }

        public void Test2(Person[] arr)
        {
            ref var item = ref MemoryMarshal.GetArrayDataReference(arr);
            for (int i = 0; i < arr.Length; i++)
            {
                var person = Unsafe.Add(ref item, i);
                person.GetName();
            }

            //CollectionsMarshal.AsSpan();  //将list转为span
        }


        public void Test3(Person[] arr)
        {
            ref var start = ref MemoryMarshal.GetArrayDataReference(arr);
            ref var end = ref Unsafe.Add(ref start, arr.Length);
            while (Unsafe.IsAddressLessThan(ref start, ref end))
            {
                start.GetName();
                start = ref Unsafe.Add(ref start, 1);
            }
        }
    }


    public class Person
    {
        public Person(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string GetName()
        {
            return Name;
        }
    }
}
