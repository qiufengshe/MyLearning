##1. CoreCLR初始化
	在coreclr_initialize进行初始化
##2. CoreCLR加载.Net程序
	通过coreclr_execute_func,加载.Net程序并执行
##3. CoreCLR关闭,卸载应用程序域,停止运行
	在执行.Net程序完成后,通过coreclr_shutdown调用,先卸载应用程序域

## GC代数分类
GC代数划分: soh(small object heap小对象堆,是0代/1代/2代)/loh(large object heap,大对象堆)及poh(pin object heap,固定对象堆),在内存是划分5块

global_region_allocator.init 对soh/loh/poh进行初始化,初始从poh(为4)开始,loh(为3),soh(2->0代) ,内存地址是从高到低


## CoreCLR加载System中类型
在AppDomain中LoadBaseSystemClasses


CoreCLR在执行c#的Main函数,按需加载,比如在执行Console.WriteLine()方法时,才会加载System.Console.dll