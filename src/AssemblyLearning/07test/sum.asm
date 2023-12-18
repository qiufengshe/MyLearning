;sum.asm

extern printf
section .data
	num   dq  10
	fmt   db  "num 0 to %d sum=%d",10,0

section .bss
section .text
	global main
main:
	push  rbp
	mov   rbp,rsp

;
	mov   rcx,[num]
	mov   rax,0
bloop:
	add   rax,rcx
	loop  bloop  ;每次执行rcx,自动递减1
	
	mov   rdi,fmt
	mov   rsi,[num]
	mov   rdx,rax
	mov   rax,0
	call  printf

	mov   rsp,rbp
	pop   rbp

	ret
