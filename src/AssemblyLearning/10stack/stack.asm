; stack.asm  

extern printf 
section .data
	string  db "ABCDE",0
	stringlen equ $ - string-1
	fmt1 	db "The original string: %s",10,0
	fmt2    db "The reversed string: %s",10,0

section .bss
section .text
	global main
main: 
	push  rbp
	mov   rbp,rsp
;打印原始字符串
	mov   rdi,fmt1
	mov   rsi,string
	mov   rax,0
	call  printf

	xor   rax,rax
	mov   rbx,string		;将字符串的地址放入rbx
	mov   rcx,stringlen		;将长度放入rcx
	mov   r12,0			;使用r12作为指针
pushLoop:	
	mov   al,byte [rbx+r12]		;把字符移到rax
	push  rax;			;将rax压入栈
	inc   r12
	loop  pushLoop


;将字符串从堆栈中逐字符串出栈
	mov   rbx,string
	mov   rcx,stringlen
	mov   r12,0
popLoop:
	pop   rax			;从堆栈中出栈一个字符
	mov   byte[rbx + r12],al	;将字符移到string中
	inc   r12
	loop  popLoop
	mov   byte[rbx+r12],0		;使用0终止字符串

;打印反转后的字符串
	mov   rdi,fmt2
	mov   rsi,string
	mov   rax,0
	call  printf	

	mov   rsp,rbp
	pop   rbp
	ret
