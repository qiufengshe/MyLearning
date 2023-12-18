# alive2.asm
#
alive2: alive2.o
	gcc -g -o alive2 alive2.o -no-pie
alive2.o: alive2.asm
	nasm -f elf64 -g -F dwarf alive2.asm -l alive2.lst
clean:
	rm -rf alive2
