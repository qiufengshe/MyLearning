;hello.asm

section .data
	msg db "hello,world",0
	NL  db 0xa
section .bss

section .text
	global main

main:
	mov rax,1 ;1表示写入
	mov rdi,1 ;1表示标准输出
	mov rsi,msg ;需要显示的字符串放在rsi中
	mov rdx,12  ;字符串的长度,不包括0
	syscall     ;显示字符串
	
	mov rax,1   ;1表示写入
	mov rdi,1   ;1表示标准输出
	mov rsi,NL  ;显示新行
	mov rdx,1   ;字符串的长度
	syscall 

	mov rax,60  ;表示退出
	mov rdi,0   ;0是成功退出代码
	syscall 
