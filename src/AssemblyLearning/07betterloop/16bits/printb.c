// printb.c
//
#include <stdio.h>

void printb(long long n)
{
	long long s,c;
	for(c=63;c>=0;c--)
	{
		s = n>>c;
		//每8位(bit)后的空间
		if((c+1)%8 == 0)
		{
			printf(" ");
		}
		if(s & 1)
		{
			printf("1");
		}
		else
		{
			printf("0");
		}
	}
	printf("\n");
}
