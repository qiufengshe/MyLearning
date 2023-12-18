; aligned3.asm


extern printf
section .data
	fmt 	db "2 times value is %.14lf",10,0
	pi	dq 3.14159265458976

section .bss
section .text
	global main
main:
	push	rbp
	mov 	rbp,rsp

	movsd	xmm0,[pi]
	addsd 	xmm0,[pi]

	mov	rdi,fmt
	mov	rax,1
	call	printf


	mov	rsp,rbp
	pop	rbp
	ret
