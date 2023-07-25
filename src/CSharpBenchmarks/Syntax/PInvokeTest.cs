using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace net6perf.pinvoke
{
	[DisassemblyDiagnoser(printSource: true, maxDepth: 3)]
	public class PInvokeTest
	{
		public IntPtr dllPtr;

		[DllImport("E:/project/csharp/Dotnet6App/net6perf/bin/Release/a.dll", EntryPoint = "add")]
		public static extern int Add(int a, int b);

		[GlobalSetup]
		public void Steup()
		{
			dllPtr = NativeLibrary.Load("E:/project/csharp/Dotnet6App/net6perf/bin/Release/b.dll");
		}

		[Benchmark]
		public void PInvokeTest1()
		{
			int sum = 0;
			for (int i = 0; i < 100000; i++)
			{
				sum += Add(1, 2);
			}
		}

		[Benchmark]
		public unsafe void PInvokeTest2()
		{
			delegate* unmanaged[Cdecl]<int, int, int> addFuncPointer = (delegate* unmanaged[Cdecl]<int, int, int>)NativeLibrary.GetExport(dllPtr, "add");
			int sum = 0;
			for (int i = 0; i < 100000; i++)
			{
				sum += addFuncPointer(1, 2);
			}
		}

		[GlobalCleanup]
		public void Clean()
		{
			if (dllPtr != IntPtr.Zero)
			{
				NativeLibrary.Free(dllPtr);
			}
		}
	}
}
