; 100 sum

extern printf
section .data
	num 	dq 100
	sum     dq 0
	fmt     db "1 to 100 sum=%d",10,0
section .bss
section .text
	global main

main:
	push    rbp
	mov     rbp,rsp

	xor 	rax,rax
	mov	rcx,[num]
bloop:
	add	rax,rcx
	loop    bloop
	
	mov     [sum],rax
	mov     rdi,fmt
	mov     rsi,[sum]
	mov     rax,0
	call    printf

	mov     rsp,rbp
	pop	rbp
	ret
