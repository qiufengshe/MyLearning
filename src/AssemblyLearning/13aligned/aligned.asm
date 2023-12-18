; aligned.asm
; 如果函数中序言,只有push rbp没有 mov rbp,rsp的话,就没有栈空间对齐,就会出现
; Segmentation fault 程序崩溃的情况

extern printf
section .data
	fmt 	db "2 times pi equal %.14f",10,0
	pi	dq 3.14159265358979
section .bss
section .text
	global main
;-----------------------------------------------
func3:
	push    rbp
	mov     rbp,rsp

	movsd   xmm0,[pi]
	addsd   xmm0,[pi]
	mov 	rdi,fmt
	mov     rax,1
	call    printf
	
	leave
	ret

;---------------------------------------------
func2:
	push	rbp
	mov  	rbp,rsp

	call	func3	

	mov 	rsp,rbp
	pop	rbp
	ret
;--------------------------------------------
func1:
	push	rbp
	mov     rbp,rsp

	call	func2

	mov	rsp,rbp
	pop	rbp
	ret

;---------------------------------------------------
main:
	push 	rbp
	mov 	rbp,rsp

	call	func1
	;movsd	xmm0,[pi]
	;addsd 	xmm0,[pi]
	;mov 	rdi,fmt
	;mov 	rax,1
	;call 	printf

	mov	rsp,rbp
	pop 	rbp
	ret
