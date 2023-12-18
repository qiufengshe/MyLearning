;hello.asm

extern printf
section .data 
	msg	db "hello, assembly!",0
	fmtstr  db "%s",10,0
	
	pi      dq 3.14	
	fmtflt  db "%lf",10,0
section .bss
section .text
	global main

main:
	push  rbp
	mov   rbp,rsp
	
	mov   rax,0
	mov   rsi,msg
	mov   rdi,fmtstr
	call  printf

; 打印pi 浮点数
	mov  rax,1
	movq xmm0,[pi]
	mov  rdi,fmtflt
	call printf	

	mov   rsp,rbp
	pop   rbp

	ret
