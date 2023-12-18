; test5.asm 
;printf int val

extern printf
section .data
	number   dd  10
	fmt      db  "number=%d",10,0

section .bss
section .text
	global main
main:
	push  rbp
	mov   rbp,rsp
;printf number
	mov   rax,0
	mov   rdi,fmt
	mov   rsi,[number]
	call  printf	

	mov   rsp,rbp
	pop   rbp
	
	ret

