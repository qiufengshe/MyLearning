; normal add 

extern printf 

section .data
	num1	dq 100
	fmt     db "0 to %ld sum=%ld",10,0

section .bss
section .text
	global main
main:
	push    rbp
	mov     rbp,rsp	
	mov     rbx,0
	mov     rax,0
bLoop:
	add     rax,rbx
	inc     rbx
	cmp     rbx,[num1]
	jle     bLoop

	mov     rdi,fmt
	mov     rsi,[num1]
	mov     rdx,rax
	mov     rax,0
	call    printf
	

	leave
	ret
