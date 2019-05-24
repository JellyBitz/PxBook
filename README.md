# PxBook
Simple but effective Packet reader (hex bytes) acting as a Book at the same time! 

Basically is made with a very limited language like script ([Syntax here](#Syntax "Syntax here")) to parse packet dumps and see his results at variables, saving you time from connection and errors not handled before.

### Features
- Read and parse packets using pxbook script and seeing all processs
- Variables viewer with FOR support
- Save and load scripts using his packet "opcode"
- Packet script viewer (pretty printting included)

Works with **raw of bytes** or **hexdumps** like this:
```
0000000000   02 01 02 80 07 00 00 09 00 4A 65 6C 6C 79 42 69   .........JellyBi
0000000016   74 7A 21 01 00 00 00 00 00 00 00 00 14 00 14 00   tz!.............
0000000032   00 00 C8 00 00 00 C8 00 00 00 00 00 00 00 05 41   ..È...È........A
0000000048   0E 00 00 00 42 0E 00 00 00 43 0E 00 00 00 34 0E   ....B....C....4.
0000000064   00 00 00 47 0E 00 00 00 00 84 07 00 00 05 00 4A   ...G...........J
0000000080   65 6C 6C 79 21 01 00 00 00 00 00 00 00 00 14 00   elly!...........
0000000096   14 00 00 00 C8 00 00 00 C8 00 00 00 00 00 00 00   ....È...È.......
0000000112   05 41 0E 00 00 00 42 0E 00 00 00 43 0E 00 00 00   .A....B....C....
0000000128   34 0E 00 00 00 47 0E 00 00 00 00                  4....G..........
```
#### Syntax
- All conditions are handled strictly using at least one space between his data
- Are not case sensitive
- Doesn't matter his margins
- Nesting support

#### Variables
- `byte` : 1 byte
- `uint8` : 1
- `ushort` : 2
- `uint16` : 2
- `uint` : 4
- `uint32` : 4
- `ulong` : 8
- `uint64` : 8
- `single` : 4
- `float` : 4
- `ascii` : 2 + length
- `ascii8` : 1 + length
- `ascii16` : 2 + length
- `ascii32` : 4 + length

##### Script Example:
```
// IF CONDITION!
// read byte and save it at A
byte A
if A == 1
    // read byte and override at A
    byte A
    byte B
    if A > 0
        // do something?
        if A <= B
            // and.. ?
        endif
    endif
endif
// FOR CONDITION!
byte n
for n
    ascii name
    ulong exp
endfor
// WHILE CONDITION!
while byte == 1
    uint32 refID
    uint8 flag
endwhile
```
###### Created on VisualStudio2015 with NET 4.5

------------
> **Do you like this Project ?**
> Support me! [Buy me a coffee <img src="https://twemoji.maxcdn.com/2/72x72/2615.png" width="18" height="18">](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=3L3UFLBN746AC "Coffee <3")
> 
> Made with <img title="Love" src="https://twemoji.maxcdn.com/2/72x72/1f499.png" width="18" height="18"> for GitHub. Pull if you want! <img title="JellyBitz" src="https://twemoji.maxcdn.com/2/72x72/1f575.png" width="18" height="18">
