; function6.asm

extern printf
section .data
	first	db "A",0
	second	db "B",0
	third	db "C",0
	fourth	db "D",0
	fifth	db "E",0
	sixth	db "F",0
	seventh	db "G",0
	eighth	db "H",0
	ninth	db "I",0
	tenth	db "J",0
	fmt	db "The string is %s",10,0
section .bss
	flist	resb 11
section .text
	global main
main:
	push 	rbp
	mov	rbp,rsp

	mov	rdi,flist
	mov	rsi,first
	mov	rdx,second
	mov	rcx,third
	mov	r8,fourth
	mov	r9,fifth
	push	tenth
	push	ninth
	push 	eighth
	push 	seventh
	push	sixth
	call	lfunc	;调用函数lfunc	
	
	;打印结果
	mov	rdi,fmt
	mov	rsi,flist
	mov 	rax,0
	call	printf	

	mov	rsp,rbp
	pop	rbp
	ret
;---------------------------------------------
lfunc:
	push	rbp
	mov	rbp,rsp

	xor	rax,rax	;清空rax寄存器
	mov	al,byte[rsi] ;将第一个参数放入al寄存器
	mov	[rdi],al     ;将al中的内容存储到内存中
	mov	al,byte[rdx] ;将第二个参数移到 al中
	mov 	[rdi+1],al   ;将al中内容存储到内存中
	mov	al,byte[rcx] ;
	mov 	[rdi+2],al
	mov 	al,byte[r8]
	mov	[rdi+3],al
	mov	al,byte[r9]
	mov	[rdi+4],al
	;从堆栈中获取参数
	push	rbx 	     ;被调用者保留
	xor	rbx,rbx
	mov	rax,qword [rbp+16] ;初始化堆栈+rip+rbp
	mov	bl,byte[rax]	;提前字符串
	mov	[rdi+5],bl	;把字符串存储到内存中
	mov	rax,qword[rbp+24] ;获取下一个值
	mov 	bl,byte[rax]
	mov	[rdi+6],bl
	mov	rax,qword[rbp+32]
	mov	bl,byte[rax]
	mov	[rdi+7],bl
	mov	rax,qword[rbp+40]
	mov	bl,byte[rax]
	mov	[rdi+8],bl
	mov	rax,qword[rbp+48]
	mov	bl,byte[rax]
	mov	[rdi+9],bl
	mov	bl,0
	mov	[rdi+10],bl
	pop	rbx		;被调用着保留
	
	mov	rsp,rbp
	pop	rbp
	ret
