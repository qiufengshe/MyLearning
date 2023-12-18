; test.asm

extern printf 
section .data 
	number1   dd  10,0
	pi        dq  3.14,0
	fmtint    db  "number1=%d",10,0
	fmtpi	  db  "pi=%lf",10,0	

section .bss
section .text
	global main

main:
	push rbp
	mov  rbp,rsp
	
;printf number1
	mov  rax,0
	mov  rsi,[number1]
	mov  rdi,fmtint
	call printf

;printf pi
	mov  rax,1
	movq xmm0,[pi]
	mov  rdi,fmtpi
	call printf

	mov  rsp,rbp
	pop  rbp

	ret
