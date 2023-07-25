using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace net6perf.LinqTest
{
	[DisassemblyDiagnoser(printSource: true, maxDepth: 2)]
	[MemoryDiagnoser]
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	public class TakTest
	{
		public int[] bytes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 42, 48, 98, 10 };

		[Params(1024, 2048)]
		public int Times { get; set; }

		[Benchmark]
		public int Take()
		{
			int sum = 0;
			for (int i = 0; i < Times; i++)
			{
				var array = bytes.Take(5).ToArray();
				sum += array.Length;
			}

			return sum;
		}


		[Benchmark]
		public int AsSpan()
		{
			int sum = 0;
			for (int i = 0; i < Times; i++)
			{
				var array = bytes.AsSpan().Slice(0, 5);
				sum += array.Length;
			}

			return sum;
		}
	}
}

////Take源码
//public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
//{
//    if (source == null)
//    {
//        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
//    }

//    return count <= 0 ?
//        Empty<TSource>() :
//        TakeIterator<TSource>(source, count);
//}

//private static IEnumerable<TSource> TakeIterator<TSource>(IEnumerable<TSource> source, int count)
//{
//    Debug.Assert(count > 0);

//    foreach (TSource element in source)
//    {
//        yield return element;  //yield简化迭代器,具体可以使用ILSpy工具查看生成IL代码
//        if (--count == 0) break;
//    }
//}

////IL部分代码 MovNext
//.method private final hidebysig newslot virtual
//            instance bool MoveNext() cil managed
//{
//            .override method instance bool [System.Runtime]
//    System.Collections.IEnumerator::MoveNext()
//    // Method begins at RVA 0x27c8
//    // Header size: 12
//    // Code size: 168 (0xa8)
//    .maxstack 3
//    .locals init (
//        [0] int32,
//        [1] bool,
//        [2] int32
//    )

//    IL_0000: ldarg.0
//    IL_0001: ldfld int32 CharpLearning.Program / '<Test>d__1'::'<>1__state'
//    IL_0006: stloc.0
//    IL_0007: ldloc.0
//    IL_0008: brfalse.s IL_0012

//    IL_000a: br.s IL_000c

//    IL_000c: ldloc.0
//    IL_000d: ldc.i4.1
//    IL_000e: beq.s IL_0014

//    IL_0010: br.s IL_0016

//    IL_0012: br.s IL_0018

//    IL_0014: br.s IL_005f

//    IL_0016: ldc.i4.0
//    IL_0017: ret

//    IL_0018: ldarg.0
//    IL_0019: ldc.i4.m1
//    IL_001a: stfld int32 CharpLearning.Program / '<Test>d__1'::'<>1__state'
//    IL_001f: nop
//    IL_0020: nop
//    IL_0021: ldarg.0
//    IL_0022: ldarg.0
//    IL_0023: ldfld int32[] CharpLearning.Program / '<Test>d__1'::source
//    IL_0028: stfld int32[] CharpLearning.Program / '<Test>d__1'::'<>s__1'
//    IL_002d: ldarg.0
//    IL_002e: ldc.i4.0
//    IL_002f: stfld int32 CharpLearning.Program / '<Test>d__1'::'<>s__2'
//    IL_0034: br.s IL_008f

//    IL_0036: ldarg.0
//    IL_0037: ldarg.0
//    IL_0038: ldfld int32[] CharpLearning.Program / '<Test>d__1'::'<>s__1'
//    IL_003d: ldarg.0
//    IL_003e: ldfld int32 CharpLearning.Program / '<Test>d__1'::'<>s__2'
//    IL_0043: ldelem.i4
//    IL_0044: stfld int32 CharpLearning.Program / '<Test>d__1'::'<element>5__3'
//    IL_0049: nop
//    IL_004a: ldarg.0
//    IL_004b: ldarg.0
//    IL_004c: ldfld int32 CharpLearning.Program / '<Test>d__1'::'<element>5__3'
//    IL_0051: stfld int32 CharpLearning.Program / '<Test>d__1'::'<>2__current'
//    IL_0056: ldarg.0
//    IL_0057: ldc.i4.1
//    IL_0058: stfld int32 CharpLearning.Program / '<Test>d__1'::'<>1__state'
//    IL_005d: ldc.i4.1
//    IL_005e: ret

