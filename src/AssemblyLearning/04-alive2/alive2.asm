; alive2.asm

extern  printf
section .data 
	msg1 db "Hello, World!",0
	msg2 db "Alive and Kicking!",0
	radius dd 357
	pi     dq  3.14
	fmtstr  db "%s",10,0
	fmtflt  db "%lf",10,0
	fmtint  db "%d",10,0

section .bss
section .text
	global main

main:
	push rbp
	mov  rbp,rsp
	
; printf msg1
	mov rax,0
	mov rdi,fmtstr
	mov rsi,msg1
	call printf

; print msg2
	mov rax,0
	mov rdi,fmtstr
	mov rsi,msg2
	call printf

; printf radius 
	mov rax,0
	mov rdi,fmtint
	mov rsi,[radius]
	call printf
; printf pi
	mov rax,1
	movq xmm0,[pi]   ;使用一个xmm寄存器
	mov rdi,fmtflt
	call printf
; 
	mov rsp,rbp
	pop rbp
	
ret

