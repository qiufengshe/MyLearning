;sub.asm

extern  printf

section .data
	num1    dq  100.0
	num2    dq  72.9
	fmt     db  "%lf - %lf = %lf",10,0


section .bss
section .text
	global main
main:
	push   rbp
	mov    rbp,rsp

	movsd  xmm2,[num1]
	subsd  xmm2,[num2]

	mov    rdi,fmt
	movsd  xmm0,[num1]
	movsd  xmm1,[num2]
	mov    rax,3
	call   printf

	leave
	ret
