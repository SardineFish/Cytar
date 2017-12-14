# Package Structure
Position | Type | Data
---------|------|-----
0 | UInt32 | SessionID
4 | UInt32 | PackSeq
8 | UInt32 | Seq
12 | UInt32 | RestLength
16 | UInt32 | AckPack
20 | UInt32 | AckData
24 | UInt32 | PackCRC
28 | Byte[Length] | Data

# Handshake Package Structure
Position | Type | Data
---------|------|-----
0 | UInt32 |  0
4 | UInt32 | SessionID

# Stream Package Structure
Position | Type | Data
---------|------|-----
0 | UInt32 | SessionID
4 | UInt32 | Seq
8 | UInt32 | Ack
12 | UInt32 | CRC32
16 | Byte[] | Data