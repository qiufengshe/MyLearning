# function4.asm/circle.asm/rect.asm
#
#
function4:function4.o circle.o rect.o
	gcc -g -o function4 function4.o circle.o rect.o -no-pie
function4.o:function4.asm
	nasm -f elf64 -g -F dwarf function4.asm -l function4.lst
circle.o:circle.asm
	nasm -f elf64 -g -F dwarf circle.asm -l circle.lst
rect.o:rect.asm
	nasm -f elf64 -g -F dwarf rect.asm -l rect.lst
clean:
	rm -rf function4.o
	rm -rf function4.lst
	rm -rf circle.o
	rm -rf circle.lst
	rm -rf rect.o
	rm -rf rect.lst