//    IL_005f: ldarg.0
//    IL_0060: ldc.i4.m1
//    IL_0061: stfld int32 CharpLearning.Program / '<Test>d__1'::'<>1__state'
//    IL_0066: ldarg.0
//    IL_0067: ldfld int32 CharpLearning.Program / '<Test>d__1'::count
//    IL_006c: ldc.i4.1
//    IL_006d: sub
//    IL_006e: stloc.2
//    IL_006f: ldarg.0
//    IL_0070: ldloc.2
//    IL_0071: stfld int32 CharpLearning.Program / '<Test>d__1'::count
//    IL_0076: ldloc.2
//    IL_0077: ldc.i4.0
//    IL_0078: ceq
//    IL_007a: stloc.1
//    IL_007b: ldloc.1
//    IL_007c: brfalse.s IL_0080

//    IL_007e: br.s IL_009f

//    IL_0080: nop
//    IL_0081: ldarg.0
//    IL_0082: ldarg.0
//    IL_0083: ldfld int32 CharpLearning.Program / '<Test>d__1'::'<>s__2'
//    IL_0088: ldc.i4.1
//    IL_0089: add
//    IL_008a: stfld int32 CharpLearning.Program / '<Test>d__1'::'<>s__2'

//    IL_008f: ldarg.0
//    IL_0090: ldfld int32 CharpLearning.Program / '<Test>d__1'::'<>s__2'
//    IL_0095: ldarg.0
//    IL_0096: ldfld int32[] CharpLearning.Program / '<Test>d__1'::'<>s__1'
//    IL_009b: ldlen
//    IL_009c: conv.i4
//    IL_009d: blt.s IL_0036

//    IL_009f: ldarg.0
//    IL_00a0: ldnull
//    IL_00a1: stfld int32[] CharpLearning.Program / '<Test>d__1'::'<>s__1'
//    IL_00a6: ldc.i4.0
//    IL_00a7: ret
//}


////性能测试
//| Method |        Job |  Runtime | Toolchain | Times |         Mean |       Error |       StdDev | Ratio | RatioSD | Code Size |   Gen 0 | Allocated | Alloc Ratio |
//|------- |----------- |--------- |---------- |------ |-------------:|------------:|-------------:|------:|--------:|----------:|--------:|----------:|------------:|
//|   Take | Job-ZJKRIV | .NET 7.0 |    net7.0 |  1024 |  66,691.3 ns |   247.98 ns |    207.08 ns |  0.74 |    0.02 |     623 B | 31.2500 |   98304 B |        1.00 |
//|   Take | Job-GWPOIS | .NET 6.0 |    net6.0 |  1024 |  72,213.3 ns |   244.29 ns |    216.55 ns |  0.80 |    0.02 |     333 B | 31.2500 |   98304 B |        1.00 |
//|   Take | Job-LEKBGG | .NET 5.0 |    net5.0 |  1024 |  90,891.2 ns | 1,756.83 ns |  2,462.83 ns |  1.00 |    0.00 |     500 B | 31.2500 |   98304 B |        1.00 |
//|        |            |          |           |       |              |             |              |       |         |           |         |           |             |
//| AsSpan | Job-GWPOIS | .NET 6.0 |    net6.0 |  1024 |     847.7 ns |     1.76 ns |      1.37 ns |  1.00 |    0.00 |      68 B |       - |         - |          NA |
//| AsSpan | Job-LEKBGG | .NET 5.0 |    net5.0 |  1024 |     848.4 ns |     1.45 ns |      1.13 ns |  1.00 |    0.00 |      68 B |       - |         - |          NA |
//| AsSpan | Job-ZJKRIV | .NET 7.0 |    net7.0 |  1024 |   1,249.1 ns |     2.31 ns |      2.05 ns |  1.47 |    0.00 |      58 B |       - |         - |          NA |
//|        |            |          |           |       |              |             |              |       |         |           |         |           |             |
//|   Take | Job-ZJKRIV | .NET 7.0 |    net7.0 |  2048 | 137,061.1 ns |   826.11 ns |    732.33 ns |  0.75 |    0.00 |     623 B | 62.5000 |  196608 B |        1.00 |
//|   Take | Job-GWPOIS | .NET 6.0 |    net6.0 |  2048 | 168,411.8 ns | 7,551.80 ns | 20,925.99 ns |  0.92 |    0.21 |     333 B | 62.5000 |  196608 B |        1.00 |
//|   Take | Job-LEKBGG | .NET 5.0 |    net5.0 |  2048 | 182,651.8 ns |   477.35 ns |    423.16 ns |  1.00 |    0.00 |     500 B | 62.5000 |  196608 B |        1.00 |
//|        |            |          |           |       |              |             |              |       |         |           |         |           |             |
//| AsSpan | Job-LEKBGG | .NET 5.0 |    net5.0 |  2048 |   1,676.9 ns |     4.84 ns |      4.04 ns |  1.00 |    0.00 |      68 B |       - |         - |          NA |
//| AsSpan | Job-GWPOIS | .NET 6.0 |    net6.0 |  2048 |   1,678.5 ns |     8.91 ns |      8.34 ns |  1.00 |    0.01 |      68 B |       - |         - |          NA |
//| AsSpan | Job-ZJKRIV | .NET 7.0 |    net7.0 |  2048 |   2,493.8 ns |    11.53 ns |     10.78 ns |  1.49 |    0.01 |      58 B |       - |         - |          NA |


