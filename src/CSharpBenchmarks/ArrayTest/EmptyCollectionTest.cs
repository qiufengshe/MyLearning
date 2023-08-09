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
		if (list.Any())   //.Net 7对Any并没有优化,这可能造成Any生成的汇编代码(328 byte)
		{
			sum += 1;
		}
	}
	return sum;
}
	}

	//源码
	/**
	    private static class EmptyArray<T>
	    {
	#pragma warning disable CA1825 // this is the implementation of Array.Empty<T>()
			internal static readonly T[] Value = new T[0];
	#pragma warning restore CA1825
		}
 
        public static T[] Empty<T>()
        {
            return EmptyArray<T>.Value;
        }

	   Enumerable.Empty源码
	   public static readonly IPartition<TElement> Instance = new EmptyPartition<TElement>();
	   public static IEnumerable<TResult> Empty<TResult>()
	   {
	   	  return EmptyPartition<TResult>.Instance;
	   }

	 */

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


; CSharpBenchmarks.ArrayTest.EmptyCollectionTest.ListEmpty2()
; 			int sum = 0;
; 			^^^^^^^^^^^^
; 			for (int i = 0; i < Count; i++)
; 			     ^^^^^^^^^
; 				var list = Enumerable.Empty<int>();
; 				^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 					sum += 1;
; 					^^^^^^^^^
; 			return sum;
; 			^^^^^^^^^^^
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,28
       mov       rsi,rcx
       xor       edi,edi
       xor       ebx,ebx
       cmp       dword ptr [rsi+8],0
       jle       short M00_L02
       mov       rcx,2A3B6806DF8
       mov       rbp,[rcx]
M00_L00:
       mov       rcx,rbp
       call      qword ptr [7FF831471318]; System.Linq.Enumerable.Any[[System.Int32, System.Private.CoreLib]](System.Collections.Generic.IEnumerable`1<Int32>);;调用Any(.Net 7没有优化)
       test      eax,eax
       je        short M00_L01
       inc       edi
M00_L01:
       inc       ebx
       cmp       ebx,[rsi+8]
       jl        short M00_L00
M00_L02:
       mov       eax,edi
       add       rsp,28
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
; Total bytes of code 67

; System.Linq.Enumerable.Any[[System.Int32, System.Private.CoreLib]](System.Collections.Generic.IEnumerable`1<Int32>)
       push      rbp
       push      rsi
       sub       rsp,38
       lea       rbp,[rsp+40]
       mov       [rbp-20],rsp
       mov       rsi,rcx
       test      rsi,rsi
       je        near ptr M01_L03
       mov       rdx,rsi
       mov       rcx,offset MT_System.Collections.Generic.ICollection`1[[System.Int32, System.Private.CoreLib]]
       call      qword ptr [7FF83103B810]; System.Runtime.CompilerServices.CastHelpers.IsInstanceOfInterface(Void*, System.Object)
       test      rax,rax
       je        short M01_L00
       mov       rcx,rax
       mov       r11,7FF830E904C0
       call      qword ptr [r11]
       test      eax,eax
       setne     al
       movzx     eax,al
       add       rsp,38
       pop       rsi
       pop       rbp
       ret
M01_L00:
       mov       rdx,rsi
       mov       rcx,offset MT_System.Linq.IIListProvider`1[[System.Int32, System.Private.CoreLib]]
       call      qword ptr [7FF83103B810]; System.Runtime.CompilerServices.CastHelpers.IsInstanceOfInterface(Void*, System.Object)
       test      rax,rax
       je        short M01_L01
       mov       rcx,rax
       mov       r11,7FF830E904B8
       mov       edx,1
       call      qword ptr [r11]
       test      eax,eax
       jl        short M01_L02
       test      eax,eax
       setne     al
       movzx     eax,al
       add       rsp,38
       pop       rsi
       pop       rbp
       ret
M01_L01:
       mov       rdx,rsi
       mov       rcx,offset MT_System.Collections.ICollection
       call      qword ptr [7FF83103B810]; System.Runtime.CompilerServices.CastHelpers.IsInstanceOfInterface(Void*, System.Object)
       test      rax,rax
       je        short M01_L02
       mov       rcx,rax
       mov       r11,7FF830E904B0
       call      qword ptr [r11]
       test      eax,eax
       setne     al
       movzx     eax,al
       add       rsp,38
       pop       rsi
       pop       rbp
       ret
M01_L02:
       mov       rcx,rsi
       mov       r11,7FF830E90498
       call      qword ptr [r11]
       mov       [rbp-10],rax
       mov       rcx,rax
       mov       r11,7FF830E904A0
       call      qword ptr [r11]
       mov       esi,eax
       mov       rcx,[rbp-10]
       mov       r11,7FF830E904A8
       call      qword ptr [r11]
       mov       eax,esi
       add       rsp,38
       pop       rsi
       pop       rbp
       ret
M01_L03:
       mov       ecx,10
       call      qword ptr [7FF83129B780]
       int       3
       push      rbp
       push      rsi
       sub       rsp,28
       mov       rbp,[rcx+20]
       mov       [rsp+20],rbp
       lea       rbp,[rbp+40]
       cmp       qword ptr [rbp-10],0
       je        short M01_L04
       mov       rcx,[rbp-10]
       mov       r11,7FF830E904A8
       call      qword ptr [r11]
M01_L04:
       nop
       add       rsp,28
       pop       rsi
       pop       rbp
       ret
; Total bytes of code 328

; CSharpBenchmarks.ArrayTest.EmptyCollectionTest.ListEmpty2()
; 			int sum = 0;
; 			^^^^^^^^^^^^
; 			for (int i = 0; i < Count; i++)
; 			     ^^^^^^^^^
; 				var list = Enumerable.Empty<int>();
; 				^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
; 					sum += 1;
; 					^^^^^^^^^
; 			return sum;
; 			^^^^^^^^^^^
       push      rdi
       push      rsi
       push      rbx
       sub       rsp,20
       mov       rsi,rcx
       xor       edi,edi
       xor       ebx,ebx
       cmp       dword ptr [rsi+8],0
       jle       short M00_L03
M00_L00:
       mov       rax,158D7801EB8
       mov       rcx,[rax]
       jmp       short M00_L04
M00_L01:
       test      eax,eax
       jne       short M00_L02
       call      qword ptr [7FF82E99D008]  ;;通过源码发现这里是个单例
       test      eax,eax
       je        short M00_L02
       inc       edi
M00_L02:
       inc       ebx
       cmp       ebx,[rsi+8]
       jl        short M00_L00
M00_L03:
       mov       eax,edi
       add       rsp,20
       pop       rbx
       pop       rsi
       pop       rdi
       ret
M00_L04:
       mov       eax,1
       jmp       short M00_L01
; Total bytes of code 75
	 * 
	 */
}
