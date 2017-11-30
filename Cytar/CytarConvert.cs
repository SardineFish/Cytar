using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public static class CytarConvert
    {
        public static byte ToByte(byte[] data)
        {
            if (data.Length <= 0)
                return 0;
            return data[0];
        }
        public static Boolean ToBoolean(byte[] data)
        {
            if (data.Length < 0)
                throw new ArgumentException("Length of data must > 0");
            return data[0] > 0;
        }
        public static Int16 ToInt16(byte[] data)
        {
            return (Int16)(
                ((Int16)data[0] << 0) | 
                ((Int16)data[1] << 8));
        }
        public static UInt16 ToUInt16(byte[] data)
        {
            return (UInt16)(
                ((UInt16)data[0] << 0) |
                ((UInt16)data[1] << 8));
        }
        public static Int32 ToInt32(byte[] data)
        {
            return (Int32)(
                ((Int32)data[0] << 0) |
                ((Int32)data[1] << 8) |
                ((Int32)data[2] << 16) |
                ((Int32)data[3] << 24));
        }
        public static UInt32 ToUInt32(byte[] data)
        {
            return (UInt32)(
                ((UInt32)data[0] << 0) |
                ((UInt32)data[1] << 8) |
                ((UInt32)data[2] << 16) |
                ((UInt32)data[3] << 24));
        }
        public static Int64 ToInt64(byte[] data)
        {
            return (Int64)(
                ((Int64)data[0] << 0) |
                ((Int64)data[1] << 8) |
                ((Int64)data[2] << 16) |
                ((Int64)data[3] << 24) |
                ((Int64)data[4] << 32) |
                ((Int64)data[5] << 40) |
                ((Int64)data[6] << 48) |
                ((Int64)data[7] << 56));
        }
        public static UInt64 ToUInt64(byte[] data)
        {
            return (UInt64)(
                ((UInt64)data[0] << 0) |
                ((UInt64)data[1] << 8) |
                ((UInt64)data[2] << 16) |
                ((UInt64)data[3] << 24) |
                ((UInt64)data[4] << 32) |
                ((UInt64)data[5] << 40) |
                ((UInt64)data[6] << 48) |
                ((UInt64)data[7] << 56));
        }
        public static float ToSingle(byte[] data)
        {
            byte[] dataFill = new byte[4] { 0, 0, 0, 0 };
            for (var i = 0; i < data.Length; i++)
            {
                dataFill[i] = data[i];
            }
            return BitConverter.ToSingle(dataFill, 0);
        }
        public static double ToDouble(byte[] data)
        {
            byte[] dataFill = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            for (var i = 0; i < data.Length; i++)
            {
                dataFill[i] = data[i];
            }
            return BitConverter.ToDouble(dataFill, 0);
        }
        static object ToNumber<T>(byte[] data)
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
        public static byte[] ToBytes(byte number)
        {
            return new byte[] { number };
        }
        public static byte[] ToBytes(bool value)
        {
            return new byte[] { value ? (byte)1 : (byte)0 };
        }
        public static byte[] ToBytes(UInt16 number)
        {
            return new byte[]
                {
                    (byte)(number&0xFF),
                    (byte)((number & 0xFF00)>>8)
                };
        }
        public static byte[] ToBytes(Int16 number) => ToBytes((UInt16)number);

        public static byte[] ToBytes(UInt32 number)
        {
            return new byte[]
                {
                    (byte)(number&0xFF),
                    (byte)((number & 0xFF00)>>8),
                    (byte)((number & 0xFF0000)>>16),
                    (byte)((number & 0xFF000000)>>24)
                };
        }
        public static byte[] ToBytes(Int32 number) => ToBytes((UInt32)number);

        public static byte[] ToBytes(UInt64 number)
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
        public static byte[] ToBytes(Int64 number) => ToBytes((UInt64)number);

        public static byte[] ToBytes(float number) => BitConverter.GetBytes(number);

        public static byte[] ToBytes(double number) => BitConverter.GetBytes(number);
    }
}
