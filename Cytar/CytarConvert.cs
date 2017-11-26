using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public static class CytarConvert
    {
        public static UInt32 BytesToUInt32(byte[] data)
        {
            byte[] dataFill = new byte[4] { 0, 0, 0, 0 };
            for(var i = 0; i < data.Length; i++)
            {
                dataFill[i] = data[i];
            }
            return (UInt32)(
                ((dataFill[0] & (0xFF)) << 0) |
                ((dataFill[1] & (0xFF))) << 8 |
                ((dataFill[2] & (0xFF)) << 16) |
                ((dataFill[3] & (0xFF)) << 24));
        }

        public static byte[] UInt32ToBytes(UInt32 number)
        {
            return new byte[]
            {
                (byte)((number&0xFF)>>0),
                (byte)((number&0xFF00)>>8),
                (byte)((number&0xFF0000)>>16),
                (byte)((number&0xFF000000)>>24),
            };
        }
    }
}
