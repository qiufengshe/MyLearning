﻿﻿﻿﻿﻿﻿﻿## .Net 7性能改进-前﻿

#### 前言

这是一篇 [Performance Improvements in .NET 7](https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/) 的翻译,原文比较长,PDF文件有232页,所以进行了拆分,每天翻译一部分.下面便开始正文:

一年前,我(Stephen Toub)写了 [.Net 6性能改进](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-6) 这篇文章,类似的还有 [.NET Core 2.0](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-core) 和 [.NET Core 2.1](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-core-2-1) 及 [.NET Core 3.0](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-core-3-0) /[.NET 5](https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/) ,我喜欢写这些文章,喜欢阅读开发人员对它们的回应.特别是去年的一条评论引起了我的共鸣.该评论引用了《 Die Hard》中的一句话:"当亚历山大看到他的领土辽阔时,他为没有更多的世界需要征服而哭泣",并质疑.NET的性能改进是否类似.没有更多的“性能”世界需要征服吗?我有点头晕目眩地说,即使.NET6很快, .NET7明确地强调了我们还可以做得更多，并且已经做得更多。

与之前版本的.NET一样,性能是贯穿整个技术栈的一个关键焦点,性能是贯穿整个栈的关键焦点,无论是为性能而明确创建的特性,还是在设计和实现时仍然牢记性能的与性能无关的特性.现在,一个.NET7发行版的候选版本即将到来,这是讨论它的好时机.

在过去的一年中,每次我回顾可能对性能产生积极影响的提交,我都会将该链接复制到我为撰写本文而维护的期日记中.几周前,当我坐下来写这篇文章时,我面临着一份近1000个影响性能的PRs的列表(在发布的7000多个PRs中),我很高兴在这里与大家分享其中的近500个(.Net 6有400左右影响性能的提交).

.Net7很快,非常快.这个版本的运行时和核心库中有上千个影响性能的PRs，更不用说Asp.Net Core和WinForm和Entity Framework Core 性能改进.目前来说.Net 7是.Net中最快的.如果你的经理问你为什么你的项目应该升级到.Net 7，你可以说"除了发布版中的所有新功能，.Net 7的速度超级快"

我希望每一个有兴趣的人在离开这篇文章时都能对.NET是如何实现的,为什么做出了各种决策,评估了哪些权衡,采用了哪些技术,考虑了哪些算法,以及使用了哪些有价值的工具和方法,使.NET比以前更快.我希望开发人员从我们自己的学习中学习,并找到将这些新发现的知识应用于他们自己的代码库的方法,从而进一步提高生态系统中代码的整体性能.我希望开发人员多做一点,考虑下一次在处理棘手问题时使用分析器,考虑查看他们正在使用的组件的源代码,以便更好地理解如何使用它,并考虑重新审视以前的假设和决策,以确定它们是否仍然准确和适当.我希望开发人员对提交PRs以改进.NET的前景感到兴奋,不仅是为了他们自己,也是为了全球使用.NET的每个开发人员.如果其中任何一个听起来很有趣,那么我鼓励你选择最后一次冒险: 准备一瓶你最喜欢的热饮,舒适地享受,并请享受.

#### .Net 7有哪些性能改进

