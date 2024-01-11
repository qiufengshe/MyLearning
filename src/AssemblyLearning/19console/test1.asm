; test1.asm
; console text

extern printf
section .data
	msg1	db	"Hello, World!",10,0
	msg1len equ	$-msg1

section .bss
section .text
	global main
main:
	push	rbp
	mov	rbp,rsp
	
	mov	rdi,msg1
	mov	rsi,msg1len
	;call	prints
	call	printf

	mov	rsp,rbp
	pop	rbp
	ret

prints:
	push	rbp
	mov	rbp,rsp

	mov	rax,1
	mov     rdi,1

	mov	rbp,rsp
	pop	rbp
	ret
