; file operation assembly 
;
;
section .data
;用于条件汇编的表达式
	CREATE	equ	1
	OVERWRITE  equ 	1
	APPEND   equ 	1
	O_WRITE  equ 	1
	READ	equ 	1
	O_READ  equ 	1
	DELETE	equ	1
;系统调用的符号
	NR_read	equ	0
	NR_write equ	1
	NR_open equ 	2
	NR_close equ	3
	NR_lseek equ	8
	NR_create equ 	85
	NR_unlink equ	87
;创装和状态标志
	O_CREAT equ  	00000100q
	O_APPEND equ 	00000200q
;访问模式
	O_RDONLY  equ   000000q
	O_WRONLY  equ 	000001q
	O_RDWR	  equ 	000002q
;创建模式(权限)
	S_IRUSR   equ	00400q
	S_IWUSR	  equ   00200q

	NL	  equ   0xa
	bufferlen equ   64
	fileName  db	"testfile.txt",0
	FD	  dq    0  ;文件描述符
	text1     db    "1. Hello...to every one!",NL,0
	len1	  dq    $-text1-1   ;移除0=
	text2     db    "2. Here I am!",NL,0
	len2      dq    $-text2-1
	text3     db    "3. alife and kicking!",NL,0
	len3	  db    $-text3-1
	text4	  db    "Adios !!!",NL,0
	len4 	  dq 	$-text4-1

	error_Create  db  "error createing file",NL,0
	error_Close   db  "error closing file",NL,0
	error_Write   db  "error writing file",NL,0
	error_Open    db  "error opening file",NL,0
	error_Append  db  "error appending to file",NL,0
	error_Delete  db  "error deleting file",NL,0
	error_Read    db  "error reading file",NL,0
	error_Print   db  "error printing string",NL,0
	error_Position db "error positioning in file",NL,0	

	success_Create	db "File created and oped",NL,0
	success_Close 	db "File closed",NL,0
	success_Write	db "Written to file ",NL,0
	success_Open 	db "File opened for R/W",NL,0
	success_Append  db "File opened for appending",NL,0
	success_Delete  db "File deleted",NL,0
	success_Read	db "Reading file",NL,0
	success_Position db "Postiioned in file",NL,0
section .bss
	buffer	resb	bufferlen
section .text
	global	main
main:
	push	rbp
	mov	rbp,rsp

	%IF CREATE
	;创建并打开文件,然后关闭
	;创建并打开文件
	mov	rdi,fileName
	call	createFile
	mov	qword [FD],rax	;保存文件描述符
	;从文件读取
	mov	rdi,qword [FD]	
	mov	rsi,buffer
	%ENDIF
	%IF OVERWRITE
	;打开并覆盖文件,然后关闭
	;打开文件
	mov	rdi,fileName
	call	openFile
	mov	qword [FD],rax	;保存文件描述符
	;写文件#2,覆盖
	mov	rdi,qword [FD]
	mov 	rsi,text2
	mov	rdx,qword [len2]
	call	writeFile
	;关闭文件
	mov	rdi,qword [FD]
	call	closeFile
	%ENDIF

	mov	rsp,rbp
	pop	rbp
	ret	



global	createFile
createFile:
	mov	rax,NR_create
	mov 	rsi,S_IRUSR | S_IWUSR
	syscall
	
	cmp	rax,0
	jl	createerror
	mov 	rdi,success_Create
	push	rax	;被调用者保留
	call	printString
	pop	rax	;调用者保留
	ret
createerror:
	mov	rdi,error_Create
	call	printString
	ret

;文件操作函数
global readFile
readFile:
	mov	rax,NR_read
	syscall
	cmp	rax,0
	jl	readerror
	mov	byte [rsi+rax],0	;添加一个终止0
	mov 	rax,rsi
	mov	rdi,success_Read
	push	rax			;调用者保留
	call	printString
	pop	rax
  readerror:
	mov	rdi,error_Read
	call	printString
	ret
;---------------------------------------------------------
global openFile
openFile:
	mov	rax,NR_open
	mov	rsi,O_RDWR
	syscall
	cmp	rax,0
	jl	openerror
	mov	rdi,success_Open
	push	rax			;调用者保留
	call	printString
	pop	rax
	ret
  openerror:
	mov	rdi,error_Open
	call	printString
	ret
;----------------------------------------------------------
global writeFile
writeFile:
	mov	rax,NR_write
	syscall	
	cmp	rax,0
	jl 	writeerror
	mov 	rdi,success_Write
	call	printString
	ret
  writeerror:
	mov	rdi,error_Write
	call	printString
	ret
;--------------------------------------------------------
global  closeFile
closeFile:
	mov	rax,NR_close
	syscall	
	cmp 	rax,0
	jl	closeerror
	mov	rdi,success_Close
	call	printString
	ret
  closeerror:
	mov	rdi,error_Close
	call	printString
	ret

global printString
printString:
	;字符计数
	mov	r12,rdi
	mov	rdx,0
strLoop:
	cmp	byte [r12],0
	je	strDone
	inc     rdx
	inc	r12
	jmp 	strLoop
strDone:
	cmp	rdx,0
	je	prtDone
	mov	rsi,rdi
	mov	rax,1
	mov	rdi,1
	syscall
prtDone:
	ret
