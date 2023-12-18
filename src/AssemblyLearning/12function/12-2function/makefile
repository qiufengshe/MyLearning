# function2.asm
#
#
function2:function2.o
	gcc -o function2 function2.o -no-pie
function2.o:function2.asm
	nasm -f elf64 -g -F dwarf function2.asm -l function2.lst
clean:
	rm -rf function2.lst
	rm -rf function2.o
	rm -rf function2
