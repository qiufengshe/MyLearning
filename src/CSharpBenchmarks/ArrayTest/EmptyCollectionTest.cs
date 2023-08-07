using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class EmptyCollectionTest
	{
		[Params(10000)]
		public int Count { get; set; }

		[Benchmark]
		public int ArrayEmpty1()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var arr = new int[0]; //这是重点
				if (arr.Length == 0)
				{
					sum += 1;
				}
			}
			return sum;
		}

		[Benchmark]
		public int ArrayEmpty2()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var arr = Array.Empty<int>(); //这是重点
				if (arr.Length == 0)
				{
					sum += 1;
				}
			}
			return sum;
		}

		[Benchmark]
		public int ListEmpty1()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var list = new List<int>(0);  //这是重点
				if (list.Count == 0)
				{
					sum += 1;
				}
			}
			return sum;
		}

		[Benchmark]
		public int ListEmpty2()
		{
			int sum = 0;
			for (int i = 0; i < Count; i++)
			{
				var list = Enumerable.Empty<int>();  //这是重点
				if (list.Any())
				{
					sum += 1;
				}
			}
			return sum;
		}
	}

	/*
	 * ;
	 * CSharpBenchmarks.ArrayTest.EmptyCollectionTest.ArrayEmpty1()
; 			int sum = 0;
; 			^^^^^^^^^^^^
; 			for (int i = 0; i < Count; i++)
; 			     ^^^^^^^^^
; 				var arr = new int[0];
; 				^^^^^^^^^^^^^^^^^^^^^
; 					sum += 1;
; 					^^^^^^^^^
; 			return sum;
; 			^^^^^^^^^^^
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,28
       xor       esi,esi
       xor       edi,edi
       mov       ebx,[rcx+8]
       test      ebx,ebx
       jle       short M00_L02
       mov       rbp,offset MT_System.Int32[]
M00_L00:
       mov       rcx,rbp
       xor       edx,edx
       call      CORINFO_HELP_NEWARR_1_VC   ;;进行内存分配(数组)
       cmp       dword ptr [rax+8],0
       jne       short M00_L01
       inc       esi
M00_L01:
       inc       edi
       cmp       edi,ebx
       jl        short M00_L00
M00_L02:
       mov       eax,esi
       add       rsp,28
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
; Total bytes of code 64
	 */


	/*
	 * 
; new int[0]在.Net 8.0和Array.Empty<int>(0)生成一样的汇编代码
; CSharpBenchmarks.ArrayTest.EmptyCollectionTest.ArrayEmpty2()
; 			int sum = 0;
; 			^^^^^^^^^^^^
; 			for (int i = 0; i < Count; i++)
; 			     ^^^^^^^^^
; 				var arr = Array.Empty<int>();
; 				^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 					sum += 1;
; 					^^^^^^^^^
; 			return sum;
; 			^^^^^^^^^^^
       xor       eax,eax
       xor       edx,edx
       mov       ecx,[rcx+8]
       test      ecx,ecx
       jle       short M00_L01
M00_L00:
       inc       eax
       inc       edx
       cmp       edx,ecx
       jl        short M00_L00
M00_L01:
       ret
; Total bytes of code 20
	 * 
	 */
}
