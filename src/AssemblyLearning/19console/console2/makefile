# console2.asm
# 控制台输出,防止溢出
#
console2:console2.o
	gcc -o console2 -g console2.o -no-pie
console2.o:console2.asm
	nasm -f elf64 -g -F dwarf console2.asm -l console2.lst
clean:
	rm -rf console2.lst
	rm -rf console2.o
	rm -rf console2
