# file.asm
#
file:file.o
	gcc -o file file.o -no-pie
file.o:file.asm
	nasm -f elf64 -g -F dwarf file.asm -l file.lst
clean:
	rm -rf file.lst
	rm -rf file.o
	rm -rf file
