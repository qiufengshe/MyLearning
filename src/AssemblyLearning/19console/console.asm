; console.asm
;
;
extern printf
section .data
	msg1	db	"Hello, World!",10,0
	msg1len equ	 $-msg1
	msg2	db "Your turn: ",0
	msg2len equ	 $-msg2
	msg3	db "You answered: ",0
	msg3len equ	 $-msg3
	inputlen equ	 10
section .bss
	input	resb	inputlen+1
section .text
	global main
main:
	push	rbp
	mov	rbp,rsp

	mov	rdi,msg1	;打印第一个字符
	mov 	rsi,msg1len
	call	prints
	;call	printf

	mov	rsi,msg2	;打印第二个字符
	mov	rdi,msg2len
	call	prints
	
	mov	rsi,input 
	mov	rdx,inputlen
	call	reads

	mov 	rsi,msg3
	mov	rdi,msg3len
	call	prints

	mov	rsi,input 
	mov	rdx,inputlen	

	mov	rsp,rbp
	pop	rbp	
	ret

;-------------------------------------------------
prints:
	push	rbp
	mov	rbp,rsp

	;rsi包含字符串的地址
	;rdx包含字符串的长度
	mov	rsi,1		;1表示写入
	mov	rdx,1		;1表示标准写入
	syscall

	mov	rsp,rbp
	pop	rbp
	ret

;---------------------------------------------------
reads:
	push	rbp
	mov	rbp,rsp
	
	;rsi包含inputbuffer的地址
	;rdx包含inputbuffer的长度
	mov	rax,0	;0表示读取
	mov	rdx,1	;1表示标准输入
	syscall

	mov	rsp,rbp
	pop	rbp	
	ret
