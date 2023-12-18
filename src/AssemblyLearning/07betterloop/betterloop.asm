;betterloop.asm

extern printf
section .data
	number   dq  1000000000
	fmt      db  "The sum from 0 to %ld is %ld",10,0
section .bss
section .text
	global main

main:
	push rbp
	mov  rbp,rsp

	mov  rcx,[number]   ;将number放入rcx,后面递减
	mov  rax,0	    ;初始化rax

bloop:
	add  rax,rcx	    ;将rcx的放入rax 第一次是5 后面依次 4  3  2 1 
	loop bloop
	mov  rdi,fmt
	mov  rsi,[number]
	mov  rdx,rax        ;将rax的值放入到rdx
	mov  rax,0
	call printf	


	mov  rsp,rbp
	pop  rbp

	ret
