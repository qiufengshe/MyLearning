#include <stdio.h>
long context[3];

void func2()
{
	printf("func2 start\n");
	my_longjmp(context,100);
	printf("func2 end\n");
}

void func()
{
	printf("func1 start\n");
	func2();
	printf("func2 end\n");
}

int main()
{
	printf("main start\n");
	int val = my_setjmp(context);
	if(val == 0)
	{
		func1();
	}
	printf("main end\n");
	return val;
}

