# UDP Package Structure
Position | Type | Data
---------|------|-----
0 | UInt32 | PackSeq
4 | UInt32 | Seq
8 | UInt32 | RestLength
12 | UInt32 | AckPack
16 | UInt32 | AckData
20 | UInt32 | PackCRC
24 | Byte[Length] | Data