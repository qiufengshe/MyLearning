#include <stdio.h>
long context[3];
	
__attribute__((naked,returns_twice))
int my_setjmp(void* context)
{
	asm("mov %%rbp,(%%rdi);"
	    "mov %%rsp,8(%%rdi);"
	    "mov (%%rsp),%%rax;"
	    "mov %%rax,16(%%rdi);"
	    "mov $0,%%rax;"
	    "ret;"
	    :::);
}

__attribute__((naked,noreturn))
void my_longjmp(void* context,int val)
{
	asm("mov (%%rdi), %%rbp;"
	    "mov 8(%%rdi),%%rsp;"
	    "mov %%rsi,%%rax;"
	    "jmp 16(%%rdi);"
	    :::);
}

void func2()
{
	printf("func2 start\n");
	my_longjmp(context,100);
	printf("func2 end\n");
}

void func1()
{
	printf("func1 start\n");
	func2();
	printf("func2 end\n");
}

int main(int argc,char* argv[])
{
	printf("main start\n");
	int val = my_setjmp(context);
	if(val == 0)
	{
		func1();
	}
	return val;
}
