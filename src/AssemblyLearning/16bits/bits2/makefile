# bits2.asm printb.c
#
bits2:bits2.o 
	gcc -o bits2 bits2.o  -no-pie
bits2.o: bits2.asm
	nasm -f elf64 -g -F dwarf bits2.asm -l bits2.lst
clean:
	rm -rf bits2.o
	rm -rf bits2
	rm -rf bits2.lst
