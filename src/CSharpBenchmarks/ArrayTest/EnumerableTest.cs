using System.Linq;
using BenchmarkDotNet.Attributes;

namespace CSharpBenchmarks.ArrayTest
{
	[MemoryDiagnoser]
	[DisassemblyDiagnoser(printSource: true)]
	public class EnumerableTest
	{
		[Params(16, 128, 1024)]
		public int Count { get; set; }

		[Benchmark]
		public void RangeTest()
		{
			for (int i = 0; i < Count; i++)
			{
				var arr = Enumerable.Range(0, Count).ToArray(); //Enumerable.Range使用
				for (int j = 0; j < arr.Length; j++)
				{
					if (arr[j] != 0)
					{

					}
				}
			}
		}

		[Benchmark]
		public void ForTest()
		{
			for (int i = 0; i < Count; i++)
			{
				var arr = new int[Count];
				arr[0] = 0;
				arr[1] = 1;
				arr[2] = 2;
				arr[3] = 3;
				arr[4] = 4;
				arr[5] = 5;
				arr[6] = 6;
				arr[7] = 7;
				for (int j = 1; j < arr.Length / 8; j++)
				{
					arr[j] = j + 8;
					arr[j + 1] = j + 8 + 1;
					arr[j + 2] = j + 8 + 2;
					arr[j + 3] = j + 8 + 3;
					arr[j + 4] = j + 8 + 4;
					arr[j + 5] = j + 8 + 5;
					arr[j + 6] = j + 8 + 6;
					arr[j + 7] = j + 8 + 7;
				}
				for (int j = 0; j < arr.Length; j++)
				{
					if (arr[j] != 0)
					{

					}
				}
			}
		}
	}
	//public int[] ToArray()
	//{
	//	int start = _start;
	//	int[] array = new int[_end - start];
	//	Fill(array, start); //ToArray先分配一个数组后,使用Fill填充数据
	//	return array;
	//}

	//public List<int> ToList()
	//{
	//	(int start, int end) = (_start, _end);
	//	List<int> list = new List<int>(end - start);
	//	Fill(SetCountAndGetSpan(list, end - start), start); //ToList先创建一个List,使用Fill填充数据 
	//	return list;
	//}

	//private static void Fill(Span<int> destination, int value)
	//{
	//	ref int pos = ref MemoryMarshal.GetReference(destination);
	//	ref int end = ref Unsafe.Add(ref pos, destination.Length);

	//	if (Vector.IsHardwareAccelerated &&
	//		Vector<int>.Count <= 8 &&
	//		destination.Length >= Vector<int>.Count)
	//	{
	//		//初始化集合值
	//		Vector<int> init = new Vector<int>((ReadOnlySpan<int>)[0, 1, 2, 3, 4, 5, 6, 7]);
	//		// 先用初始化vector中的值,让后加上init的值,current中的为: 0, 1, 2, 3, 4, 5, 6, 7
	//		Vector<int> current = new Vector<int>(value) + init;
	//		//将vector中的值,用Vector<int>.Count当作初始值,这里8
	//		Vector<int> increment = new Vector<int>(Vector<int>.Count);

	//		//根据end,减去Vector<int>.Count,计算出循环结束的位置
	//		ref int oneVectorFromEnd = ref Unsafe.Subtract(ref end, Vector<int>.Count);
	//		do
	//		{
	//			//将current的值保存pos开始,到pos+Vector<int>.Count的
	//			current.StoreUnsafe(ref pos);
	//			//将current中的值 0, 1, 2, 3, 4, 5, 6, 7+都加上8
	//			current += increment;
	//			//移动pos位置,将pos+Vector<int>.Count
	//			pos = ref Unsafe.Add(ref pos, Vector<int>.Count);
	//		}
	//		while (!Unsafe.IsAddressGreaterThan(ref pos, ref oneVectorFromEnd));

	//		value = current[0];
	//	}

	//	while (Unsafe.IsAddressLessThan(ref pos, ref end))
	//	{
	//		pos = value++;
	//		pos = ref Unsafe.Add(ref pos, 1);
	//	}
	//}

	//生成的汇编代码
	//; CSharpBenchmarks.ArrayTest.EnumerableTest.RangeTest()
	//; 			for (int i = 0; i<Count; i++)
	//; 			     ^^^^^^^^^
	//; 				var arr = Enumerable.Range(0, Count).ToArray();
	//; 				^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
	//; 				for (int j = 0; j<arr.Length; j++)
	//; 				     ^^^^^^^^^
	//; 					if (arr[j] != 0)
	//; 					^^^^^^^^^^^^^^^^
	//       push rdi

	//	   push rsi

	//	   push rbp

	//	   push rbx

	//	   sub rsp,38
	//       xor eax, eax

	//	   mov[rsp + 28],rax
	//	   mov[rsp + 30], rax

	//	   mov rbx, rcx

	//	   xor esi, esi

	//	   cmp dword ptr[rbx + 8],0
	//       jle near ptr M00_L05
	//M00_L00:
	//       mov edi,[rbx + 8]

	//	   movsxd rcx, edi

	//	   dec rcx

	//	   test edi, edi

