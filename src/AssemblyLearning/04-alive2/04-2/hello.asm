; hello.asm

extern printf

section .data
	msg db "hello,world!",10,0
	fmtstr db "%s",10,0

section .bss
section .text
	global main
main:
	push rbp
	mov  rbp,rsp

	mov  rax,0
	mov  rsi,fmtstr
	mov  rdi,msg
	call printf


	mov  rsp,rbp
	pop  rbp

	ret
