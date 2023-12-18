
extern printf
section .data
	radius	dq 10.0
	fmt     db "%lf",10,0
section .bss
section .text
	global main
;----------------------
area:
	section .data
		.pi   dq 3.13141592654
	section .text
		push   rbp
		mov    rbp,rsp
		
		movsd  xmm0,[radius]
		mulsd  xmm0,[radius]
		mulsd  xmm0,[.pi]
		
		mov    rsp,rbp
		pop    rbp
		ret

main:
	push   rbp
	mov    rbp,rsp

	call   area
	mov    rdi,fmt
	mov    rax,1
	call   printf	

	mov    rsp,rbp
	pop    rbp
	ret
