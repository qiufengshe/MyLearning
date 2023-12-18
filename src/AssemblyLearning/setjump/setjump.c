#include <stdio.h>
#include <setjmp.h>

jmp_buf context;

void func2()
{
	printf(" func2 start\n");
	longjmp(context,100);
	printf(" func2 end\n");
}

void func1()
{
	printf(" func1 start\n");
	func2();
	printf(" func2 end\n");
}

int main(int argc,char* argv[])
{
	printf(" main start\n");
	int val = setjmp(context);
	if(val == 0)
	{
		func1();
	}

	printf(" main end\n");
	return val;
}
