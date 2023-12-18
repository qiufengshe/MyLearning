; function.asm

extern printf
section .data
	radius	dq 10.0
	pi 	dq 3.14
	fmt     db "The area of the circle is %.2f",10,0
section .bss
section .text
	global main
main:
	push   rbp
	mov    rbp,rsp
	;调用area
	call   area
	mov    rdi,fmt
	movsd  xmm1,[radius]
	mov    rax,1
	call   printf

	;leave 相当于 mov rsp,rbp  和 pop rbp 两条指令
	mov    rsp,rbp
	pop    rbp
	ret

area:
	push   rbp
	mov    rbp,rsp

	movsd  xmm0,[radius]
	mulsd  xmm0,[radius]
	mulsd  xmm0,[pi]
	leave
	ret
