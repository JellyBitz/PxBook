 * * * PxBook  -  Packet Library * * *

  Hello, selfish community!

  I'm sharing today my loved tool to parse packets
without wasting too much time on it,
at the same time keeping your knowledge as organizated as possible!
  Started being for personal use but looks too good for only me using it.

 I made basically a tiny language to parse packets
in a easy way and see his results at variables,
working with hexdumps like this :

0000000000   02 01 02 80 07 00 00 09 00 4A 65 6C 6C 79 42 69   .........JellyBi
0000000016   74 7A 21 01 00 00 00 00 00 00 00 00 14 00 14 00   tz!.............
0000000032   00 00 C8 00 00 00 C8 00 00 00 00 00 00 00 05 41   ..È...È........A
0000000048   0E 00 00 00 42 0E 00 00 00 43 0E 00 00 00 34 0E   ....B....C....4.
0000000064   00 00 00 47 0E 00 00 00 00 84 07 00 00 05 00 4A   ...G...........J
0000000080   65 6C 6C 79 21 01 00 00 00 00 00 00 00 00 14 00   elly!...........
0000000096   14 00 00 00 C8 00 00 00 C8 00 00 00 00 00 00 00   ....È...È.......
0000000112   05 41 0E 00 00 00 42 0E 00 00 00 43 0E 00 00 00   .A....B....C....
0000000128   34 0E 00 00 00 47 0E 00 00 00 00                  4....G..........

or raw of bytes like :

02 01 02 80 07 00 00 09 00 4A 65 6C 6C 79 42 69 74 7A 21 01 00 00 00 00 00 00 00 00 14 00 14 00 00 00 C8 00 00 00 C8 00 00 00 00 00 00 00 05 41 0E 00 00 00 42 0E 00 00 00 43 0E 00 00 00 34 0E 00 00 00 47 0E 00 00 00 00 84 07 00 00 05 00 4A 65 6C 6C 79 21 01 00 00 00 00 00 00 00 00 14 00 14 00 00 00 C8 00 00 00 C8 00 00 00 00 00 00 00 05 41 0E 00 00 00 42 0E 00 00 00 43 0E 00 00 00 34 0E 00 00 00 47 0E 00 00 00 00

- Features

 * Read and parse packets using pxbook script (I'll explain the syntax with more details)
 * Variables viewer with FOR & WHILE support
 * Save and load scripts using his packet opcode
 * Packet script viewer (pretty printting included)

- Syntax

 ** All conditions are handled strictly using at least one space between his data
 * Are not case sensitive
 * Doesnt matter his margins
 * Thanks to the recursive technology it support nesting

[CODE]

// IF CONDITION!
// read (byte/uint8) and save it at a
byte a
if a == 1
    // read (byte/uint8) and override at a
   byte a
   byte b
   if a == b
       // do something?
   endif
endif
// FOR CONDITION!
byte n
for  n
   ascii  name
   ulong  exp
endfor
// WHILE CONDITION!
while byte == 1
    uint refID
    uint8 flag
endwhile

[/CODE]

- Variables :

* byte  (1 byte)
* uint8	 (1)
* ushort  (2)
* uint16  (2)
* uint (4)
* uint32  (4)
* ulong  (8)
* uint64  (8)
* single  (4)
* float  (4)
* ascii  (2 + length)
* ascii8  (1 + length)
* ascii16  (2 + length)
* ascii32  (4 + length)


At your app folder is only included two packet structures (.pxbook extension).

Bugs are welcome with open hands!

The funny thing is that can be used not only for silkroad,
you may find a better use of it.
Till then I'll think about sharing it at GitHub for a bit.

- Engels "JellyBitz" Quintero.