; bits1.asm
extern printf
extern printb

section .data
	msgn1	db	"Number  1",10,0
	msgn2	db	"Number  2",10,0
	msg1	db	"XOR",10,0
	msg2	db 	"OR",10,0
	msg3	db	"AND",10,0
	msg4	db	"NOT number 1",10,0
	msg5	db	"SHL 2 lower byte of number 1",10,0
	msg6	db	"SHL 2 lower byte of number 1",10,0	
	msg7 	db 	"SAL 2 lower byte of number 1",10,0
	msg8 	db	"SAR 2 lower byte of number 1",10,0
	msg9	db 	"ROL 2 lower byte of number 1",10,0
	msg10	db	"ROL 2 lower byte of nunber 2",10,0
	msg11	db	"ROR 2 lower byte of number 1",10,0
	msg12	db 	"ROR 2 lower byte of number 2",10,0
	number1 dq	-72
	number2 dq 	1064

section .bss
section	.text
	global main
main:
	push	rbp
	mov	rbp,rsp

	;打印number1
	mov	rsi,msgn1
	call	printmsg
	mov	rdi,[number1]
	call	printb	
	;打印number2
	mov	rsi,msgn2
	call	printmsg
	mov	rdi,[number2]
	call	printb
	
	;打印异或XOR 
	mov	rsi,msg1
	call	printmsg
	;xor运算后打印
	mov	rax,[number1]
	xor	rax,[number2]
	mov	rdi,rax
	call	printb
	
	;打印OR(或)
	mov	rsi,msg2
	call	printmsg
	;or运算后打印
	mov	rax,[number1]
	or	rax,[number2]
	mov	rdi,rax
	call	printb

	;打印AND(和)
	mov	rsi,msg3
	call	printmsg
	;AND运算
	mov	rax,[number1]
	and 	rax,[number2]
	mov	rdi,rax
	call	printb
	
	;打印NOT(非)
	mov	rsi,msg4
	call	printmsg
	;NOT运算
	mov	rax,[number1]
	not	rax
	mov	rdi,rax
	call	printb
	
	;打印左移(shift lef)
	mov	rsi,msg5
	call	printmsg
	;左移运算
	mov	rax,[number1]
	shl	al,2
	mov 	rdi,rax
	call	printb

	;打印右移(shift right)
	mov	rsi,msg6
	call	printmsg
	;右移运算
	mov	rax,[number1]
	shr	al,2
	mov	rdi,rax
	call	printb
	
	;打印左移算术
	mov	rsi,msg7
	call	printmsg
	;左移算术
	mov	rax,[number1]
	sal	al,2
	mov	rdi,rax
	call	printb
	
	;打印右移算术
	mov	rsi,msg8
	call	printmsg
	;右移算术(shift arithmetic right )
	mov	rax,[number1]
	sar	al,2
	mov	rdi,rax
	call	printb

	;打印向左旋转(rotate left)
	mov	rsi,msg9
	call	printmsg
	;向左旋转
	mov	rax,[number1]
	rol	al,2
	mov	rdi,rax
	call	printb

	;打印向右旋转(rotate right)
	mov	rsi,msg10
	call	printmsg
	;向右旋转
	mov	rax,[number1]
	ror	al,2
	mov	rdi,rax
	call	printb

	mov	rsp,rbp
	pop	rbp
	ret
printmsg:
section .data
	.fmtstr	db	"%s",0
section .text
	push	rbp
	mov	rbp,rsp

	mov	rdi,.fmtstr
	mov	rax,0
	call	printf	

	mov	rsp,rbp
	pop	rbp
	ret