	//	   jl near ptr M00_L06

	//	   cmp rcx,7FFFFFFF
	//	   jg        near ptr M00_L06
	//	   test      edi,edi
	//	   je        near ptr M00_L08
	//	   mov       rcx,offset MT_System.Linq.Enumerable+RangeIterator
	//	   call      CORINFO_HELP_NEWSFAST
	//	   mov       rbp, rax
	//	   call      CORINFO_HELP_GETCURRENTMANAGEDTHREADID
	//	   mov       [rbp+8], eax
	//	   xor       edx, edx
	//	   mov       [rbp+14], edx
	//	   mov       [rbp+18], edi
	//M00_L01:

	//	   test rbp, rbp
	//	   je        near ptr M00_L07
	//	   mov       rdx, rbp
	//	   mov       rcx, offset MT_System.Linq.IIListProvider`1[[System.Int32, System.Private.CoreLib]]
	//       call qword ptr[7FFB30974348]; System.Runtime.CompilerServices.CastHelpers.IsInstanceOfInterface(Void*, System.Object)
	//	   test      rax,rax
	//	   je        near ptr M00_L09
	//	   mov       rdx,offset MT_System.Linq.Enumerable+RangeIterator
	//	   cmp       [rax], rdx
	//	   jne       near ptr M00_L10
	//	   mov       ebp, [rax+14]
	//	   mov       edx, [rax+18]
	//	sub       edx, ebp
	//	   movsxd    rdx, edx
	//	   mov       rcx, offset MT_System.Int32[]
	//	   call      CORINFO_HELP_NEWARR_1_VC
	//	   mov       rdi, rax
	//	   lea       rcx, [rdi+10]
	//	   mov       edx, [rdi+8]
	//	   mov       [rsp+28], rcx
	//	   mov       [rsp+30], edx
	//	   lea       rcx, [rsp+28]
	//	   mov       edx, ebp
	//;.Net 8中新增Fill填充数据,并且使用SIMD
	//	   call      qword ptr[7FFB310D7BE8]; System.Linq.Enumerable+RangeIterator.Fill(System.Span`1<Int32>, Int32)
	//M00_L02:
	//       xor ecx, ecx

	//	   mov edx,[rdi + 8]

	//	   test edx, edx

	//	   jle short M00_L04
	//M00_L03:
	//       inc ecx

	//	   cmp edx, ecx

	//	   jg short M00_L03
	//M00_L04:
	//       inc esi

	//	   cmp esi,[rbx + 8]

	//	   jl near ptr M00_L00
	//M00_L05:
	//       add rsp,38
	//       pop rbx

	//	   pop rbp

	//	   pop rsi

	//	   pop rdi

	//	   ret
	//M00_L06:
	//       mov ecx,1
	//       call qword ptr[7FFB30CBFC00]
	//	   int       3
	//M00_L07:
	//       mov ecx,10
	//       call qword ptr[7FFB30CBFBE8]
	//	   int       3
	//M00_L08:
	//       mov rcx,7FFB30AD0A60
	//	   mov       edx,4
	//       call CORINFO_HELP_CLASSINIT_SHARED_DYNAMICCLASS

	//	   mov rcx,25AB0C01EA0
	//	   mov       rbp,[rcx]
	//	jmp near ptr M00_L01
	//M00_L09:
	//       mov rcx, rbp

	//	   call qword ptr[7FFB310D7E28]
	//	   mov       rdi,rax
	//	   jmp       short M00_L02
	//M00_L10:
	//       mov rcx, rax

	//	   mov r11,7FFB30810730
	//	   call      qword ptr[r11]

	//	   mov rdi, rax

	//	   jmp near ptr M00_L02
	//; Total bytes of code 356

	//; System.Linq.Enumerable+RangeIterator.Fill(System.Span`1<Int32>, Int32)
	//       vzeroupper
	//	   mov       rax,[rcx]
	//	mov ecx,[rcx + 8]

	//	   mov r8d, ecx

	//	   lea r8,[rax + r8 * 4]

	//	   cmp ecx,8
	//       jl short M02_L02

	//	   vmovd xmm0, edx

	//	   vpbroadcastd ymm0, xmm0

	//	   vpaddd ymm0, ymm0,[7FFB30FC3AC0]

	//	   vmovups ymm1,[7FFB30FC3AE0]

	//	   lea rdx,[r8 - 20]

	//	   nop dword ptr[rax]
	//	   nop       dword ptr[rax]
	//M02_L00:
	//       vmovups[rax],ymm0
	//	   vpaddd    ymm0,ymm0,ymm1
	//	   add       rax,20
	//       cmp rax, rdx

	//	   jbe short M02_L00

	//	   vmovd edx, xmm0

	//	   cmp rax, r8

	//	   jb short M02_L03
	//M02_L01:
	//       vzeroupper
	//	   ret
	//M02_L02:
	//       cmp rax, r8

	//	   jae short M02_L01
	//M02_L03:
	//       lea ecx,[rdx + 1]

	//	   mov[rax],edx
	//	   add       rax,4
	//       mov edx, ecx

	//	   jmp short M02_L02
	//; Total bytes of code 112
}