![.Net 7性能改进,有JIT和NativeAOT/GC/反射等](https://www.qiufengblog.com/uploadFiles/202209052119087384.png)

#### 准备工作

创建一个性能测试的项目:

```bash
#创建项目
dotnet new console -o net7perf

#进入项目
cd .\net7perf\

#使用BenchmarkDotNet进行性能测试
dotnet add package benchmarkdotnet
```

修改项目工程文件:

```xml
<Project Sdk="Microsoft.NET.Sdk">

 <PropertyGroup>
  <OutputType>Exe</OutputType>
  <!--测试.Net6和.Net 7-->
  <TargetFrameworks>net6.0;net7.0</TargetFrameworks>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
  <!--使用CSharp语言的预览版本-->
  <LangVersion>Preview</LangVersion>
  <!--允许使用不安全操作,如指针-->
  <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  <!--指定GC的模式为服务器模式-->
  <ServerGarbageCollection>true</ServerGarbageCollection>
 </PropertyGroup>

 <ItemGroup>
  <PackageReference Include="benchmarkdotnet" Version="0.13.2.1879" />
 </ItemGroup>

</Project>
```

调整Program.cs代码:

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Win32;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

[MemoryDiagnoser(displayGenColumns: false)]
[DisassemblyDiagnoser]
[HideColumns("Error", "StdDev", "Median", "RatioSD")]
public partial class Program
{
    static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

    //这里将测试代码拆分具体文件中
}
```

```bash
#运行测试
dotnet run -c Release -f net6.0 --filter '*方法名*' --runtimes net6.0 net7.0
```

后面着重翻译JIT,因为JIT是直接影响程序的性能.

#### JIT部分

我想首先讨论一下即时编译器(JIT)中的性能改进,讨论的内容本身并不是性能改进.在低层、性能敏感的代码时,能够准确理解JIT生成的汇编代码是至关重要的.有多种方法可以获取该汇编代码.在线工具sharplab.io对此非常有用(感谢@ashmind提供此工具);然而,它目前只针对单个版本，因此在编写本文时，我只能看到.NET6的输出,这使得很难将其用于A/B多版本测试.godbolt.org在这方面也很有价值,在@hez2010在compiler-explorer/compiler-explorer#3168中添加了C#支持,具有类似单个版本的限制.最灵活的解决方案涉及在本地获取该程序集代码,因为它可以将您所需的任何版本或本地构建与所需的配置和开关集进行比较.

一种常见的方法是在benchmarkdotnet中使用[DisassemblyDiagnoser]特性。只需将[DisassemblyDiagnosis]特性添加到测试类中,benchmarkdotnet将查找为测试生成的汇编代码以及它们调用的函数的深度,并以人类可读的形式转储找到的汇编代码.例如,如果我运行此测试:

```csharp
using BenchmarkDotNet.Attributes;

namespace net7perf.Maths
{
    [DisassemblyDiagnoser]
    public class MathTest
    {

        private int _a = 42, _b = 84;

        [Benchmark]
        public int Min() => Math.Min(_a, _b);
    }
}
```

运行命令:

```bash
# --filter '*MathTest*' 指定运行MathTest下的Benchmark特性方法
dotnet run -c Release -f net7.0 --filter '*MathTest*'
```

除了正常的测试,BenchmarkDotNet是可以将汇编代码保存为单个文件MathTest.md.

```assembly
; net7perf.Maths.MathTest.Min()
       mov       eax,[rcx+8]
       mov       edx,[rcx+0C]
       cmp       eax,edx
       jg        short M00_L01
       mov       edx,eax
M00_L00:
       mov       eax,edx
       ret
M00_L01:
       jmp       short M00_L00
; Total bytes of code 17
```

生成的汇编代码格式相当整洁.这种支持最近在dotnet/benchmarkdotnet#2072中得到了进一步改进,它允许在命令行上向benchmarkdotnet传递一个过滤器列表,以准确地告诉它应该转储哪些方法的汇编代码.

如果您可以获得.NET运行时的“调试”或“检查”版本("检查"是启用了优化但仍包含断言的版本),尤其是clrjit,另一种有价值的方法是设置一个环境变量,该环境变量使JIT本身对其发出的所有汇编代码进行可读的描述.这可以用于任何类型的应用程序,因为它是JIT本身的一部分,而不是任何特定工具或其他环境的一部分.它支持显示JIT每次生成代码时生成的代码(例如,如果它首先编译一个没有优化的方法,然后用优化重新编译),总的来说,这是汇编代码最准确的图片.当然,最大的缺点是它需要非发布版本的运行时,这通常意味着您需要从dotnet/runtime repo中的源代码自己构建。

直到.NET 7,也就是说,从dotnet/runtime#73365开始,这个程序集转储支持现在也可以在发布版本中使用,这意味着它只是.NET7的一部分,您不需要任何特殊的东西来使用它.要了解这一点,请尝试创建一个简单的“hello world”应用程序,如:

```csharp
using System;

class Program
{
    public static void Main() => Console.WriteLine("Hello, world!");
}
```

编译程序(如dotnet build-c Release).然后,将DOTNET_JitDisasm 环境变量设置为我们需要关心的方法名称,在本例中为“Main”(允许的确切语法更为宽松,允许使用通配符、可选名称空间和类名等).当我使用PowerShell添加环境变量:

```bash
# 1. 编译程序
dotnet build -c Release

# 2.Main为需要生成汇编代码的方法名称,添加环境变量
$env:DOTNET_JitDisasm="Main"

# 3.运行程序
dotnet run
```

运行程序后,会在控制台中打印出指定方法的汇编代码:

```assembly
; Assembly listing for method Program:Main(ref)
; Emitting BLENDED_CODE for X64 CPU with AVX - Windows
; debuggable code
; rbp based frame
; fully interruptible
; No PGO data

G_M000_IG01:                ;; offset=0000H
       55                   push     rbp
       57                   push     rdi
       4883EC28             sub      rsp, 40
       488D6C2430           lea      rbp, [rsp+30H]
       48894D10             mov      gword ptr [rbp+10H], rcx

G_M000_IG02:                ;; offset=000FH
       833DD2C80B0000       cmp      dword ptr [(reloc 0x7ffcccf3cf98)], 0
       7405                 je       SHORT G_M000_IG04

G_M000_IG03:                ;; offset=0018H
       E82366C25F           call     CORINFO_HELP_DBG_IS_JUST_MY_CODE

G_M000_IG04:                ;; offset=001DH
       90                   nop
       48B96820009DD5020000 mov      rcx, 0x2D59D002068
       488B09               mov      rcx, gword ptr [rcx]
       FF15FF101000         call     [Console:WriteLine(String)]
       90                   nop
       90                   nop

G_M000_IG05:                ;; offset=0033H
       4883C428             add      rsp, 40
       5F                   pop      rdi
       5D                   pop      rbp
       C3                   ret

; Total bytes of code 58

Hello, world!
```

这对于性能分析和调优非常有帮助,甚至对于像“我的函数是否内联”或“我期望优化的代码是否真的被优化掉了”这样简单的问题也是如此.在这篇文章的其余部分，我将包括由这两种机制之一生成的汇编代码片段,以帮助举例说明概念.

请注意,有时在确定指定什么名称作为DOTNET_JitDisasm的值时可能会有点混乱，特别是当您所关心的方法是C#编译器名称或名称混乱的方法时(因为JIT只看到IL和元数据，而不是原始的C#代码)例如,具有顶级语句的程序的入口点方法的名称、本地函数的名称等.为了帮助实现这一点，并提供JIT正在进行的工作的真正有价值的顶层视图，.NET 7还支持新的DOTNET_JitDisasmSummary 环境变量(在DOTNET/runtime#74090中引入).将其设置为“1”,它将导致JIT在每次编译方法时都发出一行,包括该方法的名称,该名称可使用DOTNET_JitDisasm复制/粘贴。但是，这个特性本身很有用，因为它可以快速地为您突出显示正在编译什么、何时编译以及使用什么设置。例如，如果我设置环境变量，然后运行“hello，world”控制台应用程序，会得到以下输出(7.0.100-rc.2.22457.11之前的版本,开启DOTNET_JitDisasmSummary=是不会出现下面输出的,7.0.100-rc.2.22457.11输出的信息更丰富)：

```csharp
   1: JIT compiled CastHelpers:StelemRef(Array,long,Object) [Tier1, IL size=88, code size=93]
   2: JIT compiled CastHelpers:LdelemaRef(Array,long,long):byref [Tier1, IL size=44, code size=44]
   3: JIT compiled SpanHelpers:IndexOfNullCharacter(byref):int [Tier1, IL size=792, code size=388]
   4: JIT compiled Program:Main() [Tier0, IL size=11, code size=36]
   5: JIT compiled ASCIIUtility:NarrowUtf16ToAscii(long,long,long):long [Tier0, IL size=490, code size=1187]
Hello, world!
```

我们可以看到,对于“hello world”,实际上只有5个方法可以进行JIT编译.当然,作为简单的“hello world”的一部分执行的方法还有很多,但几乎所有方法都有预编译的本机代码,作为核心库的“准备运行”(R2R)映像的一部分.上面列表中的前三个(StelemRef、LdelemaRef和IndexOfNullCharacter)没有,因为它们通过使用[MethodImpl(MethodImplOptions.AggressiveOptimization)]属性显式选择退出R2R(尽管名称不同,但该属性几乎不应使用,并且仅在核心库中的几个非常特定的位置使用).然后是我们的Main方法.最后是NarrowUtf16ToAscii方法,由于使用了可变宽度Vector<T>,因此它也没有R2R代码(稍后将详细介绍).运行的所有其他方法都不需要JIT.如果我们首先将DOTNET_ReadyToRun环境变量设置为0,则列表会更长,并让您非常清楚地了解JIT在启动时需要做什么(以及为什么R2R等技术对启动时间很重要).注意在输出“hello world”之前编译了多少方法:

```csharp
 1: JIT compiled CastHelpers:StelemRef(Array,long,Object) [Tier1, IL size=88, code size=93]
   2: JIT compiled CastHelpers:LdelemaRef(Array,long,long):byref [Tier1, IL size=44, code size=44]
   3: JIT compiled AppContext:Setup(long,long,int) [Tier0, IL size=68, code size=275]
   4: JIT compiled Dictionary`2:.ctor(int):this [Tier0, IL size=9, code size=40]
   5: JIT compiled Dictionary`2:.ctor(int,IEqualityComparer`1):this [Tier0, IL size=102, code size=444]
   6: JIT compiled Object:.ctor():this [Tier0, IL size=1, code size=10]
   7: JIT compiled Dictionary`2:Initialize(int):int:this [Tier0, IL size=56, code size=231]
   8: JIT compiled HashHelpers:GetPrime(int):int [Tier0, IL size=83, code size=379]
   9: JIT compiled HashHelpers:.cctor() [Tier0, IL size=24, code size=102]
  10: JIT compiled HashHelpers:GetFastModMultiplier(int):long [Tier0, IL size=9, code size=37]
  11: JIT compiled Type:GetTypeFromHandle(RuntimeTypeHandle):Type [Tier0, IL size=8, code size=14]
  12: JIT compiled Type:op_Equality(Type,Type):bool [Tier0, IL size=38, code size=143]
  13: JIT compiled NonRandomizedStringEqualityComparer:GetStringComparer(Object):IEqualityComparer`1 [Tier0, IL size=39, code size=170]
  14: JIT compiled NonRandomizedStringEqualityComparer:.cctor() [Tier0, IL size=46, code size=232]
  15: JIT compiled EqualityComparer`1:get_Default():EqualityComparer`1 [Tier0, IL size=6, code size=36]
  16: JIT compiled EqualityComparer`1:.cctor() [Tier0, IL size=26, code size=125]
  17: JIT compiled ComparerHelpers:CreateDefaultEqualityComparer(Type):Object [Tier0, IL size=235, code size=949]
  18: JIT compiled CastHelpers:ChkCastClass(long,Object):Object [Tier0, IL size=22, code size=72]
  19: JIT compiled RuntimeHelpers:GetMethodTable(Object):long [Tier0, IL size=11, code size=33]
  20: JIT compiled CastHelpers:IsInstanceOfClass(long,Object):Object [Tier0, IL size=97, code size=257]
  21: JIT compiled GenericEqualityComparer`1:.ctor():this [Tier0, IL size=7, code size=31]
  22: JIT compiled EqualityComparer`1:.ctor():this [Tier0, IL size=7, code size=31]
  23: JIT compiled CastHelpers:ChkCastClassSpecial(long,Object):Object [Tier0, IL size=87, code size=246]
  24: JIT compiled OrdinalComparer:.ctor(IEqualityComparer`1):this [Tier0, IL size=8, code size=39]
  25: JIT compiled NonRandomizedStringEqualityComparer:.ctor(IEqualityComparer`1):this [Tier0, IL size=14, code size=52]
  26: JIT compiled StringComparer:get_Ordinal():StringComparer [Tier0, IL size=6, code size=49]
  27: JIT compiled OrdinalCaseSensitiveComparer:.cctor() [Tier0, IL size=11, code size=71]
  28: JIT compiled OrdinalCaseSensitiveComparer:.ctor():this [Tier0, IL size=8, code size=33]
  29: JIT compiled OrdinalComparer:.ctor(bool):this [Tier0, IL size=14, code size=43]
  30: JIT compiled StringComparer:.ctor():this [Tier0, IL size=7, code size=31]
  31: JIT compiled StringComparer:get_OrdinalIgnoreCase():StringComparer [Tier0, IL size=6, code size=49]
  32: JIT compiled OrdinalIgnoreCaseComparer:.cctor() [Tier0, IL size=11, code size=71]
  33: JIT compiled OrdinalIgnoreCaseComparer:.ctor():this [Tier0, IL size=8, code size=36]
  34: JIT compiled OrdinalIgnoreCaseComparer:.ctor(IEqualityComparer`1):this [Tier0, IL size=8, code size=39]
  35: JIT compiled CastHelpers:ChkCastAny(long,Object):Object [Tier0, IL size=38, code size=115]
  36: JIT compiled CastHelpers:TryGet(long,long):int [Tier0, IL size=129, code size=308]
  37: JIT compiled CastHelpers:TableData(ref):byref [Tier0, IL size=7, code size=31]
  38: JIT compiled MemoryMarshal:GetArrayDataReference(ref):byref [Tier0, IL size=7, code size=24]
  39: JIT compiled CastHelpers:KeyToBucket(byref,long,long):int [Tier0, IL size=38, code size=87]
  40: JIT compiled CastHelpers:HashShift(byref):int [Tier0, IL size=3, code size=16]
  41: JIT compiled BitOperations:RotateLeft(long,int):long [Tier0, IL size=17, code size=23]
  42: JIT compiled CastHelpers:Element(byref,int):byref [Tier0, IL size=15, code size=33]
  43: JIT compiled Volatile:Read(byref):int [Tier0, IL size=6, code size=16]
  44: JIT compiled String:Ctor(long):String [Tier0, IL size=57, code size=155]
  45: JIT compiled String:wcslen(long):int [Tier0, IL size=7, code size=31]
  46: JIT compiled SpanHelpers:IndexOfNullCharacter(byref):int [Tier1, IL size=792, code size=388]
  47: JIT compiled String:get_Length():int:this [Tier0, IL size=7, code size=17]
  48: JIT compiled Buffer:Memmove(byref,byref,long) [Tier0, IL size=59, code size=102]
  49: JIT compiled RuntimeHelpers:IsReferenceOrContainsReferences():bool [Tier0, IL size=2, code size=8]
  50: JIT compiled Buffer:Memmove(byref,byref,long) [Tier0, IL size=480, code size=678]
  51: JIT compiled Dictionary`2:Add(__Canon,__Canon):this [Tier0, IL size=11, code size=55]
  52: JIT compiled Dictionary`2:TryInsert(__Canon,__Canon,ubyte):bool:this [Tier0, IL size=675, code size=2467]
  53: JIT compiled OrdinalComparer:GetHashCode(String):int:this [Tier0, IL size=7, code size=37]
  54: JIT compiled String:GetNonRandomizedHashCode():int:this [Tier0, IL size=110, code size=290]
  55: JIT compiled BitOperations:RotateLeft(int,int):int [Tier0, IL size=17, code size=20]
  56: JIT compiled Dictionary`2:GetBucket(int):byref:this [Tier0, IL size=29, code size=90]
  57: JIT compiled HashHelpers:FastMod(int,int,long):int [Tier0, IL size=20, code size=70]
  58: JIT compiled Type:get_IsValueType():bool:this [Tier0, IL size=7, code size=39]
  59: JIT compiled RuntimeType:IsValueTypeImpl():bool:this [Tier0, IL size=54, code size=158]
  60: JIT compiled RuntimeType:GetNativeTypeHandle():TypeHandle:this [Tier0, IL size=12, code size=48]
  61: JIT compiled TypeHandle:.ctor(long):this [Tier0, IL size=8, code size=25]
  62: JIT compiled TypeHandle:get_IsTypeDesc():bool:this [Tier0, IL size=14, code size=38]
  63: JIT compiled TypeHandle:AsMethodTable():long:this [Tier0, IL size=7, code size=17]
  64: JIT compiled MethodTable:get_IsValueType():bool:this [Tier0, IL size=20, code size=32]
  65: JIT compiled GC:KeepAlive(Object) [Tier0, IL size=1, code size=10]
  66: JIT compiled Buffer:_Memmove(byref,byref,long) [Tier0, IL size=25, code size=279]
  67: JIT compiled Environment:InitializeCommandLineArgs(long,int,long):ref [Tier0, IL size=75, code size=332]
  68: JIT compiled Environment:.cctor() [Tier0, IL size=11, code size=163]
  69: JIT compiled StartupHookProvider:ProcessStartupHooks() [Tier-0 switched to FullOpts, IL size=365, code size=1053]
  70: JIT compiled StartupHookProvider:get_IsSupported():bool [Tier0, IL size=18, code size=60]
  71: JIT compiled AppContext:TryGetSwitch(String,byref):bool [Tier0, IL size=97, code size=322]
  72: JIT compiled ArgumentException:ThrowIfNullOrEmpty(String,String) [Tier0, IL size=16, code size=53]
  73: JIT compiled String:IsNullOrEmpty(String):bool [Tier0, IL size=15, code size=58]
  74: JIT compiled AppContext:GetData(String):Object [Tier0, IL size=64, code size=205]
  75: JIT compiled ArgumentNullException:ThrowIfNull(Object,String) [Tier0, IL size=10, code size=42]
  76: JIT compiled Monitor:Enter(Object,byref) [Tier0, IL size=17, code size=55]
  77: JIT compiled Dictionary`2:TryGetValue(__Canon,byref):bool:this [Tier0, IL size=39, code size=97]
  78: JIT compiled Dictionary`2:FindValue(__Canon):byref:this [Tier0, IL size=391, code size=1466]
  79: JIT compiled EventSource:.cctor() [Tier0, IL size=34, code size=80]
  80: JIT compiled EventSource:InitializeIsSupported():bool [Tier0, IL size=18, code size=60]
  81: JIT compiled RuntimeEventSource:.ctor():this [Tier0, IL size=55, code size=184]
  82: JIT compiled Guid:.ctor(int,short,short,ubyte,ubyte,ubyte,ubyte,ubyte,ubyte,ubyte,ubyte):this [Tier0, IL size=86, code size=132]
  83: JIT compiled EventSource:.ctor(Guid,String):this [Tier0, IL size=11, code size=90]
  84: JIT compiled EventSource:.ctor(Guid,String,int,ref):this [Tier0, IL size=58, code size=187]
  85: JIT compiled EventSource:get_IsSupported():bool [Tier0, IL size=6, code size=11]
  86: JIT compiled TraceLoggingEventHandleTable:.ctor():this [Tier0, IL size=20, code size=67]
  87: JIT compiled EventSource:ValidateSettings(int):int [Tier0, IL size=37, code size=147]
  88: JIT compiled EventSource:Initialize(Guid,String,ref):this [Tier0, IL size=418, code size=1584]
  89: JIT compiled Guid:op_Equality(Guid,Guid):bool [Tier0, IL size=10, code size=39]
  90: JIT compiled Guid:EqualsCore(byref,byref):bool [Tier0, IL size=132, code size=171]
  91: JIT compiled ActivityTracker:get_Instance():ActivityTracker [Tier0, IL size=6, code size=49]
  92: JIT compiled ActivityTracker:.cctor() [Tier0, IL size=11, code size=71]
  93: JIT compiled ActivityTracker:.ctor():this [Tier0, IL size=7, code size=31]
  94: JIT compiled RuntimeEventSource:get_ProviderMetadata():ReadOnlySpan`1:this [Tier0, IL size=13, code size=91]
  95: JIT compiled ReadOnlySpan`1:.ctor(long,int):this [Tier0, IL size=51, code size=115]
  96: JIT compiled RuntimeHelpers:IsReferenceOrContainsReferences():bool [Tier0, IL size=2, code size=8]
  97: JIT compiled ReadOnlySpan`1:get_Length():int:this [Tier0, IL size=7, code size=17]
  98: JIT compiled OverrideEventProvider:.ctor(EventSource,int):this [Tier0, IL size=22, code size=68]
  99: JIT compiled EventProvider:.ctor(int):this [Tier0, IL size=46, code size=194]
 100: JIT compiled EtwEventProvider:.ctor():this [Tier0, IL size=7, code size=31]
 101: JIT compiled EventProvider:Register(EventSource):this [Tier0, IL size=48, code size=186]
 102: JIT compiled MulticastDelegate:CtorClosed(Object,long):this [Tier0, IL size=23, code size=70]
 103: JIT compiled EventProvider:EventRegister(EventSource,EtwEnableCallback):int:this [Tier0, IL size=53, code size=154]
 104: JIT compiled EventSource:get_Name():String:this [Tier0, IL size=7, code size=18]
 105: JIT compiled EventSource:get_Guid():Guid:this [Tier0, IL size=7, code size=41]
 106: JIT compiled EtwEventProvider:System.Diagnostics.Tracing.IEventProvider.EventRegister(EventSource,EtwEnableCallback,long,byref):int:this [Tier0, IL size=19, code size=71]
 107: JIT compiled Advapi32:EventRegister(byref,EtwEnableCallback,long,byref):int [Tier0, IL size=53, code size=374]
 108: JIT compiled Marshal:GetFunctionPointerForDelegate(__Canon):long [Tier0, IL size=17, code size=54]
 109: JIT compiled Marshal:GetFunctionPointerForDelegate(Delegate):long [Tier0, IL size=18, code size=53]
 110: JIT compiled EventPipeEventProvider:.ctor():this [Tier0, IL size=18, code size=41]
 111: JIT compiled EventListener:get_EventListenersLock():Object [Tier0, IL size=41, code size=157]
 112: JIT compiled List`1:.ctor(int):this [Tier0, IL size=47, code size=275]
 113: JIT compiled Interlocked:CompareExchange(byref,__Canon,__Canon):__Canon [Tier0, IL size=9, code size=50]
 114: JIT compiled NativeRuntimeEventSource:.cctor() [Tier0, IL size=11, code size=71]
 115: JIT compiled NativeRuntimeEventSource:.ctor():this [Tier0, IL size=63, code size=184]
 116: JIT compiled Guid:.ctor(int,ushort,ushort,ubyte,ubyte,ubyte,ubyte,ubyte,ubyte,ubyte,ubyte):this [Tier0, IL size=88, code size=132]
 117: JIT compiled NativeRuntimeEventSource:get_ProviderMetadata():ReadOnlySpan`1:this [Tier0, IL size=13, code size=91]
 118: JIT compiled EventPipeEventProvider:System.Diagnostics.Tracing.IEventProvider.EventRegister(EventSource,EtwEnableCallback,long,byref):int:this [Tier0, IL size=44, code size=118]
 119: JIT compiled EventPipeInternal:CreateProvider(String,EtwEnableCallback):long [Tier0, IL size=43, code size=320]
 120: JIT compiled Utf16StringMarshaller:GetPinnableReference(String):byref [Tier0, IL size=13, code size=50]
 121: JIT compiled String:GetPinnableReference():byref:this [Tier0, IL size=7, code size=24]
 122: JIT compiled EventListener:AddEventSource(EventSource) [Tier0, IL size=175, code size=560]
 123: JIT compiled List`1:get_Count():int:this [Tier0, IL size=7, code size=17]
 124: JIT compiled WeakReference`1:.ctor(__Canon):this [Tier0, IL size=9, code size=42]
 125: JIT compiled WeakReference`1:.ctor(__Canon,bool):this [Tier0, IL size=15, code size=60]
 126: JIT compiled List`1:Add(__Canon):this [Tier0, IL size=60, code size=124]
 127: JIT compiled String:op_Inequality(String,String):bool [Tier0, IL size=11, code size=46]
 128: JIT compiled String:Equals(String,String):bool [Tier0, IL size=36, code size=114]
 129: JIT compiled ReadOnlySpan`1:GetPinnableReference():byref:this [Tier0, IL size=23, code size=57]
 130: JIT compiled EventProvider:SetInformation(int,long,int):int:this [Tier0, IL size=38, code size=131]
 131: JIT compiled ILStubClass:IL_STUB_PInvoke(long,int,long,int):int [FullOpts, IL size=62, code size=170]
 132: JIT compiled Program:Main() [Tier0, IL size=11, code size=36]
 133: JIT compiled Console:WriteLine(String) [Tier0, IL size=12, code size=59]
 134: JIT compiled Console:get_Out():TextWriter [Tier0, IL size=20, code size=113]
 135: JIT compiled Console:.cctor() [Tier0, IL size=11, code size=71]
 136: JIT compiled Volatile:Read(byref):__Canon [Tier0, IL size=6, code size=21]
 137: JIT compiled Console:<get_Out>g__EnsureInitialized|26_0():TextWriter [Tier0, IL size=63, code size=209]
 138: JIT compiled ConsolePal:OpenStandardOutput():Stream [Tier0, IL size=34, code size=130]
 139: JIT compiled Console:get_OutputEncoding():Encoding [Tier0, IL size=72, code size=237]
 140: JIT compiled ConsolePal:get_OutputEncoding():Encoding [Tier0, IL size=11, code size=200]
 141: JIT compiled NativeLibrary:LoadLibraryCallbackStub(String,Assembly,bool,int):long [Tier0, IL size=63, code size=280]
 142: JIT compiled EncodingHelper:GetSupportedConsoleEncoding(int):Encoding [Tier0, IL size=53, code size=186]
 143: JIT compiled Encoding:GetEncoding(int):Encoding [Tier0, IL size=340, code size=1025]
 144: JIT compiled EncodingProvider:GetEncodingFromProvider(int):Encoding [Tier0, IL size=51, code size=232]
 145: JIT compiled Encoding:FilterDisallowedEncodings(Encoding):Encoding [Tier0, IL size=29, code size=84]
 146: JIT compiled LocalAppContextSwitches:get_EnableUnsafeUTF7Encoding():bool [Tier0, IL size=16, code size=46]
 147: JIT compiled LocalAppContextSwitches:GetCachedSwitchValue(String,byref):bool [Tier0, IL size=22, code size=76]
 148: JIT compiled LocalAppContextSwitches:GetCachedSwitchValueInternal(String,byref):bool [Tier0, IL size=46, code size=168]
 149: JIT compiled LocalAppContextSwitches:GetSwitchDefaultValue(String):bool [Tier0, IL size=32, code size=98]
 150: JIT compiled String:op_Equality(String,String):bool [Tier0, IL size=8, code size=39]
 151: JIT compiled Encoding:get_Default():Encoding [Tier0, IL size=6, code size=49]
 152: JIT compiled Encoding:.cctor() [Tier0, IL size=12, code size=73]
 153: JIT compiled UTF8EncodingSealed:.ctor(bool):this [Tier0, IL size=8, code size=40]
 154: JIT compiled UTF8Encoding:.ctor(bool):this [Tier0, IL size=14, code size=43]
 155: JIT compiled UTF8Encoding:.ctor():this [Tier0, IL size=12, code size=36]
 156: JIT compiled Encoding:.ctor(int):this [Tier0, IL size=42, code size=152]
 157: JIT compiled UTF8Encoding:SetDefaultFallbacks():this [Tier0, IL size=64, code size=212]
 158: JIT compiled EncoderReplacementFallback:.ctor(String):this [Tier0, IL size=110, code size=360]
 159: JIT compiled EncoderFallback:.ctor():this [Tier0, IL size=7, code size=31]
 160: JIT compiled String:get_Chars(int):ushort:this [Tier0, IL size=29, code size=61]
 161: JIT compiled Char:IsSurrogate(ushort):bool [Tier0, IL size=17, code size=43]
 162: JIT compiled Char:IsBetween(ushort,ushort,ushort):bool [Tier0, IL size=12, code size=52]
 163: JIT compiled DecoderReplacementFallback:.ctor(String):this [Tier0, IL size=110, code size=360]
 164: JIT compiled DecoderFallback:.ctor():this [Tier0, IL size=7, code size=31]
 165: JIT compiled Encoding:get_CodePage():int:this [Tier0, IL size=7, code size=17]
 166: JIT compiled Encoding:get_UTF8():Encoding [Tier0, IL size=6, code size=49]
 167: JIT compiled UTF8Encoding:.cctor() [Tier0, IL size=12, code size=76]
 168: JIT compiled Volatile:Write(byref,__Canon) [Tier0, IL size=6, code size=32]
 169: JIT compiled ConsolePal:GetStandardFile(int,int,bool):Stream [Tier0, IL size=50, code size=183]
 170: JIT compiled ConsolePal:get_InvalidHandleValue():long [Tier0, IL size=7, code size=41]
 171: JIT compiled IntPtr:.ctor(int):this [Tier0, IL size=9, code size=25]
 172: JIT compiled ConsolePal:ConsoleHandleIsWritable(long):bool [Tier0, IL size=26, code size=68]
 173: JIT compiled Kernel32:WriteFile(long,long,int,byref,long):int [Tier0, IL size=46, code size=294]
 174: JIT compiled Marshal:SetLastSystemError(int) [Tier0, IL size=7, code size=40]
 175: JIT compiled Marshal:GetLastSystemError():int [Tier0, IL size=6, code size=34]
 176: JIT compiled WindowsConsoleStream:.ctor(long,int,bool):this [Tier0, IL size=37, code size=90]
 177: JIT compiled ConsoleStream:.ctor(int):this [Tier0, IL size=31, code size=71]
 178: JIT compiled Stream:.ctor():this [Tier0, IL size=7, code size=31]
 179: JIT compiled MarshalByRefObject:.ctor():this [Tier0, IL size=7, code size=31]
 180: JIT compiled Kernel32:GetFileType(long):int [Tier0, IL size=27, code size=217]
 181: JIT compiled Console:CreateOutputWriter(Stream):TextWriter [Tier0, IL size=50, code size=230]
 182: JIT compiled Stream:.cctor() [Tier0, IL size=11, code size=71]
 183: JIT compiled NullStream:.ctor():this [Tier0, IL size=7, code size=31]
 184: JIT compiled EncodingExtensions:RemovePreamble(Encoding):Encoding [Tier0, IL size=25, code size=118]
 185: JIT compiled UTF8EncodingSealed:get_Preamble():ReadOnlySpan`1:this [Tier0, IL size=24, code size=99]
 186: JIT compiled UTF8Encoding:get_PreambleSpan():ReadOnlySpan`1 [Tier0, IL size=12, code size=87]
 187: JIT compiled ConsoleEncoding:.ctor(Encoding):this [Tier0, IL size=14, code size=52]
 188: JIT compiled Encoding:.ctor():this [Tier0, IL size=8, code size=33]
 189: JIT compiled Encoding:SetDefaultFallbacks():this [Tier0, IL size=23, code size=65]
 190: JIT compiled EncoderFallback:get_ReplacementFallback():EncoderFallback [Tier0, IL size=6, code size=49]
 191: JIT compiled EncoderReplacementFallback:.cctor() [Tier0, IL size=11, code size=71]
 192: JIT compiled EncoderReplacementFallback:.ctor():this [Tier0, IL size=12, code size=44]
 193: JIT compiled DecoderFallback:get_ReplacementFallback():DecoderFallback [Tier0, IL size=6, code size=49]
 194: JIT compiled DecoderReplacementFallback:.cctor() [Tier0, IL size=11, code size=71]
 195: JIT compiled DecoderReplacementFallback:.ctor():this [Tier0, IL size=12, code size=44]
 196: JIT compiled StreamWriter:.ctor(Stream,Encoding,int,bool):this [Tier0, IL size=201, code size=564]
 197: JIT compiled Task:get_CompletedTask():Task [Tier0, IL size=6, code size=49]
 198: JIT compiled Task:.cctor() [Tier0, IL size=76, code size=316]
 199: JIT compiled TaskFactory:.ctor():this [Tier0, IL size=7, code size=31]
 200: JIT compiled Task`1:.ctor(bool,VoidTaskResult,int,CancellationToken):this [Tier0, IL size=21, code size=75]
 201: JIT compiled Task:.ctor(bool,int,CancellationToken):this [Tier0, IL size=70, code size=181]
 202: JIT compiled <>c:.cctor() [Tier0, IL size=11, code size=71]
 203: JIT compiled <>c:.ctor():this [Tier0, IL size=7, code size=31]
 204: JIT compiled TextWriter:.ctor(IFormatProvider):this [Tier0, IL size=36, code size=124]
 205: JIT compiled TextWriter:.cctor() [Tier0, IL size=26, code size=108]
 206: JIT compiled NullTextWriter:.ctor():this [Tier0, IL size=7, code size=31]
 207: JIT compiled TextWriter:.ctor():this [Tier0, IL size=29, code size=103]
 208: JIT compiled String:ToCharArray():ref:this [Tier0, IL size=52, code size=173]
 209: JIT compiled MemoryMarshal:GetArrayDataReference(ref):byref [Tier0, IL size=7, code size=24]
 210: JIT compiled ConsoleStream:get_CanWrite():bool:this [Tier0, IL size=7, code size=18]
 211: JIT compiled ConsoleEncoding:GetEncoder():Encoder:this [Tier0, IL size=12, code size=57]
 212: JIT compiled UTF8Encoding:GetEncoder():Encoder:this [Tier0, IL size=7, code size=63]
 213: JIT compiled EncoderNLS:.ctor(Encoding):this [Tier0, IL size=37, code size=102]
 214: JIT compiled Encoder:.ctor():this [Tier0, IL size=7, code size=31]
 215: JIT compiled Encoding:get_EncoderFallback():EncoderFallback:this [Tier0, IL size=7, code size=18]
 216: JIT compiled EncoderNLS:Reset():this [Tier0, IL size=24, code size=92]
 217: JIT compiled ConsoleStream:get_CanSeek():bool:this [Tier0, IL size=2, code size=12]
 218: JIT compiled StreamWriter:set_AutoFlush(bool):this [Tier0, IL size=25, code size=72]
 219: JIT compiled StreamWriter:CheckAsyncTaskInProgress():this [Tier0, IL size=19, code size=47]
 220: JIT compiled Task:get_IsCompleted():bool:this [Tier0, IL size=16, code size=40]
 221: JIT compiled Task:IsCompletedMethod(int):bool [Tier0, IL size=11, code size=25]
 222: JIT compiled StreamWriter:Flush(bool,bool):this [Tier0, IL size=272, code size=1127]
 223: JIT compiled StreamWriter:ThrowIfDisposed():this [Tier0, IL size=15, code size=43]
 224: JIT compiled Encoding:get_Preamble():ReadOnlySpan`1:this [Tier0, IL size=12, code size=70]
 225: JIT compiled ConsoleEncoding:GetPreamble():ref:this [Tier0, IL size=6, code size=27]
 226: JIT compiled Array:Empty():ref [Tier0, IL size=6, code size=49]
 227: JIT compiled EmptyArray`1:.cctor() [Tier0, IL size=12, code size=52]
 228: JIT compiled ReadOnlySpan`1:op_Implicit(ref):ReadOnlySpan`1 [Tier0, IL size=7, code size=79]
 229: JIT compiled ReadOnlySpan`1:.ctor(ref):this [Tier0, IL size=33, code size=81]
 230: JIT compiled MemoryMarshal:GetArrayDataReference(ref):byref [Tier0, IL size=7, code size=24]
 231: JIT compiled ConsoleEncoding:GetMaxByteCount(int):int:this [Tier0, IL size=13, code size=63]
 232: JIT compiled UTF8EncodingSealed:GetMaxByteCount(int):int:this [Tier0, IL size=20, code size=50]
 233: JIT compiled Span`1:.ctor(long,int):this [Tier0, IL size=51, code size=115]
 234: JIT compiled ReadOnlySpan`1:.ctor(ref,int,int):this [Tier0, IL size=65, code size=147]
 235: JIT compiled Encoder:GetBytes(ReadOnlySpan`1,Span`1,bool):int:this [Tier0, IL size=44, code size=234]
 236: JIT compiled MemoryMarshal:GetNonNullPinnableReference(ReadOnlySpan`1):byref [Tier0, IL size=30, code size=54]
 237: JIT compiled ReadOnlySpan`1:get_Length():int:this [Tier0, IL size=7, code size=17]
 238: JIT compiled MemoryMarshal:GetNonNullPinnableReference(Span`1):byref [Tier0, IL size=30, code size=54]
 239: JIT compiled Span`1:get_Length():int:this [Tier0, IL size=7, code size=17]
 240: JIT compiled EncoderNLS:GetBytes(long,int,long,int,bool):int:this [Tier0, IL size=92, code size=279]
 241: JIT compiled ArgumentNullException:ThrowIfNull(long,String) [Tier0, IL size=12, code size=45]
 242: JIT compiled Encoding:GetBytes(long,int,long,int,EncoderNLS):int:this [Tier0, IL size=57, code size=187]
 243: JIT compiled EncoderNLS:get_HasLeftoverData():bool:this [Tier0, IL size=35, code size=105]
 244: JIT compiled UTF8Encoding:GetBytesFast(long,int,long,int,byref):int:this [Tier0, IL size=33, code size=119]
 245: JIT compiled Utf8Utility:TranscodeToUtf8(long,int,long,int,byref,byref):int [Tier0, IL size=1446, code size=3208]
 246: JIT compiled Math:Min(int,int):int [Tier0, IL size=8, code size=28]
 247: JIT compiled ASCIIUtility:NarrowUtf16ToAscii(long,long,long):long [Tier0, IL size=490, code size=1187]
 248: JIT compiled WindowsConsoleStream:Flush():this [Tier0, IL size=26, code size=56]
 249: JIT compiled ConsoleStream:Flush():this [Tier0, IL size=1, code size=10]
 250: JIT compiled TextWriter:Synchronized(TextWriter):TextWriter [Tier0, IL size=28, code size=121]
 251: JIT compiled SyncTextWriter:.ctor(TextWriter):this [Tier0, IL size=14, code size=52]
 252: JIT compiled SyncTextWriter:WriteLine(String):this [Tier0, IL size=13, code size=140]
 253: JIT compiled StreamWriter:WriteLine(String):this [Tier0, IL size=20, code size=110]
 254: JIT compiled String:op_Implicit(String):ReadOnlySpan`1 [Tier0, IL size=31, code size=171]
 255: JIT compiled String:GetRawStringData():byref:this [Tier0, IL size=7, code size=24]
 256: JIT compiled ReadOnlySpan`1:.ctor(byref,int):this [Tier0, IL size=15, code size=39]
 257: JIT compiled StreamWriter:WriteSpan(ReadOnlySpan`1,bool):this [Tier0, IL size=368, code size=1036]
 258: JIT compiled MemoryMarshal:GetReference(ReadOnlySpan`1):byref [Tier0, IL size=8, code size=17]
 259: JIT compiled Buffer:MemoryCopy(long,long,long,long) [Tier0, IL size=21, code size=83]
 260: JIT compiled Unsafe:ReadUnaligned(long):long [Tier0, IL size=10, code size=17]
 261: JIT compiled ASCIIUtility:AllCharsInUInt64AreAscii(long):bool [Tier0, IL size=16, code size=38]
 262: JIT compiled ASCIIUtility:NarrowFourUtf16CharsToAsciiAndWriteToBuffer(byref,long) [Tier0, IL size=107, code size=171]
 263: JIT compiled Unsafe:WriteUnaligned(byref,int) [Tier0, IL size=11, code size=22]
 264: JIT compiled Unsafe:ReadUnaligned(long):int [Tier0, IL size=10, code size=16]
 265: JIT compiled ASCIIUtility:AllCharsInUInt32AreAscii(int):bool [Tier0, IL size=11, code size=25]
 266: JIT compiled ASCIIUtility:NarrowTwoUtf16CharsToAsciiAndWriteToBuffer(byref,int) [Tier0, IL size=24, code size=35]
 267: JIT compiled Span`1:Slice(int,int):Span`1:this [Tier0, IL size=39, code size=135]
 268: JIT compiled Span`1:.ctor(byref,int):this [Tier0, IL size=15, code size=39]
 269: JIT compiled Span`1:op_Implicit(Span`1):ReadOnlySpan`1 [Tier0, IL size=19, code size=90]
 270: JIT compiled ReadOnlySpan`1:.ctor(byref,int):this [Tier0, IL size=15, code size=39]
 271: JIT compiled WindowsConsoleStream:Write(ReadOnlySpan`1):this [Tier0, IL size=35, code size=149]
 272: JIT compiled WindowsConsoleStream:WriteFileNative(long,ReadOnlySpan`1,bool):int [Tier0, IL size=107, code size=272]
 273: JIT compiled ReadOnlySpan`1:get_IsEmpty():bool:this [Tier0, IL size=10, code size=24]
Hello, world!
 274: JIT compiled AppContext:OnProcessExit() [Tier0, IL size=43, code size=161]
 275: JIT compiled AssemblyLoadContext:OnProcessExit() [Tier0, IL size=101, code size=442]
 276: JIT compiled EventListener:DisposeOnShutdown() [Tier0, IL size=150, code size=618]
 277: JIT compiled List`1:.ctor():this [Tier0, IL size=18, code size=133]
 278: JIT compiled List`1:.cctor() [Tier0, IL size=12, code size=129]
 279: JIT compiled List`1:GetEnumerator():Enumerator:this [Tier0, IL size=7, code size=162]
 280: JIT compiled Enumerator:.ctor(List`1):this [Tier0, IL size=39, code size=64]
 281: JIT compiled Enumerator:MoveNext():bool:this [Tier0, IL size=81, code size=159]
 282: JIT compiled Enumerator:get_Current():__Canon:this [Tier0, IL size=7, code size=22]
 283: JIT compiled WeakReference`1:TryGetTarget(byref):bool:this [Tier0, IL size=24, code size=66]
 284: JIT compiled List`1:AddWithResize(__Canon):this [Tier0, IL size=39, code size=85]
 285: JIT compiled List`1:Grow(int):this [Tier0, IL size=53, code size=121]
 286: JIT compiled List`1:set_Capacity(int):this [Tier0, IL size=86, code size=342]
 287: JIT compiled CastHelpers:StelemRef_Helper(byref,long,Object) [Tier0, IL size=34, code size=104]
 288: JIT compiled CastHelpers:StelemRef_Helper_NoCacheLookup(byref,long,Object) [Tier0, IL size=26, code size=111]
 289: JIT compiled Enumerator:MoveNextRare():bool:this [Tier0, IL size=57, code size=80]
 290: JIT compiled Enumerator:Dispose():this [Tier0, IL size=1, code size=14]
 291: JIT compiled EventSource:Dispose():this [Tier0, IL size=14, code size=54]
 292: JIT compiled EventSource:Dispose(bool):this [Tier0, IL size=124, code size=236]
 293: JIT compiled EventProvider:Dispose():this [Tier0, IL size=14, code size=54]
 294: JIT compiled EventProvider:Dispose(bool):this [Tier0, IL size=90, code size=230]
 295: JIT compiled EventProvider:EventUnregister(long):this [Tier0, IL size=14, code size=50]
 296: JIT compiled EtwEventProvider:System.Diagnostics.Tracing.IEventProvider.EventUnregister(long):int:this [Tier0, IL size=7, code size=181]
 297: JIT compiled GC:SuppressFinalize(Object) [Tier0, IL size=18, code size=53]
 298: JIT compiled EventPipeEventProvider:System.Diagnostics.Tracing.IEventProvider.EventUnregister(long):int:this [Tier0, IL size=13, code size=187]
```

现在,让我们继续讨论实际的性能改进,从on-stack replacement(栈上替换)开始.

#### On-Stack Replacement(OSR,栈上替换)

在栈上替换(OSR)是在.NET 7中实现JIT的最酷的功能之一.但要真正理解OSR,我们首先需要理解分层编译,所以快速回顾一下…

使用JIT编译器的托管环境必须处理的问题之一是启动和吞吐量之间的权衡.从历史上看,编译器优化的任务是生成执行更快的代码,以便在应用程序或服务运行时实现尽可能最佳的吞吐量.但这种优化需要分析,需要时间,执行所有这些工作会导致启动时间增加,因为程序的所有代码(例如,在web服务器可以服务第一个请求之前需要运行的所有代码)都需要编译.因此,JIT编译器需要做出权衡：以更长的启动时间为代价提高吞吐量,或以降低吞吐量为代价提高启动时间.对于某些类型的应用程序和服务,折衷是一个简单的调用,例如,如果您的服务启动一次,然后运行几天,额外几秒的启动时间并不重要,或者如果您是一个控制台应用程序,将要进行快速计算并退出,启动时间才是最重要的.但是,JIT如何知道它处于哪个场景中,我们真的希望每个开发人员都知道这些设置和权衡,并相应地配置他们的每个应用程序吗?对此的一个答案是提前编译,它在.NET中采用了多种形式.例如,所有的核心库都是“crossgen”,这意味着它们已经通过一个工具运行,该工具生成了前面提到的R2R格式,生成的二进制文件包含汇编代码,只需要稍加调整即可实际执行；不是每个方法都可以为其生成代码,但足以显著减少启动时间.当然,这种方法也有其自身的缺点,例如,JIT编译器的一个承诺是,它可以利用当前机器/进程的知识进行最佳优化,因此,例如,R2R映像必须假设某个基线指令集(例如,哪些矢量化指令可用),而JIT可以看到哪些实际可用并使用最佳.“分层编译”提供了另一个答案,无论是否使用这些其他提前(AOT)编译解决方案,都可以使用.

分层编译使JIT能够鱼与熊掌兼得.分层编译这个想法很简单:允许JIT多次编译相同的代码.第一次,JIT可以使用尽可能少的优化(少量优化实际上可以使JIT自身的吞吐量更快,因此这些优化仍然适用),生成相当未优化的汇编代码,但速度非常快.当它这样做时,它可以在程序集中添加一些工具来跟踪方法的调用频率.事实证明,在启动路径上使用的许多函数被调用一次,或者可能只调用了几次,优化它们比不优化执行它们需要更多的时间.然后,当方法的插装触发某个阈值时,例如,一个方法已执行30次,工作项将排队重新编译该方法,但这一次JIT可以对其进行所有优化.这被亲切地称为“分层”.一旦重新编译完成,方法的调用站点就会用新高度优化的汇编代码的地址进行修补,未来的调用将采用快速路径.因此,我们获得了更快的启动和更快的持续吞吐量.

然而,一个问题是不适合这种模式的方法.当然,许多性能敏感的方法都相对较快,执行了很多次,但也有大量性能敏感方法只执行了几次,甚至可能只执行一次,但执行需要很长时间,甚至可能是整个过程的持续时间:带循环的方法.因此,默认情况下,分层编译未应用于循环,但可以通过将DOTNET_TC_QuickJitForLoops环境变量设置为1来启用.我们可以通过尝试使用.NET 6的简单控制台应用程序来查看其效果.使用默认设置,运行此应用程序:

```csharp
class Program
{
    static void Main()
    {
        var sw = new System.Diagnostics.Stopwatch();
        while (true)
        {
            sw.Restart();
            for (int trial = 0; trial < 10_000; trial++)
            {
                int count = 0;
                for (int i = 0; i < char.MaxValue; i++)
                    if (IsAsciiDigit((char)i))
                        count++;
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        static bool IsAsciiDigit(char c) => (uint)(c - '0') <= 9;
    }
}
```

输出内容:

```csharp
00:00:00.5734352
00:00:00.5526667
00:00:00.5675267
00:00:00.5588724
00:00:00.5616028
```

现在,尝试将DOTNET_TC_QuickJitForLoops设置为1.当我再次运行它时,得到如下数字:

```csharp
00:00:01.2841397
00:00:01.2693485
00:00:01.2755646
00:00:01.2656678
00:00:01.2679925
```

换句话说,启用DOTNET_TC_QuickJitForLoops时,需要的时间是不启用时的2.5倍(在.NET6中).这是因为这个主函数从未应用过优化.通过将DOTNET_TC_QuickJitForLoops设置为1,我们说“JIT,请将分层也应用于具有循环的方法”,但这种具有循环的方式只调用一次,因此在整个过程中,它最终保持在“tier-0”,即未优化.现在,让我们对.NET 7进行同样的尝试.无论是否设置了环境变量,我都会再次得到如下数字:

```csharp
00:00:00.5528889
00:00:00.5562563
00:00:00.5622086
00:00:00.5668220
00:00:00.5589112
```

但重要的是,这种方法仍然参与了分层.事实上,我们可以通过使用前面提到的DOTNET_JitDisasmSummary=1环境变量来确认.当我设置并再次运行时,我在输出中看到这些行:

```csharp
   4: JIT compiled Program:Main() [Tier0, IL size=83, code size=319]
...
   6: JIT compiled Program:Main() [Tier1-OSR @0x27, IL size=83, code size=380]
```

栈上替换的思想是,方法不仅可以在调用之间替换,而且可以在“栈上”执行时替换.除了为调用计数检测第0层代码外,还为迭代计数检测循环.当迭代超过某个限制时,JIT编译该方法的新的高度优化版本,将所有本地/寄存器状态从当前调用转移到新调用,然后跳到新方法中的适当位置.通过使用前面讨论的DOTNET_JitDisasm环境变量,我们可以看到这一点.将其设置为Program:*以查看为Program类中的所有方法生成的汇编代码,然后再次运行应用程序.您应该看到如下输出:

```assembly
; Assembly listing for method Program:Main()
; Emitting BLENDED_CODE for X64 CPU with AVX - Windows
; Tier-0 compilation
; MinOpts code
; rbp based frame
; partially interruptible

G_M000_IG01:                ;; offset=0000H
       55                   push     rbp
       4881EC80000000       sub      rsp, 128
       488DAC2480000000     lea      rbp, [rsp+80H]
       C5D857E4             vxorps   xmm4, xmm4
       C5F97F65B0           vmovdqa  xmmword ptr [rbp-50H], xmm4
       33C0                 xor      eax, eax
       488945C0             mov      qword ptr [rbp-40H], rax

G_M000_IG02:                ;; offset=001FH
       48B9002F0B50FC7F0000 mov      rcx, 0x7FFC500B2F00
       E8721FB25F           call     CORINFO_HELP_NEWSFAST
       488945B0             mov      gword ptr [rbp-50H], rax
       488B4DB0             mov      rcx, gword ptr [rbp-50H]
       FF1544C70D00         call     [Stopwatch:.ctor():this]
       488B4DB0             mov      rcx, gword ptr [rbp-50H]
       48894DC0             mov      gword ptr [rbp-40H], rcx
       C745A8E8030000       mov      dword ptr [rbp-58H], 0x3E8

G_M000_IG03:                ;; offset=004BH
       8B4DA8               mov      ecx, dword ptr [rbp-58H]
       FFC9                 dec      ecx
       894DA8               mov      dword ptr [rbp-58H], ecx
       837DA800             cmp      dword ptr [rbp-58H], 0
       7F0E                 jg       SHORT G_M000_IG05

G_M000_IG04:                ;; offset=0059H
       488D4DA8             lea      rcx, [rbp-58H]
       BA06000000           mov      edx, 6
       E8B985AB5F           call     CORINFO_HELP_PATCHPOINT

G_M000_IG05:                ;; offset=0067H
       488B4DC0             mov      rcx, gword ptr [rbp-40H]
       3909                 cmp      dword ptr [rcx], ecx
       FF1585C70D00         call     [Stopwatch:Restart():this]
       33C9                 xor      ecx, ecx
       894DBC               mov      dword ptr [rbp-44H], ecx
       33C9                 xor      ecx, ecx
       894DB8               mov      dword ptr [rbp-48H], ecx
       EB20                 jmp      SHORT G_M000_IG08

G_M000_IG06:                ;; offset=007FH
       8B4DB8               mov      ecx, dword ptr [rbp-48H]
       0FB7C9               movzx    rcx, cx
       FF152DD40B00         call     [Program:<Main>g__IsAsciiDigit|0_0(ushort):bool]
       85C0                 test     eax, eax
       7408                 je       SHORT G_M000_IG07
       8B4DBC               mov      ecx, dword ptr [rbp-44H]
       FFC1                 inc      ecx
       894DBC               mov      dword ptr [rbp-44H], ecx

G_M000_IG07:                ;; offset=0097H
       8B4DB8               mov      ecx, dword ptr [rbp-48H]
       FFC1                 inc      ecx
       894DB8               mov      dword ptr [rbp-48H], ecx

G_M000_IG08:                ;; offset=009FH
       8B4DA8               mov      ecx, dword ptr [rbp-58H]
       FFC9                 dec      ecx
       894DA8               mov      dword ptr [rbp-58H], ecx
       837DA800             cmp      dword ptr [rbp-58H], 0
       7F0E                 jg       SHORT G_M000_IG10

G_M000_IG09:                ;; offset=00ADH
       488D4DA8             lea      rcx, [rbp-58H]
       BA23000000           mov      edx, 35
       E86585AB5F           call     CORINFO_HELP_PATCHPOINT

G_M000_IG10:                ;; offset=00BBH
       817DB800CA9A3B       cmp      dword ptr [rbp-48H], 0x3B9ACA00
       7CBB                 jl       SHORT G_M000_IG06
       488B4DC0             mov      rcx, gword ptr [rbp-40H]
       3909                 cmp      dword ptr [rcx], ecx
       FF1570C70D00         call     [Stopwatch:get_ElapsedMilliseconds():long:this]
       488BC8               mov      rcx, rax
       FF1507D00D00         call     [Console:WriteLine(long)]
       E96DFFFFFF           jmp      G_M000_IG03

; Total bytes of code 222

; Assembly listing for method Program:<Main>g__IsAsciiDigit|0_0(ushort):bool
; Emitting BLENDED_CODE for X64 CPU with AVX - Windows
; Tier-0 compilation
; MinOpts code
; rbp based frame
; partially interruptible

G_M000_IG01:                ;; offset=0000H
       55                   push     rbp
       488BEC               mov      rbp, rsp
       894D10               mov      dword ptr [rbp+10H], ecx

G_M000_IG02:                ;; offset=0007H
       8B4510               mov      eax, dword ptr [rbp+10H]
       0FB7C0               movzx    rax, ax
       83C0D0               add      eax, -48
       83F809               cmp      eax, 9
       0F96C0               setbe    al
       0FB6C0               movzx    rax, al

G_M000_IG03:                ;; offset=0019H
       5D                   pop      rbp
       C3                   ret
```

这里有一些相关的事情需要注意.首先，顶部的注释强调了这段代码是如何编译的:

```assembly
; Tier-0 compilation
; MinOpts code
```

因此,我们知道这是用最小优化(“*MinOpts*”)编译的方法的初始版本(“第0层”).第二，注意装配的这一行：

```assembly
FF152DD40B00         call     [Program:<Main>g__IsAsciiDigit|0_0(ushort):bool]
```

我们的IsAsciiDigit辅助方法是简单的可内联的,但它没有内联;相反,程序集有一个对它的调用,事实上,我们可以在下面看到为IsAsciiDigit生成的代码(也是“MinOpts”).为什么?因为内联是一种优化(非常重要的优化),但在tier-0中被禁用(因为为了进行良好的内联分析也非常昂贵).第三,我们可以看到JIT输出到检测该方法的代码.这有点复杂,但我会指出相关部分.首先,我们看到:

```assembly
C745A8E8030000       mov      dword ptr [rbp-58H], 0x3E8
```

0x3E8是十进制1000的十六进制值,这是在JIT生成方法的优化版本之前循环需要迭代的默认迭代次数(可通过环境变量*DOTNET_TC_OnStackReplacement_InitialCounter*进行配置).因此,我们看到1000存储在这个堆栈位置.然后,在该方法的后面,我们看到:

```assembly
G_M000_IG03:                ;; offset=004BH
       8B4DA8               mov      ecx, dword ptr [rbp-58H]
       FFC9                 dec      ecx
       894DA8               mov      dword ptr [rbp-58H], ecx
       837DA800             cmp      dword ptr [rbp-58H], 0
       7F0E                 jg       SHORT G_M000_IG05

G_M000_IG04:                ;; offset=0059H
       488D4DA8             lea      rcx, [rbp-58H]
       BA06000000           mov      edx, 6
       E8B985AB5F           call     CORINFO_HELP_PATCHPOINT

G_M000_IG05:                ;; offset=0067H
```

生成的代码将该计数器加载到ecx寄存器中,将其递减,存储回,然后查看计数器是否下降到0.如果没有下降,则代码跳转到G_M000_IG05,这是循环其余部分中实际代码的标签.但如果计数器下降到0,JIT将继续将相关状态存储到rcx和edx寄存器中,然后调用CORINFO_HELP_PATCHPOINT helper方法.该助手负责触发优化方法的创建（如果它还不存在）,修复所有适当的跟踪状态,并跳转到新方法.事实上,如果您再次查看运行程序的控制台输出,您将看到主方法的另一个输出:

```assembly
; Assembly listing for method Program:Main()
; Emitting BLENDED_CODE for X64 CPU with AVX - Windows
; Tier-1 compilation
; OSR variant for entry point 0x23
; optimized code
; rsp based frame
; fully interruptible
; No PGO data
; 1 inlinees with PGO data; 8 single block inlinees; 0 inlinees without PGO data

G_M000_IG01:                ;; offset=0000H
       4883EC58             sub      rsp, 88
       4889BC24D8000000     mov      qword ptr [rsp+D8H], rdi
       4889B424D0000000     mov      qword ptr [rsp+D0H], rsi
       48899C24C8000000     mov      qword ptr [rsp+C8H], rbx
       C5F877               vzeroupper
       33C0                 xor      eax, eax
       4889442428           mov      qword ptr [rsp+28H], rax
       4889442420           mov      qword ptr [rsp+20H], rax
       488B9C24A0000000     mov      rbx, gword ptr [rsp+A0H]
       8BBC249C000000       mov      edi, dword ptr [rsp+9CH]
       8BB42498000000       mov      esi, dword ptr [rsp+98H]

G_M000_IG02:                ;; offset=0041H
       EB45                 jmp      SHORT G_M000_IG05
                            align    [0 bytes for IG06]

G_M000_IG03:                ;; offset=0043H
       33C9                 xor      ecx, ecx
       488B9C24A0000000     mov      rbx, gword ptr [rsp+A0H]
       48894B08             mov      qword ptr [rbx+08H], rcx
       488D4C2428           lea      rcx, [rsp+28H]
       48B87066E68AFD7F0000 mov      rax, 0x7FFD8AE66670

G_M000_IG04:                ;; offset=0060H
       FFD0                 call     rax ; Kernel32:QueryPerformanceCounter(long):int
       488B442428           mov      rax, qword ptr [rsp+28H]
       488B9C24A0000000     mov      rbx, gword ptr [rsp+A0H]
       48894310             mov      qword ptr [rbx+10H], rax
       C6431801             mov      byte  ptr [rbx+18H], 1
       33FF                 xor      edi, edi
       33F6                 xor      esi, esi
       833D92A1E55F00       cmp      dword ptr [(reloc 0x7ffcafe1ae34)], 0
       0F85CA000000         jne      G_M000_IG13

G_M000_IG05:                ;; offset=0088H
       81FE00CA9A3B         cmp      esi, 0x3B9ACA00
       7D17                 jge      SHORT G_M000_IG09

G_M000_IG06:                ;; offset=0090H
       0FB7CE               movzx    rcx, si
       83C1D0               add      ecx, -48
       83F909               cmp      ecx, 9
       7702                 ja       SHORT G_M000_IG08

G_M000_IG07:                ;; offset=009BH
       FFC7                 inc      edi

G_M000_IG08:                ;; offset=009DH
       FFC6                 inc      esi
       81FE00CA9A3B         cmp      esi, 0x3B9ACA00
       7CE9                 jl       SHORT G_M000_IG06

G_M000_IG09:                ;; offset=00A7H
       488B6B08             mov      rbp, qword ptr [rbx+08H]
       48899C24A0000000     mov      gword ptr [rsp+A0H], rbx
       807B1800             cmp      byte  ptr [rbx+18H], 0
       7436                 je       SHORT G_M000_IG12

G_M000_IG10:                ;; offset=00B9H
       488D4C2420           lea      rcx, [rsp+20H]
       48B87066E68AFD7F0000 mov      rax, 0x7FFD8AE66670

G_M000_IG11:                ;; offset=00C8H
       FFD0                 call     rax ; Kernel32:QueryPerformanceCounter(long):int
       488B4C2420           mov      rcx, qword ptr [rsp+20H]
       488B9C24A0000000     mov      rbx, gword ptr [rsp+A0H]
       482B4B10             sub      rcx, qword ptr [rbx+10H]
       4803E9               add      rbp, rcx
       833D2FA1E55F00       cmp      dword ptr [(reloc 0x7ffcafe1ae34)], 0
       48899C24A0000000     mov      gword ptr [rsp+A0H], rbx
       756D                 jne      SHORT G_M000_IG14

G_M000_IG12:                ;; offset=00EFH
       C5F857C0             vxorps   xmm0, xmm0
       C4E1FB2AC5           vcvtsi2sd  xmm0, rbp
       C5FB11442430         vmovsd   qword ptr [rsp+30H], xmm0
       48B9F04BF24FFC7F0000 mov      rcx, 0x7FFC4FF24BF0
       BAE7070000           mov      edx, 0x7E7
       E82E1FB25F           call     CORINFO_HELP_GETSHARED_NONGCSTATIC_BASE
       C5FB10442430         vmovsd   xmm0, qword ptr [rsp+30H]
       C5FB5905E049F6FF     vmulsd   xmm0, xmm0, qword ptr [(reloc 0x7ffc4ff25720)]
       C4E1FB2CD0           vcvttsd2si  rdx, xmm0
       48B94B598638D6C56D34 mov      rcx, 0x346DC5D63886594B
       488BC1               mov      rax, rcx
       48F7EA               imul     rdx:rax, rdx
       488BCA               mov      rcx, rdx
       48C1E93F             shr      rcx, 63
       48C1FA0B             sar      rdx, 11
       4803CA               add      rcx, rdx
       FF1567CE0D00         call     [Console:WriteLine(long)]
       E9F5FEFFFF           jmp      G_M000_IG03

G_M000_IG13:                ;; offset=014EH
       E8DDCBAC5F           call     CORINFO_HELP_POLL_GC
       E930FFFFFF           jmp      G_M000_IG05

G_M000_IG14:                ;; offset=0158H
       E8D3CBAC5F           call     CORINFO_HELP_POLL_GC
       EB90                 jmp      SHORT G_M000_IG12

; Total bytes of code 351
```

这里,我们再次注意到一些有趣的事情.首先,在头文件中我们看到:

```assembly
; Tier-1 compilation
; OSR variant for entry point 0x23
; optimized code
```

因此,我们知道这既是优化的“第1层”代码,也是该方法的“OSR变体”.其次,请注意,不再调用IsAsciiDigit方法.相反,我们看到的是,该调用的位置:

```assembly
G_M000_IG06:                ;; offset=0090H
       0FB7CE               movzx    rcx, si
       83C1D0               add      ecx, -48
       83F909               cmp      ecx, 9
       7702                 ja       SHORT G_M000_IG08
```

这是将一个值加载到rcx中,从中减去48(48是“0”字符的十进制ASCII值),并将结果值与9进行比较.听起来很像我们的IsasciidGit实现( (uint)(c-“0”)<=9 ),不是吗？这是因为它是.帮助程序成功地内联到现在优化的代码中.

很好,现在在.NET7中,我们可以在很大程度上避免启动和吞吐量之间的权衡,因为OSR使分层编译能够应用于所有方法,甚至是那些长期运行的方法.许多提交开始启用此功能,包括过去几年中的许多提交,但是所有的功能在发布时都被禁用了.由于dotnet/runtime#62831在Arm64上实现了对OSR的支持(之前仅实现了x64支持),以及dotnet/Runtime#63406和dotnet/runtime#65609修改了如何OSR导入和epilog的处理,dotnet/runtime#65675在默认情况下启用OSR(DOTNET_TC_QuickJitForLoops=1).

但是,分层编译和OSR不仅仅是关于启动(尽管它们在那里当然非常有价值).他们还将进一步提高吞吐量.尽管分层编译最初被设想为一种在不影响吞吐量的情况下优化启动的方法,但它已经远远不止于此.JIT可以在tier-0期间了解到关于方法的各种信息,然后可以用于tier-1.例如,执行的tier-2代码意味着该方法访问的任何静态都将被初始化,这意味着任何只读静态不仅在执行tier-3代码时已经初始化,而且它们的值永远不会改变.这反过来意味着,任何原始类型的只读静态(如bool、int等)都可以被视为常量,而不是静态只读字段,并且在第1层编译期间,JIT可以优化它们,就像优化常量一样.例如,在将DOTNET_JitDisasm设置为Program:Test后,尝试运行以下简单程序:

```csharp
using System.Runtime.CompilerServices;

class Program
{
    static readonly bool Is64Bit = Environment.Is64BitProcess;

    static int Main()
    {
        int count = 0;
        for (int i = 0; i < 1_000_000_000; i++)
            if (Test())
                count++;
        return count;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static bool Test() => Is64Bit;
}
```

当我这样做时,我得到以下输出:

```assembly
; Assembly listing for method Program:Test():bool
; Emitting BLENDED_CODE for X64 CPU with AVX - Windows
; Tier-0 compilation
; MinOpts code
; rbp based frame
; partially interruptible

G_M000_IG01:                ;; offset=0000H
       55                   push     rbp
       4883EC20             sub      rsp, 32
       488D6C2420           lea      rbp, [rsp+20H]

G_M000_IG02:                ;; offset=000AH
       48B9B8639A3FFC7F0000 mov      rcx, 0x7FFC3F9A63B8
       BA01000000           mov      edx, 1
       E8C220B25F           call     CORINFO_HELP_GETSHARED_NONGCSTATIC_BASE
       0FB60545580C00       movzx    rax, byte  ptr [(reloc 0x7ffc3f9a63ea)]

G_M000_IG03:                ;; offset=0025H
       4883C420             add      rsp, 32
       5D                   pop      rbp
       C3                   ret

; Total bytes of code 43

; Assembly listing for method Program:Test():bool
; Emitting BLENDED_CODE for X64 CPU with AVX - Windows
; Tier-1 compilation
; optimized code
; rsp based frame
; partially interruptible
; No PGO data

G_M000_IG01:                ;; offset=0000H

G_M000_IG02:                ;; offset=0000H
       B801000000           mov      eax, 1

G_M000_IG03:                ;; offset=0005H
       C3                   ret

; Total bytes of code 6
```

注意,我们再次看到程序的两个输出:Test方法的汇编代码.首先,我们看到“Tier-0”代码,它正在访问静态（注意调用CORINFO_HELP_GETSHARED_NONGCSTATIC_BASE指令).但是,我们看到了“Tier-1”代码,其中所有的开销都消失了,取而代之的是mov eax,1.由于必须执行“Tier-0”代码才能分层,“Tier-2”代码是在知道静态只读bool IS64位字段的值为真的情况下生成的（1),因此,整个方法是将值1存储到用于返回值的eax寄存器中.

这非常有用,现在编写组件时都考虑到了分层.考虑一下新的Regex源代码生成器,这将在后面的文章中讨论(Roslyn源代码生成器是几年前引入的;就像Roslyn分析器能够插入编译器并基于编译器从源代码中学习到的所有数据提供额外的诊断一样,Roslyn源代码生成器能够分析相同的数据,然后用额外的源代码进一步增加编译单元).Regex源生成器应用基于此的dotnet/runtime#67775技术.Regex支持设置流程范围的超时,该超时将应用于没有显式设置超时的Regex实例.这意味着,即使设置这样一个进程范围的超时非常罕见,Regex源生成器仍然需要输出与超时相关的代码,以备需要.它通过输出一些helper来实现,像这样:

```csharp
static class Utilities
{
    internal static readonly TimeSpan s_defaultTimeout = AppContext.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") is TimeSpan timeout ? timeout : Timeout.InfiniteTimeSpan;
    internal static readonly bool s_hasTimeout = s_defaultTimeout != Timeout.InfiniteTimeSpan;
}
```

然后调用的方式,如下所示:

```csharp
if (Utilities.s_hasTimeout)
{
    base.CheckTimeout();
}
```

在第0层中,这些检查仍然会在程序集代码中发出,但在吞吐量很重要的第1层中,如果没有设置相关的AppContext开关,那么s_defaultTimeout将会是Timeout.infinittimeespan,此时s_hasTimeout将为false.由于s_hasTimeout是一个静态的只读bool, JIT将能够将其视为const,并且所有的条件,如if (Utilities.s_hasTimeout)将被视为与if (false)相等,并从汇编代码中完全清除为死代码.

但是,这有点老生常谈了.自从.NET Core 3.0引入分层编译以来,JIT就能够进行这样的优化.不过,现在在.NET 7中,在OSR中,它也可以在默认情况下对带有循环的方法进行优化(因此启用了类似于正则表达式的情况).然而,当OSR与另一个令人兴奋的特性：动态PGO相结合时,OSR的真正魔力开始发挥作用.

#### PGO

我在我的 [Performance Improvements in .NET 6](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-6) 文章中写过关于配置文件引导优化(PGO)的内容,但在这里我将再次介绍它,因为它为.net 7带来了大量的改进.

PGO已经存在很长一段时间了,存在于各种语言和编译器中.基本思想是,在编译应用程序时,要求编译器向应用程序中注入工具来跟踪各种有趣的信息.然后让应用运行,运行各种常见场景,使该工具““分析”应用执行时发生的情况,然后保存结果.然后应用程序被重新编译,将这些检测结果反馈给编译器,并允许它优化应用程序,以准确地实现预期的使用方式.这种PGO的方法被称为“静态PGO”,因为所有的信息都是在实际部署之前收集的,多年来在.Net以各种形式一直在做这种事情.不过,在我看来,在.Net中真正有趣的开发是“动态PGO”,它是在.Net 6中引入的,但默认是关闭的.

动态PGO利用了分层编译的优势.我注意到JIT利用0层代码来跟踪方法被调用了多少次,或者在循环的情况下,跟踪循环执行了多少次.它也可以用它来做其他事情.例如,它可以准确地跟踪将哪些具体类型用作接口分派的目标,然后在第1层专门化代码以期望最常见的类型(这被称为“受保护的去虚拟化”或GDV).你可以在这个小例子中看到.设置<font color=red>DOTNET_TieredPGO</font>环境变量为1,然后在.Net 7上运行:

```csharp
class Program
{
    static void Main()
    {
        IPrinter printer = new Printer();
        for (int i = 0; ; i++)
        {
            DoWork(printer, i);
        }
    }

    static void DoWork(IPrinter printer, int i)
    {
        printer.PrintIfTrue(i == int.MaxValue);
    }

    interface IPrinter
    {
        void PrintIfTrue(bool condition);
    }

    class Printer : IPrinter
    {
        public void PrintIfTrue(bool condition)
        {
            if (condition) Console.WriteLine("Print!");
        }
    }
}
```

DoWork的tier-0代码看起来是这样的:

```assembly
G_M000_IG01:                ;; offset=0000H
       55                   push     rbp
       4883EC30             sub      rsp, 48
       488D6C2430           lea      rbp, [rsp+30H]
       33C0                 xor      eax, eax
       488945F8             mov      qword ptr [rbp-08H], rax
       488945F0             mov      qword ptr [rbp-10H], rax
       48894D10             mov      gword ptr [rbp+10H], rcx
       895518               mov      dword ptr [rbp+18H], edx

G_M000_IG02:                ;; offset=001BH
       FF059F220F00         inc      dword ptr [(reloc 0x7ffc3f1b2ea0)]
       488B4D10             mov      rcx, gword ptr [rbp+10H]
       48894DF8             mov      gword ptr [rbp-08H], rcx
       488B4DF8             mov      rcx, gword ptr [rbp-08H]
       48BAA82E1B3FFC7F0000 mov      rdx, 0x7FFC3F1B2EA8
       E8B47EC55F           call     CORINFO_HELP_CLASSPROFILE32
       488B4DF8             mov      rcx, gword ptr [rbp-08H]
       48894DF0             mov      gword ptr [rbp-10H], rcx
       488B4DF0             mov      rcx, gword ptr [rbp-10H]
       33D2                 xor      edx, edx
       817D18FFFFFF7F       cmp      dword ptr [rbp+18H], 0x7FFFFFFF
       0F94C2               sete     dl
       49BB0800F13EFC7F0000 mov      r11, 0x7FFC3EF10008
       41FF13               call     [r11]IPrinter:PrintIfTrue(bool):this
       90                   nop

G_M000_IG03:                ;; offset=0062H
       4883C430             add      rsp, 48
       5D                   pop      rbp
       C3                   ret
```

最值得注意的是,你可以看到调用<font color=red>call [r11]IPrinter:PrintIfTrue(bool):this</font>做接口分发.但是,接下来看看为第1层生成的代码.我们仍然看到调用<font color =red >[r11]IPrinter:PrintIfTrue(bool):this </font>,但我们也看到了这个:

```assembly
G_M000_IG02:                ;; offset=0020H
       48B9982D1B3FFC7F0000 mov      rcx, 0x7FFC3F1B2D98
       48390F               cmp      qword ptr [rdi], rcx
       7521                 jne      SHORT G_M000_IG05
       81FEFFFFFF7F         cmp      esi, 0x7FFFFFFF
       7404                 je       SHORT G_M000_IG04

G_M000_IG03:                ;; offset=0037H
       FFC6                 inc      esi
       EBE5                 jmp      SHORT G_M000_IG02

G_M000_IG04:                ;; offset=003BH
       48B9D820801A24020000 mov      rcx, 0x2241A8020D8
       488B09               mov      rcx, gword ptr [rcx]
       FF1572CD0D00         call     [Console:WriteLine(String)]
       EBE7                 jmp      SHORT G_M000_IG03
```

第一个代码块检查IPrinter的具体类型(存储在rdi中),并将其与Printer的已知类型(0x7FFC3F1B2D98)进行比较.如果它们不同,它就跳转到在未优化版本中执行的相同接口分派.但如果它们是相同的,则直接跳转到打印机的内联版本.PrintIfTrue(你可以在这个方法中看到对Console:WriteLine的调用).因此,通常的情况(本例中唯一的情况)是以单个比较和分支为代价的超级高效.

在.NET6中已经有了,那么为什么我们现在要讨论它呢?有几个方面有所改善.首先,由于dotnet/runtime#61453等改进,PGO现在与OSR一起工作.这是一件大事,因为这意味着执行这种接口分发(这是相当常见的)的长时间运行的热方法可以获得这种类型的去虚拟化/内联优化.第二,虽然PGO目前没有默认启用,但我们已经让它更容易启用.在dotnet/runtime#71438和dotnet/sdk#26350之间,现在可以简单地把 <font color=red><TieredPGO>true</TieredPGO><font> 在你的项目工程文件(*.csproj)中,它会有相同的效果,如果你设置DOTNET_TieredPGO=1之前的应用程序调用,启用动态PGO(注意,它不禁用R2R图像,所以如果你想要整个核心库也采用动态PGO,你还需要设置DOTNET_ReadyToRun=0).第三,动态PGO已经学会了如何测量和优化附加的东西.

```csharp
using System.Runtime.CompilerServices;

class Program
{
    static int[] s_values = Enumerable.Range(0, 1_000).ToArray();

    static void Main()
    {
        for (int i = 0; i < 1_000_000; i++)
            Sum(s_values, i => i * 42);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static int Sum(int[] values, Func<int, int> func)
    {
        int sum = 0;
        foreach (int value in values)
            sum += func(value);
        return sum;
    }
}
```

如果未启用PGO,则生成的优化程序集如下:

```assembly
; Assembly listing for method Program:Sum(ref,Func`2):int
; Emitting BLENDED_CODE for X64 CPU with AVX - Windows
; Tier-1 compilation
; optimized code
; rsp based frame
; partially interruptible
; No PGO data

G_M000_IG01:                ;; offset=0000H
       4156                 push     r14
       57                   push     rdi
       56                   push     rsi
       55                   push     rbp
       53                   push     rbx
       4883EC20             sub      rsp, 32
       488BF2               mov      rsi, rdx

G_M000_IG02:                ;; offset=000DH
       33FF                 xor      edi, edi
       488BD9               mov      rbx, rcx
       33ED                 xor      ebp, ebp
       448B7308             mov      r14d, dword ptr [rbx+08H]
       4585F6               test     r14d, r14d
       7E16                 jle      SHORT G_M000_IG04

G_M000_IG03:                ;; offset=001DH
       8BD5                 mov      edx, ebp
       8B549310             mov      edx, dword ptr [rbx+4*rdx+10H]
       488B4E08             mov      rcx, gword ptr [rsi+08H]
       FF5618               call     [rsi+18H]Func`2:Invoke(int):int:this
       03F8                 add      edi, eax
       FFC5                 inc      ebp
       443BF5               cmp      r14d, ebp
       7FEA                 jg       SHORT G_M000_IG03

G_M000_IG04:                ;; offset=0033H
       8BC7                 mov      eax, edi

G_M000_IG05:                ;; offset=0035H
       4883C420             add      rsp, 32
       5B                   pop      rbx
       5D                   pop      rbp
       5E                   pop      rsi
       5F                   pop      rdi
       415E                 pop      r14
       C3                   ret

; Total bytes of code 64
```

注意<font color=red> call [rsi+18H]Func ' 2:Invoke(int):int:this </font>在那里调用委托.现在启用PGO:

```assembly
; Assembly listing for method Program:Sum(ref,Func`2):int
; Emitting BLENDED_CODE for X64 CPU with AVX - Windows
; Tier-1 compilation
; optimized code
; optimized using profile data
; rsp based frame
; fully interruptible
; with Dynamic PGO: edge weights are valid, and fgCalledCount is 5628
; 0 inlinees with PGO data; 1 single block inlinees; 0 inlinees without PGO data

G_M000_IG01:                ;; offset=0000H
       4157                 push     r15
       4156                 push     r14
       57                   push     rdi
       56                   push     rsi
       55                   push     rbp
       53                   push     rbx
       4883EC28             sub      rsp, 40
       488BF2               mov      rsi, rdx

G_M000_IG02:                ;; offset=000FH
       33FF                 xor      edi, edi
       488BD9               mov      rbx, rcx
       33ED                 xor      ebp, ebp
       448B7308             mov      r14d, dword ptr [rbx+08H]
       4585F6               test     r14d, r14d
       7E27                 jle      SHORT G_M000_IG05

G_M000_IG03:                ;; offset=001FH
       8BC5                 mov      eax, ebp
       8B548310             mov      edx, dword ptr [rbx+4*rax+10H]
       4C8B4618             mov      r8, qword ptr [rsi+18H]
       48B8A0C2CF3CFC7F0000 mov      rax, 0x7FFC3CCFC2A0
       4C3BC0               cmp      r8, rax
       751D                 jne      SHORT G_M000_IG07
       446BFA2A             imul     r15d, edx, 42

G_M000_IG04:                ;; offset=003CH
       4103FF               add      edi, r15d
       FFC5                 inc      ebp
       443BF5               cmp      r14d, ebp
       7FD9                 jg       SHORT G_M000_IG03

G_M000_IG05:                ;; offset=0046H
       8BC7                 mov      eax, edi

G_M000_IG06:                ;; offset=0048H
       4883C428             add      rsp, 40
       5B                   pop      rbx
       5D                   pop      rbp
       5E                   pop      rsi
       5F                   pop      rdi
       415E                 pop      r14
       415F                 pop      r15
       C3                   ret

G_M000_IG07:                ;; offset=0055H
       488B4E08             mov      rcx, gword ptr [rsi+08H]
       41FFD0               call     r8
       448BF8               mov      r15d, eax
       EBDB                 jmp      SHORT G_M000_IG04
```

我选择了<font color=red>i=> i * 42</font>中的<font color=red>42</font>常数,以便于在程序集中看到它,果然,它就在那里:

```assembly
G_M000_IG03:                ;; offset=001FH
       8BC5                 mov      eax, ebp
       8B548310             mov      edx, dword ptr [rbx+4*rax+10H]
       4C8B4618             mov      r8, qword ptr [rsi+18H]
       48B8A0C2CF3CFC7F0000 mov      rax, 0x7FFC3CCFC2A0
       4C3BC0               cmp      r8, rax
       751D                 jne      SHORT G_M000_IG07
       446BFA2A             imul     r15d, edx, 42
```

这是将目标地址从委托加载到<font color=red>r8</font>,并将预期目标的地址加载到<font color=red>  rax</font>.如果它们相同,它就简单地执行内联操作(<font color=red>imul r15d, edx, 42</font>),否则就跳转到G_M000_IG07,后者调用<font color=red>r8</font>中的函数.如果我们将其作为基准运行,效果会很明显:

```csharp
static int[] s_values = Enumerable.Range(0, 1_000).ToArray();

[Benchmark]
public int DelegatePGO() => Sum(s_values, i => i * 42);

static int Sum(int[] values, Func<int, int>? func)
{
    int sum = 0;
    foreach (int value in values)
    {
        sum += func(value);
    }
    return sum;
}
```

禁用PGO后,我们在.Net 6和.Net 7上获得了相同的性能吞吐量:

| Method      | Runtime  |     Mean | Ratio |
| ----------- | -------- | -------: | ----: |
| DelegatePGO | .NET 6.0 | 1.665 us |  1.00 |
| DelegatePGO | .NET 7.0 | 1.659 us |  1.00 |

但是当我们启用动态PGO(DOTNET_TieredPGO=1)时,情况就不同了. .Net 6会快14%,而.Net 7会快3倍!

| Method      | Runtime  |       Mean | Ratio |
| ----------- | -------- | ---------: | ----: |
| DelegatePGO | .NET 6.0 | 1,427.7 ns |  1.00 |
| DelegatePGO | .NET 7.0 |   539.0 ns |  0.38 |

dotnet/runtime#70377是动态PGO的另一个有价值的改进,它使PGO能够很好地进行循环克隆和不变量提升.为了更好地理解这一点,我们稍微离题一下这些是什么.循环克隆是JIT用于避免循环快速路径中的各种开销的一种机制.考虑下面这个例子中的<font color=red>Test</font>方法:

```csharp
using System.Runtime.CompilerServices;

class Program
{
    static void Main()
    {
        int[] array = new int[10_000_000];
        for (int i = 0; i < 1_000_000; i++)
        {
            Test(array);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool Test(int[] array)
    {
        for (int i = 0; i < 0x12345; i++)
        {
            if (array[i] == 42)
            {
                return true;
            }
        }

        return false;
    }
}
```

JIT不知道传入的数组是否足够长,使得循环中所有对数组[i]的访问都在边界内,因此它需要为每次访问注入边界检查.虽然在前面简单地进行长度检查并在长度不够时提前抛出异常是一件好事,但这样做也会改变行为(想象一下这个方法正在写入数组,或者改变一些共享状态).相反,JIT使用“循环克隆”.它本质上重写了<font color=red>Test</font>方法,使其更像这样:

```csharp
if (array is not null && array.Length >= 0x12345)
{
    for (int i = 0; i < 0x12345; i++)
    {
        if (array[i] == 42) // no bounds checks emitted for this access :-)
        {
            return true;
        }
    }
}
else
{
    for (int i = 0; i < 0x12345; i++)
    {
        if (array[i] == 42) // bounds checks emitted for this access :-(
        {
            return true;
        }
    }
}
return false;
```

这样,以牺牲一些代码重复为代价,我们得到了没有边界检查的快速循环,并且只支付了慢路径上的边界检查.你可以在生成的程序集中看到这个(如果你还不知道,DOTNET_JitDisasm是我在.Net 7中最喜欢的特性之一):

```assembly
; Assembly listing for method Program:Test(ref):bool
; Emitting BLENDED_CODE for X64 CPU with AVX - Windows
; Tier-1 compilation
; optimized code
; rsp based frame
; fully interruptible
; No PGO data

G_M000_IG01:                ;; offset=0000H
       4883EC28             sub      rsp, 40

G_M000_IG02:                ;; offset=0004H
       33C0                 xor      eax, eax
       4885C9               test     rcx, rcx
       7429                 je       SHORT G_M000_IG05
       81790845230100       cmp      dword ptr [rcx+08H], 0x12345
       7C20                 jl       SHORT G_M000_IG05
       0F1F40000F1F840000000000 align    [12 bytes for IG03]

G_M000_IG03:                ;; offset=0020H
       8BD0                 mov      edx, eax
       837C91102A           cmp      dword ptr [rcx+4*rdx+10H], 42
       7429                 je       SHORT G_M000_IG08
       FFC0                 inc      eax
       3D45230100           cmp      eax, 0x12345
       7CEE                 jl       SHORT G_M000_IG03

G_M000_IG04:                ;; offset=0032H
       EB17                 jmp      SHORT G_M000_IG06

G_M000_IG05:                ;; offset=0034H
       3B4108               cmp      eax, dword ptr [rcx+08H]
       7323                 jae      SHORT G_M000_IG10
       8BD0                 mov      edx, eax
       837C91102A           cmp      dword ptr [rcx+4*rdx+10H], 42
       7410                 je       SHORT G_M000_IG08
       FFC0                 inc      eax
       3D45230100           cmp      eax, 0x12345
       7CE9                 jl       SHORT G_M000_IG05

G_M000_IG06:                ;; offset=004BH
       33C0                 xor      eax, eax

G_M000_IG07:                ;; offset=004DH
       4883C428             add      rsp, 40
       C3                   ret

G_M000_IG08:                ;; offset=0052H
       B801000000           mov      eax, 1

G_M000_IG09:                ;; offset=0057H
       4883C428             add      rsp, 40
       C3                   ret

G_M000_IG10:                ;; offset=005CH
       E81FA0C15F           call     CORINFO_HELP_RNGCHKFAIL
       CC                   int3

; Total bytes of code 98
```

G_M000_IG02块执行null检查和长度检查,如果失败,则跳转到G_M000_IG05块.如果两者都成功了,它就会执行循环(block G_M000_IG03),不进行边界检查:

```assembly
G_M000_IG03:                ;; offset=0020H
       8BD0                 mov      edx, eax
       837C91102A           cmp      dword ptr [rcx+4*rdx+10H], 42
       7429                 je       SHORT G_M000_IG08
       FFC0                 inc      eax
       3D45230100           cmp      eax, 0x12345
       7CEE                 jl       SHORT G_M000_IG03
```

边界检查只出现在慢路径块中:

```assembly
G_M000_IG05:                ;; offset=0034H
       3B4108               cmp      eax, dword ptr [rcx+08H]
       7323                 jae      SHORT G_M000_IG10
       8BD0                 mov      edx, eax
       837C91102A           cmp      dword ptr [rcx+4*rdx+10H], 42
       7410                 je       SHORT G_M000_IG08
       FFC0                 inc      eax
       3D45230100           cmp      eax, 0x12345
       7CE9                 jl       SHORT G_M000_IG05
```

这是“循环克隆”.那什么是"不变提升”呢? 提升是把某个东西从循环中拉出来放到循环之前,不变量是不变的东西.因此,不变提升是在循环之前从循环中拉出一些东西,以避免在每次循环迭代中重新计算一个不变的答案.实际上,前面的例子已经展示了不变量提升,因为边界检查被移动到循环之前,而不是在循环中,但一个更具体的例子应该是这样的:

```csharp
[MethodImpl(MethodImplOptions.NoInlining)]
private static bool Test(int[] array)
{
    for (int i = 0; i < 0x12345; i++)
    {
        if (array[i] == array.Length - 42)
        {
            return true;
        }
    }

    return false;
}
```

注意数组的值为<font color=red>array.Length - 42</font>在每次循环迭代中不会改变,所以它对循环迭代是“不变的”,可以被取出,生成的代码会这样做:

```assembly
G_M000_IG02:                ;; offset=0004H
       33D2                 xor      edx, edx ;;检查数组是否为null
       4885C9               test     rcx, rcx
       742A                 je       SHORT G_M000_IG05
       448B4108             mov      r8d, dword ptr [rcx+08H]
       4181F845230100       cmp      r8d, 0x12345
       7C1D                 jl       SHORT G_M000_IG05
       4183C0D6             add      r8d, -42   ;;加-42
       0F1F4000             align    [4 bytes for IG03]

G_M000_IG03:                ;; offset=0020H
       8BC2                 mov      eax, edx
       4439448110           cmp      dword ptr [rcx+4*rax+10H], r8d
       7433                 je       SHORT G_M000_IG08
       FFC2                 inc      edx
       81FA45230100         cmp      edx, 0x12345
       7CED                 jl       SHORT G_M000_IG03
```

在这里,我们再次看到检查数组是否为null(<font color=red>test rcx, rcx</font>)和数组的长度被检查(<font color=red>mov r8d, dword ptr [rcx+08H]</font>然后cmp r8d, 0x12345),但然后与数组的长度在<font color=red>r8d</font>,然后我们看到这个前端块减去42从长度(<font color=red>add r8d, -42</font>),这是在我们继续进入快速路径循环在G_M000_IG03块之前.这样就可以将那组额外的操作排除在循环之外,从而避免了每次迭代重新计算值的开销.

那么这如何应用于动态PGO呢?请记住,对于PGO能够做到避免接口/虚拟分发,它通过进行类型检查来查看所使用的类型是否为最常见的类型;如果是,则使用直接调用该类型的方法的快速路径(在这样做时,该调用可能会内联),如果不是,则返回到正常的接口/虚拟分发.该检查对循环是不变的.因此,当一个方法被分层并启用PGO时,类型检查现在可以从循环中升起,这使得处理常见情况的成本更低.考虑一下我们原来例子的变化:

```csharp
using System.Runtime.CompilerServices;

class Program
{
    static void Main()
    {
        IPrinter printer = new BlankPrinter();
        while (true)
        {
            DoWork(printer);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void DoWork(IPrinter printer)
    {
        for (int j = 0; j < 123; j++)
        {
            printer.Print(j);
        }
    }

    interface IPrinter
    {
        void Print(int i);
    }

    class BlankPrinter : IPrinter
    {
        public void Print(int i)
        {
            Console.Write("");
        }
    }
}
```

当我们查看在启用动态PGO的情况下为其生成优化的汇编代码,我们看到:

```assembly
; Assembly listing for method Program:DoWork(IPrinter)
; Emitting BLENDED_CODE for X64 CPU with AVX - Windows
; Tier-1 compilation
; optimized code
; optimized using profile data
; rsp based frame
; partially interruptible
; with Dynamic PGO: edge weights are invalid, and fgCalledCount is 12187
; 0 inlinees with PGO data; 1 single block inlinees; 0 inlinees without PGO data

G_M000_IG01:                ;; offset=0000H
       57                   push     rdi
       56                   push     rsi
       4883EC28             sub      rsp, 40
       488BF1               mov      rsi, rcx

G_M000_IG02:                ;; offset=0009H
       33FF                 xor      edi, edi
       4885F6               test     rsi, rsi
       742B                 je       SHORT G_M000_IG05
       48B9982DD43CFC7F0000 mov      rcx, 0x7FFC3CD42D98
       48390E               cmp      qword ptr [rsi], rcx
       751C                 jne      SHORT G_M000_IG05

G_M000_IG03:                ;; offset=001FH
       48B9282040F948020000 mov      rcx, 0x248F9402028
       488B09               mov      rcx, gword ptr [rcx]
       FF1526A80D00         call     [Console:Write(String)]
       FFC7                 inc      edi
       83FF7B               cmp      edi, 123
       7CE6                 jl       SHORT G_M000_IG03

G_M000_IG04:                ;; offset=0039H
       EB29                 jmp      SHORT G_M000_IG07

G_M000_IG05:                ;; offset=003BH
       48B9982DD43CFC7F0000 mov      rcx, 0x7FFC3CD42D98
       48390E               cmp      qword ptr [rsi], rcx
       7521                 jne      SHORT G_M000_IG08
       48B9282040F948020000 mov      rcx, 0x248F9402028
       488B09               mov      rcx, gword ptr [rcx]
       FF15FBA70D00         call     [Console:Write(String)]

G_M000_IG06:                ;; offset=005DH
       FFC7                 inc      edi
       83FF7B               cmp      edi, 123
       7CD7                 jl       SHORT G_M000_IG05

G_M000_IG07:                ;; offset=0064H
       4883C428             add      rsp, 40
       5E                   pop      rsi
       5F                   pop      rdi
       C3                   ret

G_M000_IG08:                ;; offset=006BH
       488BCE               mov      rcx, rsi
       8BD7                 mov      edx, edi
       49BB1000AA3CFC7F0000 mov      r11, 0x7FFC3CAA0010
       41FF13               call     [r11]IPrinter:Print(int):this
       EBDE                 jmp      SHORT G_M000_IG06

; Total bytes of code 127
```

我们可以在G_M000_IG02块中看到,它正在对IPrinter实例进行类型检查,如果检查失败就跳转到G_M000_IG05(<font color=red>mov rcx, 0x7FFC3CD42D98</font>然后(<font color=red>cmp qword ptr [rsi], rcx</font>然后<font color=red>jne SHORT G_M000_IG05</font>),否则就跳转到G_M000_IG03,这是一个紧密的快速路径循环,内联<font color=red>BlankPrinter.Print</font>没有看到类型检查!

有趣的是,这样的改进也会带来挑战.PGO导致类型检查数量的显著增加,因为专门化给定类型的调用站点需要与该类型进行比较.然而,常见的子表达式消除(CSE)在历史上并不适用于这种类型句柄(CSE是一种编译器优化,通过一次计算结果然后存储它以供后续使用,而不是每次重新计算它,从而消除重复表达式).dotnet/runtime#70580通过为这些常量句柄启用CSE修复了这个问题.例如,考虑以下方法:

```csharp
[Benchmark]
[Arguments("", "", "", "")]
public bool AllAreStrings(object o1, object o2, object o3, object o4) =>
    o1 is string && o2 is string && o3 is string && o4 is string;
```

在.Net 6中JIT生成了以下汇编代码:

```assembly
; Program.AllAreStrings(System.Object, System.Object, System.Object, System.Object)
       test      rdx,rdx
       je        short M00_L01
       mov       rax,offset MT_System.String ;;第1次加载
       cmp       [rdx],rax
       jne       short M00_L01
       test      r8,r8
       je        short M00_L01
       mov       rax,offset MT_System.String  ;;第2次加载
       cmp       [r8],rax
       jne       short M00_L01
       test      r9,r9
       je        short M00_L01
       mov       rax,offset MT_System.String  ;;第3次加载
       cmp       [r9],rax
       jne       short M00_L01
       mov       rax,[rsp+28]
       test      rax,rax
       je        short M00_L00
       mov       rdx,offset MT_System.String  ;;第4次加载
       cmp       [rax],rdx
       je        short M00_L00
       xor       eax,eax
M00_L00:
       test      rax,rax
       setne     al
       movzx     eax,al
       ret
M00_L01:
       xor       eax,eax
       ret
; Total bytes of code 100
```

注意,C#有四个string(字符串)test(逻辑与运算,汇编代码有四个加载mov rax,offset MT_System.String.现在在.Net 7中一次加载:

```assembly
; Program.AllAreStrings(System.Object, System.Object, System.Object, System.Object)
       test      rdx,rdx
       je        short M00_L01
       mov       rax,offset MT_System.String  ;;只有1次加载
       cmp       [rdx],rax
       jne       short M00_L01
       test      r8,r8
       je        short M00_L01
       cmp       [r8],rax
       jne       short M00_L01
       test      r9,r9
       je        short M00_L01
       cmp       [r9],rax
       jne       short M00_L01
       mov       rdx,[rsp+28]
       test      rdx,rdx
       je        short M00_L00
       cmp       [rdx],rax
       je        short M00_L00
       xor       edx,edx
M00_L00:
       xor       eax,eax
       test      rdx,rdx
       setne     al
       ret
M00_L01:
       xor       eax,eax
       ret
; Total bytes of code 69
```

#### Bounds Check Elimination(消除边界检查)

.Net最吸引人的地方之一是它的安全性.运行时保护对数组、字符串和跨范围(Span)的访问,这样您就不会意外地破坏内存;如果这样任意读取/写入内存,则会出现异常.当然,这不是魔法;这是由JIT在每次这些数据结构建立索引时插入边界检查来完成的.例如,这个:

```csharp
[MethodImpl(MethodImplOptions.NoInlining)]
static int Read0thElement(int[] array) => array[0];
```

结果为:

```assembly
G_M000_IG01:                ;; offset=0000H
       4883EC28             sub      rsp, 40

G_M000_IG02:                ;; offset=0004H
       83790800             cmp      dword ptr [rcx+08H], 0
       7608                 jbe      SHORT G_M000_IG04
       8B4110               mov      eax, dword ptr [rcx+10H]

G_M000_IG03:                ;; offset=000DH
       4883C428             add      rsp, 40
       C3                   ret

G_M000_IG04:                ;; offset=0012H
       E8E9A0C25F           call     CORINFO_HELP_RNGCHKFAIL
       CC                   int3
```

数组在<font color=red>rcx</font>寄存器中传递给这个方法,指向对象中的方法表指针,数组的长度存储在对象中的方法表指针之后(在64位进程中为8字节).因此<font color=red>cmp dword ptr [rcx+08H]</font>, 0指令读取数组的长度并将长度与0比较;这是有意义的,因为长度不能是负的,我们试图访问第0个元素,所以只要长度不是0,数组就有足够的元素让我们访问它的第0个元素.在长度为0的情况下,代码跳转到函数的末尾,其中包含调用<font color=red> CORINFO_HELP_RNGCHKFAIL</font>;这是一个JIT helper(辅助函数),它会抛出<font color=red>IndexOutOfRangeException</font>.如果长度足够,然而,它然后读取存储在数组数据开始的int,这在64位是16字节(0x10)超过指针(<font color=red> mov eax, dword ptr [rcx+10H]</font>).

虽然这些边界检查本身并不是非常昂贵,但如果它们很多,它们的成本就会增加.因此,虽然JIT需要确保“安全”访问不会超出边界,但它也试图证明某些访问不会超出边界,在这种情况下,它不必发出边界检查,因为它知道这是多余的.在.Net的每一个版本中,都添加了越来越多的案例来消除这些边界检查, .Net 7中也不例外.

例如,来自@anthonycanino的dotnet/runtime#61662使JIT能够理解各种形式的二进制操作,作为范围检查的一部分.考虑这个方法:

```csharp
[MethodImpl(MethodImplOptions.NoInlining)]
private static ushort[]? Convert(ReadOnlySpan<byte> bytes)
{
    if (bytes.Length != 16)
    {
        return null;
    }

    var result = new ushort[8];
    for (int i = 0; i < result.Length; i++)
    {
        result[i] = (ushort)(bytes[i * 2] * 256 + bytes[i * 2 + 1]);
    }

    return result;
}
```

它验证输入span为16字节长,然后创建一个<font color=red>new ushort[8]</font>数组,其中数组中的每个ushort组合两个输入字节.为此,它将遍历输出数组,并使用<font color=red>i *2</font>和<font color=red>i* 2 + 1</font>作为下标来索引字节数组.在.Net 6中,每一个索引操作都会导致边界检查,程序集如下:

```assembly
cmp       r8d,10    
jae       short G_M000_IG04 
movsxd    r8,r8d
```

其中G_M000_IG04是我们现在熟悉的CORINFO_HELP_RNGCHKFAIL.但是在.Net 7中，我们得到了这个方法汇编代码:

```assembly
G_M000_IG01:                ;; offset=0000H
       56                   push     rsi
       4883EC20             sub      rsp, 32

G_M000_IG02:                ;; offset=0005H
       488B31               mov      rsi, bword ptr [rcx]
       8B4908               mov      ecx, dword ptr [rcx+08H]
       83F910               cmp      ecx, 16
       754C                 jne      SHORT G_M000_IG05
       48B9302F542FFC7F0000 mov      rcx, 0x7FFC2F542F30
       BA08000000           mov      edx, 8
       E80C1EB05F           call     CORINFO_HELP_NEWARR_1_VC
       33D2                 xor      edx, edx
                            align    [0 bytes for IG03]

G_M000_IG03:                ;; offset=0026H
       8D0C12               lea      ecx, [rdx+rdx]
       448BC1               mov      r8d, ecx
       FFC1                 inc      ecx
       458BC0               mov      r8d, r8d
       460FB60406           movzx    r8, byte  ptr [rsi+r8]
       41C1E008             shl      r8d, 8
       8BC9                 mov      ecx, ecx
       0FB60C0E             movzx    rcx, byte  ptr [rsi+rcx]
       4103C8               add      ecx, r8d
       0FB7C9               movzx    rcx, cx
       448BC2               mov      r8d, edx
       6642894C4010         mov      word  ptr [rax+2*r8+10H], cx
       FFC2                 inc      edx
       83FA08               cmp      edx, 8
       7CD0                 jl       SHORT G_M000_IG03

G_M000_IG04:                ;; offset=0056H
       4883C420             add      rsp, 32
       5E                   pop      rsi
       C3                   ret

G_M000_IG05:                ;; offset=005CH
       33C0                 xor      rax, rax

G_M000_IG06:                ;; offset=005EH
       4883C420             add      rsp, 32
       5E                   pop      rsi
       C3                   ret

; Total bytes of code 100
```

没有边界检查,这是很容易看到的,因为在方法的最后没有显示<font color=red> call CORINFO_HELP_RNGCHKFAIL</font>.通过这个提交, JIT能够理解某些乘法和移位操作的影响,以及它们对数据结构边界的关系.因为它可以看到结果数组的长度是8,并且循环从0迭代到那个排他的上界,它知道<font color=red>i</font>将始终在<font color=red> [0,7]</font>范围内,这意味着<font color=red> i *2</font>将始终在<font color=red> [0,14]</font>范围内,<font color=red>i* 2 + 1</font>将始终在<font color=red>[0,15]</font>范围内.因此,它能够证明不需要边界检查.

dotnet/runtime#61569和dotnet/runtime#62864也有助于消除边界检查,当处理常量字符串和Span初始化从静态RVA(“相对虚拟地址”静态字段,基本上是模块数据段中的静态字段).例如,考虑以下基准测试:

```csharp
[Benchmark]
[Arguments(1)]
public char GetChar(int i)
{
    const string Text = "hello";
    return (uint)i < Text.Length ? Text[i] : '\0';
}
```

在.Net 6中，我们得到了这样的汇编代码:

```assembly
; Program.GetChar(Int32)
       sub       rsp,28
       mov       eax,edx
       cmp       rax,5
       jl        short M00_L00
       xor       eax,eax
       add       rsp,28
       ret
M00_L00:
       cmp       edx,5
       jae       short M00_L01
       mov       rax,2278B331450
       mov       rax,[rax]
       movsxd    rdx,edx
       movzx     eax,word ptr [rax+rdx*2+0C]
       add       rsp,28
       ret
M00_L01:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 56
```

这一开始是有意义的:JIT显然能够看到<font color=red>Text</font>的长度是5,所以它实现了<font color=red> (uint)i < Text</font>.长度检查通过做<font color=red> cmp rax,5</font>,如果<font color=red> i</font>作为一个无符号值大于或等于5,它然后零'返回值(返回<font color=red> '\0'</font>)和退出.如果长度小于5(在这种情况下,由于无符号比较,它至少是0),然后跳转到M00_L00从字符串中读取值…但我们随后看到另一个<font color=red> cmp</font>针对5,这一次是作为范围检查的一部分.因此,即使JIT知道索引在边界内,它也不能删除边界检查.在.Net 7中,我们得到这样的结果:

```assembly
; Program.GetChar(Int32)
       cmp       edx,5
       jb        short M00_L00
       xor       eax,eax
       ret
M00_L00:
       mov       rax,2B0AF002530
       mov       rax,[rax]
       mov       edx,edx
       movzx     eax,word ptr [rax+rdx*2+0C]
       ret
; Total bytes of code 29
```

生成的汇编代码变得很好.

dotnet/runtime#67141是一个很好的例子,说明了不断发展的系统需求如何推动JIT的特定优化.<font color=red>Regex</font>(正则表达式)编译器和正则表达式源代码生成器通过使用存储在字符串中的位图查找来处理正则表达式字符类的某些情况.例如,为了确定<font color=red> char c</font>是否在字符类“<font color=red>[a-Za-z0-9_]</font>”（将匹配下划线或任何ASCII字母或数字）,实现最终生成一个类似以下方法体的表达式：

```csharp
[Benchmark]
[Arguments('a')]
public bool IsInSet(char c) =>
    c < 128 && ("\0\0\0\u03FF\uFFFE\u87FF\uFFFE\u07FF"[c >> 4] & (1 << (c & 0xF))) != 0;
```

该实现将8个字符的字符串作为128位查找表处理.如果已知字符在范围内(例如它实际上是一个7位值),那么它将使用该值的前3位索引到字符串的8个元素中,并使用后4位选择该元素的16位中的一个,从而告诉我们这个输入字符是否在集合中.在.Net 6中,即使我们知道字符在字符串的范围内,JIT也无法通过长度比较或位移位来识别.

```assembly
; Program.IsInSet(Char)
       sub       rsp,28
       movzx     eax,dx
       cmp       eax,80
       jge       short M00_L00
       mov       edx,eax
       sar       edx,4
       cmp       edx,8
       jae       short M00_L01
       mov       rcx,299835A1518
       mov       rcx,[rcx]
       movsxd    rdx,edx
       movzx     edx,word ptr [rcx+rdx*2+0C]
       and       eax,0F
       bt        edx,eax
       setb      al
       movzx     eax,al
       add       rsp,28
       ret
M00_L00:
       xor       eax,eax
       add       rsp,28
       ret
M00_L01:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 75
```

前面提到的提交负责长度检查.这个提交负责位偏移.所以在.Net 7中，我们得到了这样的代码:

```assembly
; Program.IsInSet(Char)
       movzx     eax,dx
       cmp       eax,80
       jge       short M00_L00
       mov       edx,eax
       sar       edx,4
       mov       rcx,197D4800608
       mov       rcx,[rcx]
       mov       edx,edx
       movzx     edx,word ptr [rcx+rdx*2+0C]
       and       eax,0F
       bt        edx,eax
       setb      al
       movzx     eax,al
       ret
M00_L00:
       xor       eax,eax
       ret
; Total bytes of code 51
```

注意,没有调用CORINFO_HELP_RNGCHKFAIL.正如您可能猜到的,这种检查在Regex中经常发生,这使它成为一个非常有用的添加.

当谈到数组访问时,边界检查是一个明显的开销,但它们不是唯一的.还需要尽可能使用开销较低的指令.在.Net 6中,使用如下方法:

```csharp
[MethodImpl(MethodImplOptions.NoInlining)]
private static int Get(int[] values, int i) => values[i];
```

将生成如下汇编代码:

```assembly
; Program.Get(Int32[], Int32)
       sub       rsp,28
       cmp       edx,[rcx+8]
       jae       short M01_L00
       movsxd    rax,edx
       mov       eax,[rcx+rax*4+10]
       add       rsp,28
       ret
M01_L00:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 27
```

这看起来与我们之前的讨论相当熟悉;JIT加载数组的长度([rcx+8]),并将其与i的值(在edx中)进行比较,然后跳到末尾,如果i超出了边界就抛出异常.在这个跳转之后,我们立即看到一个movsxd rax, edx指令,它从edx中取i的32位值,并将其移动到64位寄存器rax中.作为移动的一部分,它是符号延伸;这就是指令名的“sxd”部分(符号扩展意味着新的64位值的上32位将被设置为32位值的上32位的值,以便数字保留其有符号的值).有趣的是,我们知道数组和张成空间的长度是非负的,因为我们只是对长度进行了i的边界检查,我们也知道i是非负的.这使得符号扩展毫无用处,因为上边的位保证是0.由于mov指令的零扩展比movsxd稍微便宜一点,我们可以简单地使用它来代替.这正是dotnet/runtime#57970 from @pentp为数组和span所做的(dotnet/runtime#70884也类似地在其他情况下避免一些签名类型转换).在.Net 7中,我们得到了这样的结果:

```assembly
; Program.Get(Int32[], Int32)
       sub       rsp,28
       cmp       edx,[rcx+8]
       jae       short M01_L00
       mov       eax,edx
       mov       eax,[rcx+rax*4+10]
       add       rsp,28
       ret
M01_L00:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 26
```

不过,这并不是数组访问开销的唯一来源.事实上,有一个非常大的数组访问开销类别一直存在,但这是众所周知的,甚至有老的FxCop规则和新的Roslyn分析程序对此发出警告: 多维数组访问.在使用多维数组的情况下,开销不仅仅是每个索引操作上的额外分支,或者计算元素位置所需的额外数学运算,而是它们目前基本上未修改地通过JIT的优化阶段.dotnet/runtime#70271通过在JIT管道的早期扩展多维数组访问来改善这里的状态,这样以后的优化阶段就可以像其他代码一样改进多维访问,包括CSE和循环不变提升.通过对多维数组的所有元素进行求和的简单基准测试,就可以看出这一点的影响.

```csharp
private int[,] _square;

[Params(1000)]
public int Size { get; set; }

[GlobalSetup]
public void Setup()
{
    int count = 0;
    _square = new int[Size, Size];
    for (int i = 0; i < Size; i++)
    {
        for (int j = 0; j < Size; j++)
        {
            _square[i, j] = count++;
        }
    }
}

[Benchmark]
public int Sum()
{
    int[,] square = _square;
    int sum = 0;
    for (int i = 0; i < Size; i++)
    {
        for (int j = 0; j < Size; j++)
        {
            sum += square[i, j];
        }
    }
    return sum;
}
```

| Method | Runtime  |   Mean   | Ratio |
| :----: | :------: | :------: | :---: |
|  Sum   | .NET 6.0 | 964.1 us | 1.00  |
|  Sum   | .NET 7.0 | 674.7 us | 0.70  |

前面的示例假设您知道多维数组的每个维度的大小(它直接指循环中的<font color=red>Size</font>大小).显然,情况并非总是如此(甚至很少如此).在这种情况下,您更可能使用<font color=red>Array.GetUpperBound</font>方法,并且由于多维数组可以具有非零的下界,即<font color=red>Array.GetLowerBound</font>.这将导致如下代码:

```csharp
private int[,] _square;

[Params(1000)]
public int Size { get; set; }

[GlobalSetup]
public void Setup()
{
    int count = 0;
    _square = new int[Size, Size];
    for (int i = 0; i < Size; i++)
    {
        for (int j = 0; j < Size; j++)
        {
            _square[i, j] = count++;
        }
    }
}

[Benchmark]
public int Sum()
{
    int[,] square = _square;
    int sum = 0;
    for (int i = square.GetLowerBound(0); i < square.GetUpperBound(0); i++)
    {
        for (int j = square.GetLowerBound(1); j < square.GetUpperBound(1); j++)
        {
            sum += square[i, j];
        }
    }
    return sum;
}
```

在.Net 7中,由于dotnet/runtime#60816,那些GetLowerBound和GetUpperBound调用变成了JIT intrinsic.“intrinsic”由编译器可以替换它认为更好的代码,而不是仅仅依赖于方法定义的实现(如果它甚至有一个).在.Net中有数以千计的方法以这种方式为JIT所知,其中GetLowerBound和GetUpperBound就是最新的两个方法.现在,作为intrinsic,当传递给它们一个常量值(例如0表示第0个rank)时,JIT可以替换必要的程序集指令,直接从存放边界的内存位置读取数据.下面是.Net 6中这个基准测试的汇编代码;这里主要看到的是所有对GetLowerBound和GetUpperBound的调用:

```assembly
; Program.Sum()
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,28
       mov       rsi,[rcx+8]
       xor       edi,edi
       mov       rcx,rsi
       xor       edx,edx
       cmp       [rcx],ecx
       call      System.Array.GetLowerBound(Int32)
       mov       ebx,eax
       mov       rcx,rsi
       xor       edx,edx
       call      System.Array.GetUpperBound(Int32)
       cmp       eax,ebx
       jle       short M00_L03
M00_L00:
       mov       rcx,[rsi]
       mov       ecx,[rcx+4]
       add       ecx,0FFFFFFE8
       shr       ecx,3
       cmp       ecx,1
       jbe       short M00_L05
       lea       rdx,[rsi+10]
       inc       ecx
       movsxd    rcx,ecx
       mov       ebp,[rdx+rcx*4]
       mov       rcx,rsi
       mov       edx,1
       call      System.Array.GetUpperBound(Int32)
       cmp       eax,ebp
       jle       short M00_L02
M00_L01:
       mov       ecx,ebx
       sub       ecx,[rsi+18]
       cmp       ecx,[rsi+10]
       jae       short M00_L04
       mov       edx,ebp
       sub       edx,[rsi+1C]
       cmp       edx,[rsi+14]
       jae       short M00_L04
       mov       eax,[rsi+14]
       imul      rax,rcx
       mov       rcx,rdx
       add       rcx,rax
       add       edi,[rsi+rcx*4+20]
       inc       ebp
       mov       rcx,rsi
       mov       edx,1
       call      System.Array.GetUpperBound(Int32)
       cmp       eax,ebp
       jg        short M00_L01
M00_L02:
       inc       ebx
       mov       rcx,rsi
       xor       edx,edx
       call      System.Array.GetUpperBound(Int32)
       cmp       eax,ebx
       jg        short M00_L00
M00_L03:
       mov       eax,edi
       add       rsp,28
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       ret
M00_L04:
       call      CORINFO_HELP_RNGCHKFAIL
M00_L05:
       mov       rcx,offset MT_System.IndexOutOfRangeException
       call      CORINFO_HELP_NEWSFAST
       mov       rsi,rax
       call      System.SR.get_IndexOutOfRange_ArrayRankIndex()
       mov       rdx,rax
       mov       rcx,rsi
       call      System.IndexOutOfRangeException..ctor(System.String)
       mov       rcx,rsi
       call      CORINFO_HELP_THROW
       int       3
; Total bytes of code 219
```

下面是.Net 7生成的汇编代码:

```assembly
; Program.Sum()
       push      r14
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,20
       mov       rdx,[rcx+8]
       xor       eax,eax
       mov       ecx,[rdx+18]
       mov       r8d,ecx
       mov       r9d,[rdx+10]
       lea       ecx,[rcx+r9+0FFFF]
       cmp       ecx,r8d
       jle       short M00_L03
       mov       r9d,[rdx+1C]
       mov       r10d,[rdx+14]
       lea       r10d,[r9+r10+0FFFF]
M00_L00:
       mov       r11d,r9d
       cmp       r10d,r11d
       jle       short M00_L02
       mov       esi,r8d
       sub       esi,[rdx+18]
       mov       edi,[rdx+10]
M00_L01:
       mov       ebx,esi
       cmp       ebx,edi
       jae       short M00_L04
       mov       ebp,[rdx+14]
       imul      ebx,ebp
       mov       r14d,r11d
       sub       r14d,[rdx+1C]
       cmp       r14d,ebp
       jae       short M00_L04
       add       ebx,r14d
       add       eax,[rdx+rbx*4+20]
       inc       r11d
       cmp       r10d,r11d
       jg        short M00_L01
M00_L02:
       inc       r8d
       cmp       ecx,r8d
       jg        short M00_L00
M00_L03:
       add       rsp,20
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r14
       ret
M00_L04:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 130
```

重要的是,注意没有更多的<font color=red> call</font>(调用) (除了末尾的边界检查异常).例如,不是第一次调用<font color=red> GetUpperBound</font>:

```assembly
call      System.Array.GetUpperBound(Int32)
```

我们得到:

```assembly
mov       r9d,[rdx+1C]
mov       r10d,[rdx+14]
lea       r10d,[r9+r10+0FFFF]
```

最终使代码运行的更快:

| Method | Runtime  |       Mean | Ratio |
| ------ | -------- | ---------: | ----: |
| Sum    | .NET 6.0 | 2,657.5 us |  1.00 |
| Sum    | .NET 7.0 |   676.3 us |  0.25 |

#### Loop Hoisting and Cloning(循环提升与克隆)

我们之前看到了PGO如何与环提升和克隆相互作用,这些优化也看到了其他改进.

从历史上看,JIT对提升的支持仅限于不变量提升到一个级别.考虑一下这个例子:

```csharp
[Benchmark]
public void Compute()
{
    for (int thousands = 0; thousands < 10; thousands++)
    {
        for (int hundreds = 0; hundreds < 10; hundreds++)
        {
            for (int tens = 0; tens < 10; tens++)
            {
                for (int ones = 0; ones < 10; ones++)
                {
                    int n = ComputeNumber(thousands, hundreds, tens, ones);
                    Process(n);
                }
            }
        }
    }
}

static int ComputeNumber(int thousands, int hundreds, int tens, int ones) =>
    (thousands * 1000) +
    (hundreds * 100) +
    (tens * 10) +
    ones;

[MethodImpl(MethodImplOptions.NoInlining)]
static void Process(int n) { }
```

这么一看,你可能会说"什么可以被提升,n的计算需要所有的循环输入,所有的计算都在ComputeNumber中"但是从编译器的角度来看,ComputeNumber函数是可内联的,因此在逻辑上可以作为调用者的一部分,n的计算实际上被分成多个部分,每个部分可以被提升到不同的层次,例如,数十的计算可以提升一层,数百的计算可以提升两层,数千的计算可以提升三层.下面是.NET 6中使用<font color=red>[DisassemblyDiagnoser]</font>输出:

```assembly
; Program.Compute()
       push      r14
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,20
       xor       esi,esi
M00_L00:
       xor       edi,edi
M00_L01:
       xor       ebx,ebx
M00_L02:
       xor       ebp,ebp
       imul      ecx,esi,3E8    ;;将esi的值乘以1000,放到ecx中
       imul      eax,edi,64     ;;将edi的值乘以100,放到eax中
       add       ecx,eax
       lea       eax,[rbx+rbx*4]
       lea       r14d,[rcx+rax*2]
M00_L03:
       lea       ecx,[r14+rbp]
       call      Program.Process(Int32)
       inc       ebp               ;;ebp寄存器的值自增1
       cmp       ebp,0A            ;;判断ebp的值是否小于10,小于10,则调整M00_L03
       jl        short M00_L03
       inc       ebx      
       cmp       ebx,0A
       jl        short M00_L02
       inc       edi
       cmp       edi,0A
       jl        short M00_L01
       inc       esi
       cmp       esi,0A
       jl        short M00_L00
       add       rsp,20
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r14
       ret
; Total bytes of code 84
```

我们可以看到这里发生了一些提升.毕竟,最内部的循环(标记为M00_L03)只有五条指令：递增<font color=red> ebp</font>（此时为“<font color=red> ones</font>”计数器值),如果它仍然小于0xA（10),则跳回到M00_ L03,将<font color=red>r14中</font>的所有添加到“ones”.太好了,所以我们已经将所有不必要的计算从内部循环中移除,只剩下将1的位置添加到数字的其余部分.让我们去一个水平.M00_L02是tens循环的标签.我们在那里看到了什么？麻烦这两条指令<font color=red> imul ecx,esi 3E8</font>和<font color=red> imul eax,edi, 64</font>正在执行**thousands*1000**和 **hundreds*100**操作,突出显示这些本可以进一步提升的操作被卡在了下一个最内部环路中.现在,我们得到了.NET 7的结果,在dotnet/runtime#68061中进行了改进：

```assembly
; Program.Compute()
       push      r15
       push      r14
       push      r12
       push      rdi
       push      rsi
       push      rbp
       push      rbx
       sub       rsp,20
       xor       esi,esi
M00_L00:
       xor       edi,edi
       imul      ebx,esi,3E8    ;;将esi的值乘以1000,放到ecx中
M00_L01:
       xor       ebp,ebp
       imul      r14d,edi,64	;;将edi的值乘以100,放到eax中
       add       r14d,ebx
M00_L02:
       xor       r15d,r15d
       lea       ecx,[rbp+rbp*4]
       lea       r12d,[r14+rcx*2]
M00_L03:
       lea       ecx,[r12+r15]
       call      qword ptr [Program.Process(Int32)]
       inc       r15d
       cmp       r15d,0A
       jl        short M00_L03
       inc       ebp
       cmp       ebp,0A
       jl        short M00_L02
       inc       edi
       cmp       edi,0A
       jl        short M00_L01
       inc       esi
       cmp       esi,0A
       jl        short M00_L00
       add       rsp,20
       pop       rbx
       pop       rbp
       pop       rsi
       pop       rdi
       pop       r12
       pop       r14
       pop       r15
       ret
; Total bytes of code 99
```

注意这些imul指令的位置.有四个标签,每个标签对应一个循环,我们可以看到最外面的循环有<font color=red>imul ebx,esi,3E8</font>(用于千位计算),下一个循环有<font color=red>imul r14d,edi,64</font>(用于百位计算),突出显示这些计算被提升到适当的级别(十位和个位计算仍然在正确的位置).

在克隆方面有了更多的改进.以前,循环克隆只适用于按1从低值迭代到高值的循环.在dotnet/runtime#60148中,与上限值的比较可以是<=而不仅仅是<.使用dotnet/runtime#67930,向下迭代的循环也可以被克隆,递增和递减大于1的循环也可以被克隆.考虑一下这个基准:

```csharp
private int[] _values = Enumerable.Range(0, 1000).ToArray();

[Benchmark]
[Arguments(0, 0, 1000)]
public int LastIndexOf(int arg, int offset, int count)
{
    int[] values = _values;
    for (int i = offset + count - 1; i >= offset; i--)
        if (values[i] == arg)
            return i;
    return 0;
}
```

如果没有循环克隆,JIT不能假设<font color=red> offset(偏移量)</font>到<font color=red> offset+count</font>在范围内,因此对数组的每次访问进行边界检查.使用循环克隆JIT可以生成不带边界检查的循环版本,并且只有在知道所有访问都有效时才使用.这正是.NET 7中发生的情况.以下是我们在.NET 6中得到的结果：

```assembly
; Program.LastIndexOf(Int32, Int32, Int32)
       sub       rsp,28
       mov       rcx,[rcx+8]
       lea       eax,[r8+r9+0FFFF]
       cmp       eax,r8d
       jl        short M00_L01
       mov       r9d,[rcx+8]
       nop       word ptr [rax+rax]
M00_L00:
       cmp       eax,r9d
       jae       short M00_L03
       movsxd    r10,eax
       cmp       [rcx+r10*4+10],edx
       je        short M00_L02
       dec       eax
       cmp       eax,r8d
       jge       short M00_L00
M00_L01:
       xor       eax,eax
       add       rsp,28
       ret
M00_L02:
       add       rsp,28
       ret
M00_L03:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 72
```

注意如何在核心循环中,标签M00_L00,有一个边界检查(<font color=red> cmp eax,r9d</font>和<font color=red> jae short M00_L03</font>,它跳转到一个<font color=red> call CORINFO_HELP_RNGCHKFAIL</font>).下面是我们在.Net 7中得到的结果:

```assembly
; Program.LastIndexOf(Int32, Int32, Int32)
       sub       rsp,28
       mov       rax,[rcx+8]
       lea       ecx,[r8+r9+0FFFF]
       cmp       ecx,r8d
       jl        short M00_L02
       test      rax,rax
       je        short M00_L01
       test      ecx,ecx
       jl        short M00_L01
       test      r8d,r8d
       jl        short M00_L01
       cmp       [rax+8],ecx
       jle       short M00_L01
M00_L00:
       mov       r9d,ecx
       cmp       [rax+r9*4+10],edx
       je        short M00_L03
       dec       ecx
       cmp       ecx,r8d
       jge       short M00_L00
       jmp       short M00_L02
M00_L01:
       cmp       ecx,[rax+8]
       jae       short M00_L04
       mov       r9d,ecx
       cmp       [rax+r9*4+10],edx
       je        short M00_L03
       dec       ecx
       cmp       ecx,r8d
       jge       short M00_L01
M00_L02:
       xor       eax,eax
       add       rsp,28
       ret
M00_L03:
       mov       eax,ecx
       add       rsp,28
       ret
M00_L04:
       call      CORINFO_HELP_RNGCHKFAIL
       int       3
; Total bytes of code 98
```

请注意代码是如何变大的,以及现在如何有两个循环变体:一个在M00_L00,另一个在M00_L01.第二个M00_L01有一个分支指向同一个<font color=red> call CORINFO_HELP_RNGCHKFAIL</font>,但第一个没有,因为只有在证明<font color=red> offset</font>、<font color=red> count</font>和<font color=red> _values.Length</font>,循环才会被使用.长度是这样的,索引将始终在边界内.

其他提交也改进了循环克隆.dotnet/runtime#59886使JIT能够选择不同的形式来发出选择快速或缓慢循环路径的条件,例如是否发出所有的条件,然后分支<font color=red> (if (!)(cond1 & cond2)) goto slowPath)</font>,或者是否单独发出每个条件<font color=red> (if (!cond1) goto slowPath</font>;如果<font color=red> (!cond2) goto slowPath)</font>.dotnet/runtime#66257启用循环克隆踢在循环变量初始化为更多种类的表达式(例如,<font color=red> for (int frommindex = lastIndex - lengthToClear;…))</font>.而dotnet/runtime#70232增加了JIT与执行更广泛操作的主体克隆循环.

#### Folding(折叠), propagation(传播), and substitution(替换)

<font color=red> 常量</font>折叠是一种优化,编译器在编译时计算只涉及常量的表达式的值,而不是在运行时生成代码来计算值.在.Net中有多个级别的常量折叠,其中一些常量折叠由C#编译器执行,另一些常量折叠由JIT编译器执行.例如给定C#代码:

``` csharp
[Benchmark]
public int A() => 3 + (4 * 5);

[Benchmark]
public int B() => A() * 2;
```

C#编译器将为这些方法生成IL代码,如下所示:

```csharp
.method public hidebysig instance int32 A () cil managed 
{
    .maxstack 8
    IL_0000: ldc.i4.s 23  //在编译时,由编译器计算值
    IL_0002: ret
}

.method public hidebysig instance int32 B () cil managed 
{
    .maxstack 8
    IL_0000: ldarg.0
    IL_0001: call instance int32 Program::A()  //调用方法A,可以看到没有常量折叠和常量传播
    IL_0006: ldc.i4.2
    IL_0007: mul
    IL_0008: ret
}
```

您可以看到,C#编译器已经计算出了<font color=red> 3 +(4**5)</font>的值,因为方法A的IL包含了等价的<font color=red> return 23</font>;但是,方法<font color=red> B</font>包含等价的<font color=red> return A()* 2</font>; ,强调C#编译器执行的常量折叠只是在方法内部.下面是JIT生成的结果:

```assembly
; Program.A()
       mov       eax,17    ;;17是十六进制,为十进制的23
       ret
; Total bytes of code 6

; Program.B()
       mov       eax,2E    ;;2E为十六进制,为十进制的46
       ret
; Total bytes of code 6
```

方法A的汇编代码是不是特别有趣,它只是返回相同的值23(十六进制0x17).但是方法B更有趣.JIT内联了从<font color=red> B</font>到<font color=red> A</font>的调用,将<font color=red> A</font>的内容暴露给<font color=red> B</font>,这样JIT就有效地将<font color=red> B</font>的主体看作是等价于<font color=red>  return 23 * 2;</font>此时,JIT可以完成自己的常量折叠,并将B的主体转换为简单地返回46(十六进制0x2e).常量传播与常量折叠有着错综复杂的联系,本质上就是可以将常量值(通常是通过常量折叠计算的值)替换为进一步的表达式,此时它们也可以被折叠.

JIT长期以来一直在执行常量折叠,但它在.NET7中得到了进一步改进.常量折叠可以改进的方法之一是公开更多要折叠的值,这意味着更多的内联.dotnet/runtime#55745帮助内联线理解,像<font color=red>M(constant + constant) </font>(注意这些常量可能是其他方法调用的结果)这样的方法调用本身就是将常量传递给<font color=red>M</font>,而传递给方法调用的常量是对内联线的提示,它应该考虑更积极地内联,因为将该常量公开给被调用方的主体可能会显著减少实现被调用方所需的代码量.JIT之前可能已经内联了这样一个方法,但当涉及内联时,JIT都是关于启发式和生成足够的证据来证明内联是值得的;这有助于证明这一点.例如,该模式显示在TimeSpan上的各种<font color=red>FromXx</font>方法中.例如<font color=red>TimeSpan.FromSeconds</font>实现为：

```csharp
// TicksPerSecond is a constant
public static TimeSpan FromSeconds(double value) => Interval(value, TicksPerSecond); 
```

并且,为了本例的目的,避免参数验证,<font color=red> Interval</font>为:

```csharp
private static TimeSpan Interval(double value, double scale) => 
    IntervalFromDoubleTicks(value * scale);
private static TimeSpan IntervalFromDoubleTicks(double ticks) => 
    ticks == long.MaxValue ? TimeSpan.MaxValue : new TimeSpan((long)ticks);
```

如果所有内容都内联,则<font color=red>FromSeconds</font>本质上是:

```csharp
public static TimeSpan FromSeconds(double value)
{
    double ticks = value * 10_000_000;
    return ticks == long.MaxValue ? TimeSpan.MaxValue : new TimeSpan((long)ticks);
}
```

如果<font color=red>value</font>是一个常量,比如5,那么这里就可以被折叠成常量(在<font color=red>ticks == long.MaxValue</font>分支上消除死代码)简单地:

```csharp
return new TimeSpan(50_000_000);
```

为此,我将省去.Net 6生成汇编代码,但在.Net 7中,有这样一个基准测试:

```csharp
[Benchmark]
public TimeSpan FromSeconds() => TimeSpan.FromSeconds(5);
```

我们现在得到的是简单明了的汇编代码:

``` assembly
; Program.FromSeconds()
       mov       eax,2FAF080  ;;2FAF080为5*1000*1000
       ret
; Total bytes of code 6
```

另一个改进常量折叠的更改包括@SingleAccretion的dotnet/runtime#57726,它在特定的场景中消除了常量折叠,有时在对从方法调用返回的结构进行逐字段赋值时显示.作为一个小例子,考虑这个访问<font color=red>Color.DarkOrange</font>属性,它会产生<font color=red>new Color(KnownColor.DarkOrange)</font>:

```csharp
[Benchmark]
public Color DarkOrange() => Color.DarkOrange;
```

在.Net 6中，JIT生成如下代码:

```assembly
; Program.DarkOrange()
       mov       eax,1
       mov       ecx,39
       xor       r8d,r8d
       mov       [rdx],r8
       mov       [rdx+8],r8
       mov       [rdx+10],cx
       mov       [rdx+12],ax
       mov       rax,rdx
       ret
; Total bytes of code 32
```

有趣的是,有些常量(39是<font color=red>KnownColor.DarkOrange</font>常量值，1是私有<font color=red>StateKnownColorValid</font>常量值)被加载到寄存器<font color=red>(mov eax，1</font>，然后<font color=red>mov ecx，39</font>)中,然后被存储到返回的<font color=red>Color</font>结构的相关位置(<font color=red>mov[rdx+12]，ax</font>和<font color=red>mov[rdx+10]，cx</font>). 在.NET 7中,它现在生成:

```assembly
; Program.DarkOrange()
       xor       eax,eax
       mov       [rdx],rax
       mov       [rdx+8],rax
       mov       word ptr [rdx+10],39
       mov       word ptr [rdx+12],1
       mov       rax,rdx
       ret
; Total bytes of code 25
```

直接将这些常量值赋值到它们的目标位置(<font color=red>mov word ptr [rdx+12]，1</font>和<font color=red>mov word ptr [rdx+10]，39</font>).其他变化贡献常量折叠包括dotnet/runtime#58171从@SingleAccretion和dotnet/runtime#57605从@SingleAccretion .

然而,一个很大的改进类别来自与传播相关的优化,即前向替换.考虑一下这个不太好的基准测试:

```csharp
[Benchmark]
public int Compute1() => Value + Value + Value + Value + Value;

[Benchmark]
public int Compute2() => SomethingElse() + Value + Value + Value + Value + Value;

private static int Value => 16;

[MethodImpl(MethodImplOptions.NoInlining)]
private static int SomethingElse() => 42;
```

如果我们看一下在.Net 6上为Compute1生成的汇编代码,它看起来就像我们所希望的那样。我们把Valuae相加了5次, Value被简单地内联并返回一个常量16,所以我们希望为Compute1生成的汇编代码能够有效地返回值80(十六进制0x50),这正是所发生的:

```assembly
; Program.Compute1()
       mov       eax,50  ;;内联后为80(16进制是0x50)
       ret
; Total bytes of code 6
```

但是Compute2生成汇编代码有点不同.代码的结构是这样的,对SomethingElse的额外调用最终会略微干扰JIT的分析,而.Net 6最终会得到这样的汇编代码:

```assembly
; Program.Compute2()
       sub       rsp,28
       call      Program.SomethingElse()
       add       eax,10		;;10为16进制16的值
       add       eax,10
       add       eax,10
       add       eax,10
       add       eax,10
       add       rsp,28
       ret
; Total bytes of code 29
```

而不是单个<font color=red>mov eax,50</font>将值0x50放入返回寄存器,分别为5个单独的<font color=red>add eax, 10</font>生成最终结果0x50(80)值.这个相加的过程是不理想.

事实证明,JIT的许多优化操作的是作为解析IL的一部分创建的树数据结构.在某些情况下,当它们所操作的树更大,包含更多要分析的内容时,优化可以做得更好.但是,各种操作可以将这些树分解为更小的、单独的树,例如使用作为内联一部分创建的临时变量,这样做可以抑制这些操作.为了有效地将这些树组合一起,需要一些东西,那就是正向替换.你可以把正向替换想象成逆向的CSE(公共表达式消除);与尝试查找重复表达式并通过一次计算值并将其存储到临时值中来消除它们不同,正向替换消除了临时值,并有效地将表达式树移动到它的使用站点.显然,如果这样做会否定CSE并导致重复的工作,您就不希望这样做,但是对于只定义一次并使用一次的表达式,这种向前传播是有价值的.

dotnet /runtime#61023添加了一个初始的有限版本的前向替换,然后dotnet /runtime#63720添加了一个更健壮的通用实现.随后,dotnet/runtime#70587对其进行了扩展,使其也涵盖了一些SIMD向量,然后dotnet/runtime#71161对其进行了进一步改进,以支持替换到更多的位置(在本例中为调用实参).有了这些,我们愚蠢的基准测试现在在.Net 7上生成了以下代码:

```assembly
; Program.Compute2()
       sub       rsp,28
       call      qword ptr [7FFCB8DAF9A8]
       add       eax,50		;;在.Net 6生成汇编代码,需要5次add相加操作,这里直接用5次相加的值
       add       rsp,28
       ret
; Total bytes of code 18
```

#### Vectorization(向量)

SIMD(即单指令多数据),是一种一条指令同时应用于多条数据的处理方式.你有一个数字列表,你想找到一个特定值的索引?您可以遍历列表,一次比较一个元素,这在功能上是很好的.但是,如果在读取和比较一个元素所需的相同时间内,您可以读取和比较两个元素、四个元素或32个元素呢?这就是SIMD,使用SIMD指令称为“向量化”,其中操作同时应用于一个“向量”中的所有元素.

.NET长期以来一直支持Vector<T>形式的向量,这是一种易于使用的类型,JIT可以很好的支持,使开发人员能够编写向量的实现.Vector<T>最大的优点之一也是最大的缺点之一.该类型旨在适应硬件中可用的任何宽度向量指令.如果机器支持256位宽度向量,那很好,这就是Vector<T>的目标.如果不是,如果机器支持128位宽度向量,那就很好,这就是Vector<T>的目标.但这种灵活性有各种各样的缺点,至少在今天是这样；例如,您可以对Vector<T>执行的操作最终需要与所用向量的宽度无关,因为宽度是根据代码实际运行的硬件而变化的.这意味着可以在Vector<T>上公开的操作是有限的,这反过来又限制了可以用它向量的操作种类.此外,因为在给定的进程中,它只有一个大小,所以一些介于128位到256位之间的数据集大小可能不会像您希望的那样得到处理.您编写了基于Vector<byte>的算法,并在支持256位向量的机器上运行它,这意味着它一次可以处理32个字节,但您可以向它输入31个字节.如果Vector<T>映射到128位向量,它本来可以用于改进该输入的处理,但由于其向量大小大于输入数据大小,实现最终会回落到未加速的向量.还有一些与R2R和Native AOT相关的问题,因为提前编译需要提前知道Vector<T>操作应该使用哪些指令.您在前面讨论DOTNET_JitDisasmSummary；的输出时已经看到了这一点；我们看到,NarrowUtf16ToAscii方法是在“hello,world”控制台应用程序中JIT编译的少数方法之一,这是因为它使用Vector<T>而缺少R2R代码.

从.NET Core 3.0开始, .NET获得了数以千计的新“硬件指令”方法,其中大多数是映射到这些SIMD指令之一的.NET API.这些内部函数使专家能够编写针对特定指令集的实现,如果做得好,则可以获得最佳性能,但它还要求开发人员了解每个指令集,并为可能相关的每个指令集实现其算法,例如,支持AVX2实现的话,支持SSE2实现的时候,或ArmBase实现(如果支持),依此类推. 在.Net 8中Vector开始支持AVX512指令.
