; circle.asm

extern pi
section .data
section .bss
section .text
global	c_area
global  c_circum
c_area:
	section .text
	push	rbp
	mov 	rbp,rsp

	movsd	xmm1,qword [pi]
	mulsd	xmm0,xmm0	;半径放在xmm0寄存器中
	mulsd	xmm0,xmm1

	mov	rsp,rbp
	pop 	rbp
	ret
c_circum:
	section .text
	push	rbp
	mov 	rbp,rsp
	
	movsd	xmm1,qword [pi]
	addsd	xmm0,xmm0	;半径放在xmm0寄存器中
	mulsd	xmm0,xmm1

	mov 	rsp,rbp
	pop	rbp
	ret
