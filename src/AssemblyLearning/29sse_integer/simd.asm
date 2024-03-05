extern printf
section .data
        dummy   db      13
align   16
        ;pdivector1相当于数组
        pdivector1      dd 1
                        dd 2
                        dd 3
                        dd 4
	;pdivector2相当于数组
        pdivector2      dd 5
                        dd 6
                        dd 7
                        dd 8
        fmt             db "Sum Vector:%d %d %d %d",10,0
section .bss
alignb  16
        pdivector_res   resd    4
section .text
        global main
main:
	;序言
        push    rbp
        mov     rbp,rsp

	;将pdivector1,加载到xmm0寄存器中
        movdqa  xmm0,[pdivector1]
	;将pdivector2和xmm0寄存器内的值,进行加法运算
        paddd  xmm0,[pdivector2]

        ;将结果保存在内存中
        movdqa  [pdivector_res],xmm0
        ;打印内存中的向量
        mov     rsi,pdivector_res
        mov     rdi,fmt
        call    printpdi

	;尾言
        mov     rsp,rbp
        pop     rbp
        ret

;打印-----------------------------------
printpdi:
        push    rbp
        mov     rbp,rsp

        movdqa  xmm0,[rsi]
        ;从xmmo0中提取打包的值
        pextrd  esi,xmm0,0
        pextrd  edx,xmm0,1
        pextrd  ecx,xmm0,2
        pextrd  r8d,xmm0,3

        ;没有浮点数
        mov     rax,0
        call    printf
        mov     rsp,rbp
        pop     rbp
        ret
