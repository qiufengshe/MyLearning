; aligned2.asm


extern printf

section .data
	fmt	db "2 time pi equals %.14f",10,0
	pi 	dq 3.1314159265358979

section .bss
section .text
	global main
main:
	push	rbp
	mov 	rbp,rsp

	movsd	xmm0,[pi]
	addsd	xmm0,[pi]
	mov 	rdi,fmt
	mov	rax,1
	call	printf

	leave
	ret
