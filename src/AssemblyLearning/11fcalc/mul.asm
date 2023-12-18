; mul.asm

extern printf
section .data
	num1 	dq 10
	sum     dq 0
	fmt     db "%ld * %ld = %ld",10,0


section .bss
section .text
	global main
main:
	push   rbp
	mov    rbp,rsp
	
	mov    rax,[num1]
	imul    rax,[num1]
	mov    [sum],rax
	
	mov    rdi,fmt
	mov    rsi,[num1]
	mov    rdx,[num1]
	mov    rcx,[sum]
	mov    rax,0
	call   printf	

	leave
	ret
