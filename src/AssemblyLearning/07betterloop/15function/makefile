# function.asm
#
function:function.o
	gcc -o function function.o -no-pie
function.o:function.asm
	nasm -f elf64 -g -F dwarf function.asm -l function.lst
clean:
	rm -rf function
	rm -rf fucntion.o
	rm -rf function.lst