//; net6perf.LinqTest.TakTest.Take()
//; int sum = 0;
//; ^^^^^^^^^^^^
//; for (int i = 0; i < Times; i++)
//    ; ^^^^^^^^^
//    ; var array = bytes.Take(5).ToArray();
//; ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
//; sum += array.Length;
//; ^^^^^^^^^^^^^^^^^^^^
//; return sum;
//; ^^^^^^^^^^^push      r14
//             push      rdi
//       push      rsi
//       push      rbp
//       push      rbx
//       sub       rsp,20
//       mov       rsi, rcx
//       xor       edi, edi
//       xor       ebx, ebx
//       cmp       dword ptr [rsi+10],0
//       jle near ptr M00_L03
//M00_L00:
//       mov rbp,[rsi + 8]
//       test      rbp, rbp
//       je        near ptr M00_L04
//       mov       rdx, rbp
//       mov       rcx, offset MT_System.Linq.IPartition`1[[System.Int32, System.Private.CoreLib]]
//; 调用IsInstanceOfInterface方法
//       call qword ptr [System.Runtime.CompilerServices.CastHelpers.IsInstanceOfInterface(Void *, System.Object)]
//test rax, rax
//       jne       short M00_L01
//       mov       rcx, offset MT_System.Linq.Enumerable+ListPartition`1[[System.Int32, System.Private.CoreLib]]
//       call CORINFO_HELP_NEWSFAST
//       mov       r14, rax
//       call      CORINFO_HELP_GETCURRENTMANAGEDTHREADID
//       mov       [r14+8],eax
//       lea       rcx,[r14 + 18]
//       mov       rdx, rbp
//       call      CORINFO_HELP_ASSIGN_REF
//       xor       ecx, ecx
//       mov       [r14+14],ecx
//       mov       dword ptr [r14+20],4
//       jmp short M00_L02
//M00_L01:
//       mov rcx, rax
//       mov       r11,7FFE65150440
//       mov       edx,5
//       call      qword ptr [r11]
//mov r14, rax
//M00_L02:
//       mov rcx, r14
//       call      qword ptr [System.Linq.Enumerable.ToArray[[System.Int32, System.Private.CoreLib]](System.Collections.Generic.IEnumerable`1 < Int32 >)]
//       add edi,[rax + 8]
//       inc       ebx
//       cmp       ebx,[rsi + 10]
//       jl        near ptr M00_L00
//M00_L03:
//       mov eax, edi
//       add       rsp,20
//       pop       rbx
//       pop       rbp
//       pop       rsi
//       pop       rdi
//       pop       r14
//       ret
//M00_L04:
//       mov ecx,10
//       call      qword ptr [7FFE65535780]
//int 3
//; Total bytes of code 191


//; System.Runtime.CompilerServices.CastHelpers.IsInstanceOfInterface(Void *, System.Object)
//       test rdx, rdx
//       je        short M01_L03
//       mov       rax,[rdx]
//       movzx     r8d, word ptr [rax+0E]
//test r8, r8
//       je        short M01_L02
//       mov       r9,[rax + 38]
//       cmp       r8,4
//       jl        short M01_L01
//M01_L00:
//       cmp[r9],rcx
//       je        short M01_L03
//       cmp       [r9+8],rcx
//       je        short M01_L03
//       cmp       [r9+10],rcx
//       je        short M01_L03
//       cmp       [r9+18],rcx
//       je        short M01_L03
//       add       r9,20
//       add       r8,0FFFFFFFFFFFFFFFC
//       cmp       r8,4
//       jge       short M01_L00
//       test      r8, r8
//       je        short M01_L02
//M01_L01:
//       cmp[r9],rcx
//       je        short M01_L03
//       add       r9,8
//       dec       r8
//       test      r8, r8
//       jg        short M01_L01
//M01_L02:
//       test dword ptr [rax],406C0000
//       jne       short M01_L04
//       xor       edx, edx
//M01_L03:
//       mov rax, rdx
//       ret
//M01_L04:
//       jmp qword ptr [System.Runtime.CompilerServices.CastHelpers.IsInstance_Helper(Void *, System.Object)]
//; Total bytes of code 107