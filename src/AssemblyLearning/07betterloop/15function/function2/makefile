# function6.asm
#
function6:function6.o
	gcc -o function6 function6.o -no-pie
function6.o:function6.asm
	nasm -f elf64 -g -F dwarf function6.asm -l function6.lst
clean:
	rm -rf function6
	rm -rf function6.o
	rm -rf function6.lst
