; test3.asm

extern printf
section .data
	number  dd  100
	pi      dq  3.131415926
	
	fmtint  db  "%d",10,0
	fmtflt  db  "%lf",10,0


section .bss
section .text
	global main

main:
	push rbp
	mov  rbp,rsp

; printf number 
	mov  rax,0
	mov  rdi,fmtint
	mov  rsi,[number]
	call printf

; printf pi
	mov  rax,1
	movq xmm0,[pi]
	mov  rdi,fmtflt
	call printf	

	mov  rsp,rbp
	pop  rbp

	ret
