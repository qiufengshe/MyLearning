# bits.asm printb.c
#
bits1:bits1.o printb.o
	gcc -o bits1 bits1.o printb.o -no-pie
bits1.o:bits1.asm
	nasm -f elf64 -g -F dwarf bits1.asm -l bits1.lst
printb.o:printb.c
	gcc -c printb.c
clean:
	rm -rf printb.o
	rm -rf bits1.o
	rm -rf bits1.lst
	rm -rf bits1
