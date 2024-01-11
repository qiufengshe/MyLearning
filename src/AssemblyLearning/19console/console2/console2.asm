; console2.asm
; 控制台IO输出,防止溢出
;
section .data
	msg1	db "Hello, World!",10,0
	msg2	db "Your turn (only a-z):",0
	msg3	db "You answered:",0
	inputlen equ 10   	;input buffer的长度
	NL	db 0xa		;换行
section .bss
	input   resb inputlen+1  ;为终止的0提供空间
section .text
	global  main
main:
	push	rbp
	mov	rbp,rsp

	mov	rdi,msg1	;打印第一个字符
	call	prints
	mov	rdi,msg2	;打印第二个字符
	call	prints
	
	mov	rdi,input	;input buffer地址
	mov	rsi,inputlen	;inout buffer长度
	call	reads		;等待输入
	
	mov	rdi,msg3	;打印第三个字符串并添加输入的字符串
	call	prints
	
	mov	rdi,input	;打印input buffer
	call	prints
	
	mov	rdi,NL
	call	prints
	
	
	mov	rsp,rbp
	pop 	rbp
	ret


prints:
	push	rbp
	mov	rbp,rsp
	
	push	r12		;被调用者保留
	;字符计数
	xor	rdx,rdx		;长度存放在rdx中
	mov	r12,rdi
.lengthloop:
	cmp	byte [r12],0
	je 	.lengthfound
	inc	rdx
	inc     r12
	jmp	.lengthloop
.lengthfound:
	cmp	rdx,0
	je	.done
	mov 	rsi,rdi		;rdi包含字符的长度
	mov 	rax,1		;1表示写入
	mov	rdi,1		;1表示标准输出
	syscall
.done:
	mov	rsp,rbp
	pop 	rbp
	ret

reads:
section .data
section .bss
	.inputlen resb 1
section .text
	push	rbp
	mov	rbp,rsp
	push	r12	;被调用这保留
	push 	r13	;被调用者保留
	push	r14	;被调用者保留
	mov	r12,rdi ;input buffer长度
	mov	r13,rsi	;最大长度放在r13中
	xor	r14,r14 ;字符计数器

.readc:
	mov	rax,0	;读
	mov	rdi,1	;标准输入
	lea	rsi,[.inputlen]
	mov	rdx,1	;读取几个字符
	syscall	
	
	mov	al,[.inputlen]
	cmp	al,byte [NL] 
	je	.done	;换行结束
	cmp	al,97	;比a还小
	jl	.readc
	cmp	al,122  ;比z还大吗
	jg   	.readc
	inc	r14
	cmp	r14,r13
	ja	.readc		;达到缓冲区的最大限制,忽略
	mov	byte [r12],al	;安全缓冲区的字符
	inc	r12
	jmp	.readc
.done:
	mov	rsp,rbp
	pop	rbp
	ret
