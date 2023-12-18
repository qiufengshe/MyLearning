; add.asm
extern  printf
section .data
	num1 	dq 10
	num2    dq 20
	sum     dq 0
	fmt 	db "%d+%d=%d",10,0

section .bss
section .text
	global main
main:
	push   rbp
	mov    rbp,rsp
	;
	mov    rax,[num1]
	add    rax,[num2]
	mov    [sum],rax
	
	;
	mov    rdi,fmt
	mov    rsi,[num1]
	mov    rdx,[num2]
	mov    rcx,[sum]
	mov    rax,0
	call   printf	

	mov    rsp,rbp
	pop    rbp
	ret
