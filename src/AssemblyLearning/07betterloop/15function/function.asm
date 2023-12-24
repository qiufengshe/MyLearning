; function.asm 
; 调用约定
; 整数参数
;	第一个参数放入rdi	
;	第二个参数放入rsi
;	第三个参数放入rdx
;	第四个参数放入rcx
;	第五个参数放入r8
;	第六个参数放入r9
;超过6个参数后
extern	printf
section .data
	first	db "A",0
	second 	db "B",0
	third	db "C",0
	fourth	db "D",0
	fifth	db "E",0
	sixth	db "F",0
	seventh db "G",0
	eighth	db "H",0
	ninth	db "I",0
	tenth	db "J",0
	fmt1	db "string is :%s%s%s%s%s%s%s%s%s%s",10,0
	fmt2	db "PI= %f",10,0
	pi	dq 3.14
section .bss
section .text
	global main
main:
	push	rbp
	mov	rbp,rsp
	
	mov	rdi,fmt1
	mov	rsi,first
	mov	rdx,second
	mov 	rcx,third
	mov	r8,fourth
	mov	r9,fifth
	;超过6个参数后,使用压栈,按相反的顺序压入栈中
	push	tenth
	push	ninth
	push 	eighth
	push	seventh
	push	sixth
	mov	rax,0
	call	printf
	and	rsp,0xfffffffffffffff0
	movsd	xmm0,[pi]
	mov	rax,1
	mov	rdx,fmt2
	call	printf 

	mov	rsp,rbp
	pop	rbp
	ret
