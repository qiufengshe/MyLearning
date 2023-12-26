; bits2.asm

extern printf
section .data
	msgn1	db "Number 1 is = %d",0
	msgn2	db "Number 2 is = %d",0
	msg1	db "SHL 2 = OK multiply by 4",0
	msg2	db "SHR	2 = WRONG divide by 4",0
	msg3	db "SAL 2 = correctly divide by 4",0
	msg4	db "SAR 2 = correctly divide by 4",0
	msg5	db "SHR 2 = OK divide by 4",0

	number1 dq 8
	number2 dq -8
	result	dq 0

section	.bss
section .text
	global main
main:
	push	rbp
	mov	rbp,rsp

;SHL ----------------------------------------------
;正数
	mov	rsi,msg1
	call	printmsg	;打印字符
	mov	rsi,[number1]
	call	printbr		;打印数字
	mov	rax,[number1]
	shl	rax,2		;乘以4(逻辑)
	mov	rsi,rax
	call	printres	
;负数
	mov 	rsi,msg1
	call	printmsg 	;打印标题
	mov	rsi,[number2]
	call	printbr
	;计算
	mov	rax,[number2]
	shl	rax,2		;负数逻辑左移*4
	mov	rsi,rax
	call	printres
	
;SAL--------------------------------------------------------
;正数
	mov	rsi,msg3
	call	printmsg	;打印标题
	mov	rsi,[number1]
	call	printbr		;打印number1
	mov	rax,[number1]
	sal	rax,2		;乘以4(算术)
	mov	rsi,rax
	call	printres
;负数
	mov	rsi,msg3
	call	printmsg
	mov	rsi,[number2]
	call	printbr
	mov	rax,[number2]
	sal	rax,2
	mov	rsi,rax
	call	printres
;SHR--------------------------------------------------------
;正数
	mov	rsi,msg5
	call	printmsg
	mov	rsi,[number1]
	call	printbr
	
	mov	rax,[number1]
	shr	rax,2
	mov	rsi,rax
	call	printres
;负数
	mov 	rsi,msg2
	call	printmsg
	mov	rsi,[number2]
	call	printbr

	mov	rax,[number2]
	shr	rax,2
	mov	rsi,rax
	call	printres

;SAR-------------------------------------------------------
;正数
	mov	rsi,msg4
	call	printmsg
	mov	rsi,[number2]
	call	printbr
	
	mov	rax,[number2]
	sar	rax,2
	mov	rsi,rax
	call	printbr
	
	mov	rsp,rbp
	pop	rbp
	ret

;-printmsg打印字符-------------------------------------------
printmsg:
	section .data
		.fmtstr	db "%s",10,0
	section .text
		mov	rdi,.fmtstr
		mov	rax,0
		call	printf
		ret
;------------------------------------------------------------

;打印数字----------------------------------------------------
printbr:
	section .data
		.fmtstr db "The original number is %lld",10,0
	section .text
		mov	rdi,.fmtstr
		mov	rax,0
		call	printf
		ret
;打印数字----------------------------------------------------

;打印结果----------------------------------------------------
printres:
	section .data
		.fmtstr	db "The resulting number is %lld",10,0
	section .text
		mov	rdi,.fmtstr
		mov	rax,0
		call	printf
		ret
;打印结果----------------------------------------------------
