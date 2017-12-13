# UDP Package Structure
Position | Type | Data
---------|------|-----
0 | Int32 | PackSeq
4 | Int32 | RestLength
8 | Int32 | Ack
[12] | [UInt32] | [PackCRC]
12[/16] | Byte[Length] | Data