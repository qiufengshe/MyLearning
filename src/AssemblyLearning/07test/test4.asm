;test4.asm

extern printf

section .data
	pi   dq  3.14
	fmt  db   "pi=%lf",10,0

section .bss
section .text
	global main
main:
	push  rbp
	mov   rbp,rsp
	
; printf pi 
	mov   rax,1
	movq  xmm0,[pi]
	mov   rdi,fmt
	call  printf

; 退出栈
	mov   rsp,rbp
	pop   rbp

	ret
