;test3.asm

extern printf
section .data 
	number   dq  10
	pi       dq  3.14
	fmtint   db  "The number 0 to %d sum=%d",10,0
	fmtflt   db  "%lf",10,0

section .bss
section .text
	global main
main:
	push  rbp
	mov   rbp,rsp

	mov   rax,0
	mov   rcx,[number]
bloop:
	add   rax,rcx
	loop  bloop

	mov   rdx,rax
	mov   rdi,fmtint
	mov   rsi,[number]
	mov   rdx,rax
	mov   rax,0
	call  printf

;printf pi
	mov   rax,1
	movq  xmm0,[pi]
	mov   rdi,fmtflt
	call  printf

	mov   rsp,rbp
	pop   rbp

	ret
