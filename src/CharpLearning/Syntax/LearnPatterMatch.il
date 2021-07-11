.class public auto ansi beforefieldinit CharpLearning.Syntax.LearnPatternMatch
    extends [System.Runtime]System.Object
{
    .method public hidebysig specialname rtspecialname instance void .ctor () cil managed 
    {
        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: nop
        IL_0007: ret
    }

    .method public hidebysig instance bool IsAlpha (
            char c
        ) cil managed 
    {
        .locals init (
            [0] bool V_0,
            [1] bool V_1
        )

        //判断>='a' a=97
        IL_0000: nop
        IL_0001: ldarg.1
        IL_0002: ldc.i4.s 97
        IL_0004: blt.s IL_000d

        //判断<='z' 122
        IL_0006: ldarg.1
        IL_0007: ldc.i4.s 122
        IL_0009: ble.s IL_0019

        IL_000b: br.s IL_001d

        //判断>='A' A=65
        IL_000d: ldarg.1
        IL_000e: ldc.i4.s 65
        IL_0010: blt.s IL_001d

        //判断<='Z' Z=90
        IL_0012: ldarg.1
        IL_0013: ldc.i4.s 90
        IL_0015: ble.s IL_0019

        IL_0017: br.s IL_001d

        IL_0019: ldc.i4.1
        IL_001a: stloc.0
        IL_001b: br.s IL_001f

        IL_001d: ldc.i4.0
        IL_001e: stloc.0

        IL_001f: ldloc.0
        IL_0020: stloc.1
        IL_0021: br.s IL_0023

        IL_0023: ldloc.1
        IL_0024: ret
    }
}