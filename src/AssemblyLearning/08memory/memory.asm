; memory.asm 
section .data
	bNum  db 123
	Wnum  dw 12345
	warray  times  5 dw 0 ; 包含5个元素,每个元素都是0

	dNum   dd  12345
	qNum1  dq  12345
	text1  db  "abc",0
	qNum2  dq  3.141592654
	text2  db  "cde",0

section .bss
	bvar   resb 1
	dvar   resd 1
	wvar   resw 10
	qvar   resq 3
section .text
	global main
main:
	push   rbp
	mov    rbp,rsp
;
	lea    rax,[bNum]  ;在rax中加载bNum的值
	mov    

	mov    rsp,rbp
	pop    rbp

	ret	
