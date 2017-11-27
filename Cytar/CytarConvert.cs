using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public static class CytarConvert
    {
        public static byte BytesToByte(byte[] data)
        {
            if (data.Length <= 0)
                return 0;
            return data[0];
        }
        public static Int16 BytesToInt16(byte[] data)
        {
            return (Int16)BytesToNumber<Int16>(data);
        }
        public static Int32 BytesToInt32(byte[] data)
        {
            return (Int32)BytesToNumber<Int32>(data);
            /*byte[] dataFill = new byte[4] { 0, 0, 0, 0 };
            for(var i = 0; i < data.Length; i++)
            {
                dataFill[i] = data[i];
            }
            return (Int32)(
                ((dataFill[0] & (0xFF)) << 0) |
                ((dataFill[1] & (0xFF))) << 8 |
                ((dataFill[2] & (0xFF)) << 16) |
                ((dataFill[3] & (0xFF)) << 24));*/
        }
        public static Int64 BytesToInt64(byte[] data)
        {
            return (Int64)BytesToNumber<Int64>(data);
        }
        public static float BytesToSingle(byte[] data)
        {
            byte[] dataFill = new byte[4] { 0, 0, 0, 0 };
            for (var i = 0; i < data.Length; i++)
            {
                dataFill[i] = data[i];
            }
            return BitConverter.ToSingle(dataFill, 0);
        }
        public static double BytesToDouble(byte[] data)
        {
            byte[] dataFill = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            for (var i = 0; i < data.Length; i++)
            {
                dataFill[i] = data[i];
            }
            return BitConverter.ToDouble(dataFill, 0);
        }
        static object BytesToNumber<T>(byte[] data)
        {
            var size = System.Runtime.InteropServices.Marshal.SizeOf<T>();
            byte[] dataFill = new byte[size];
            for(var i = 0; i < data.Length; i++)
            {
                dataFill[i] = data[i];
            }
            UInt64 number = 0;
            for(var i = 0; i < size; i++)
            {
                number |= (UInt64)((UInt64)dataFill[i] << (i * 8));
            }
            return number;
        }
        public static byte[] NumberToBytes(byte number)
        {
            return new byte[] { number };
        }
        public static byte[] NumberToBytes(UInt16 number)
        {
            return new byte[]
                {
                    (byte)(number&0xFF),
                    (byte)((number & 0xFF00)>>8)
                };
        }
        public static byte[] NumberToBytes(UInt32 number)
        {
            return new byte[]
                {
                    (byte)(number&0xFF),
                    (byte)((number & 0xFF00)>>8),
                    (byte)((number & 0xFF0000)>>16),
                    (byte)((number & 0xFF000000)>>24)
                };
        }
        public static byte[] NumberToBytes(UInt64 number)
        {
            return new byte[]
                {
                    (byte)(number&0xFF),
                    (byte)((number & 0xFF00)>>8),
                    (byte)((number & 0xFF0000)>>16),
                    (byte)((number & 0xFF000000)>>24),
                    (byte)((number & 0xFF00000000)>>32),
                    (byte)((number & 0xFF0000000000)>>40),
                    (byte)((number & 0xFF000000000000)>>48),
                    (byte)((number & 0xFF00000000000000)>>56),
                };
        }
        public static byte[] NumberToBytes(float number)
        {
            return BitConverter.GetBytes(number);
        }
        public static byte[] NumberToBytes(double number)
        {
            return BitConverter.GetBytes(number);
        }
    }
}
