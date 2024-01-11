; sse integer

extern printf
section .data
	dummy	db	13
align	16
	pdivector1	dd 1
		        dd 2
			dd 3
			dd 4
	pdivector2	dd 5
			dd 6
			dd 7
			dd 8
	fmt1		db "Packed Integer Vector 1:%d %d %d %d",10,0
	fmt2		db "Packed Integer Vector 2:%d %d %d %d",10,0
	fmt3		db "Sum Vector:%d %d %d %d",10,0
	fmt4		db "Reverse of Sum Vector:%d %d %d %d ",10,0
section .bss
alignb	16
	pdivector_res	resd	4
	pdivector_other resd 4
section .text
	global main
main:
	push	rbp
	mov	rbp,rsp

	;打印向量1
	mov	rsi,pdivector1
	mov	rdi,fmt1
	call	printpdi

	mov	rsp,rbp
	pop	rbp
	ret



;-------------------------------------
printpdi:
	push	rbp
	mov	rbp,rsp
	
	movdqa	xmm0,[rsi]
	;从xmmo0中提取打包的值
	pextrd	esi,xmm0,0
	pextrd	edx,xmm0,1
	pextrd	ecx,xmm0,2
	pextrd	r8d,xmm0,3

	;没有浮点数
	mov	rax,0
	call	printf	
	mov	rsp,rbp
	pop	rbp
	ret
