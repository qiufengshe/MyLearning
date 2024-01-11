; 2023 -> 2024

extern	printf

section .data
	msg	db  "Hello 2024!",10,0
section .bss
section .text
	global  main
main:
	push	rbp
	mov	rbp,rsp

	mov	rdi,msg
	call	printf	

	mov	rsp,rbp
	pop	rbp
	ret
