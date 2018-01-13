using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable ConvertIfStatementToSwitchStatement

namespace SharpQQ.Utils
{
    public enum Endianness
    {
        Big,
        Little
    }

    public static class MyBitConverter
    {
        #region Int16

        public static byte[] GetBytesFromUInt16(ushort val, Endianness e)
        {
            if (e == Endianness.Little)
                return new[] {(byte) (val >> 0), (byte) (val >> 8)};
            else
                return new[] {(byte) (val >> 8), (byte) (val >> 0)};
        }

        public static ushort GetUInt16(byte[] dat, Endianness e, int startIndex = 0)
        {
            if (e == Endianness.Little)
                return (ushort) ((dat[startIndex + 0] << 0) | (dat[startIndex + 1] << 8));
            else
                return (ushort) ((dat[startIndex + 0] << 8) | (dat[startIndex + 1] << 0));
        }

        public static byte[] GetBytesFromInt16(short val, Endianness e)
        {
            return GetBytesFromUInt16((ushort) val, e);
        }

        public static short GetInt16(byte[] dat, Endianness e, int startIndex = 0)
        {
            return (short) GetUInt16(dat, e, startIndex);
        }

        #endregion

        #region Int32

        public static byte[] GetBytesFromUInt32(uint val, Endianness e)
        {
            if (e == Endianness.Little)
                return new[]
                {
                    (byte) (val >> 0),
                    (byte) (val >> 8),
                    (byte) (val >> 16),
                    (byte) (val >> 24)
                };
            else
                return new[]
                {
                    (byte) (val >> 24),
                    (byte) (val >> 16),
                    (byte) (val >> 8),
                    (byte) (val >> 0)
                };
        }

        public static uint GetUInt32(byte[] dat, Endianness e, int startIndex = 0)
        {
            if (e == Endianness.Little)
                return (uint) (
                    (dat[startIndex + 0] << 0) |
                    (dat[startIndex + 1] << 8) |
                    (dat[startIndex + 2] << 16) |
                    (dat[startIndex + 3] << 24)
                );
            else
                return (uint) (
                    (dat[startIndex + 0] << 24) |
                    (dat[startIndex + 1] << 16) |
                    (dat[startIndex + 2] << 8) |
                    (dat[startIndex + 3] << 0)
                );
        }

        public static byte[] GetBytesFromInt32(int val, Endianness e)
        {
            return GetBytesFromUInt32((uint) val, e);
        }

        public static int GetInt32(byte[] dat, Endianness e, int startIndex = 0)
        {
            return (int) GetUInt32(dat, e, startIndex);
        }

        #endregion

        #region Int64

        public static byte[] GetBytesFromUInt64(ulong val, Endianness e)
        {
            if (e == Endianness.Little)
                return new[]
                {
                    (byte) (val >> 0),
                    (byte) (val >> 8),
                    (byte) (val >> 16),
                    (byte) (val >> 24),
                    (byte) (val >> 32),
                    (byte) (val >> 40),
                    (byte) (val >> 48),
                    (byte) (val >> 56)
                };
            else
                return new[]
                {
                    (byte) (val >> 56),
                    (byte) (val >> 48),
                    (byte) (val >> 40),
                    (byte) (val >> 32),
                    (byte) (val >> 24),
                    (byte) (val >> 16),
                    (byte) (val >> 8),
                    (byte) (val >> 0)
                };
        }

        public static ulong GetUInt64(byte[] dat, Endianness e, int startIndex = 0)
        {
            if (e == Endianness.Little)
                return (ulong) (
                    (dat[startIndex + 0] << 0) |
                    (dat[startIndex + 1] << 8) |
                    (dat[startIndex + 2] << 16) |
                    (dat[startIndex + 3] << 24) |
                    (dat[startIndex + 4] << 32) |
                    (dat[startIndex + 5] << 40) |
                    (dat[startIndex + 6] << 48) |
                    (dat[startIndex + 7] << 56)
                );
            else
                return (ulong) (
                    (dat[startIndex + 0] << 56) |
                    (dat[startIndex + 1] << 48) |
                    (dat[startIndex + 2] << 40) |
                    (dat[startIndex + 3] << 32) |
                    (dat[startIndex + 4] << 24) |
                    (dat[startIndex + 5] << 16) |
                    (dat[startIndex + 6] << 8) |
                    (dat[startIndex + 7] << 0)
                );
        }

        public static byte[] GetBytesFromInt64(long val, Endianness e)
        {
            return GetBytesFromUInt64((ulong) val, e);
        }

        public static long GetInt64(byte[] dat, Endianness e, int startIndex = 0)
        {
            return (long) GetUInt64(dat, e, startIndex);
        }

        #endregion

        #region Objects

        public static byte[] IntegerToBytes(object val, Endianness e)
        {
            byte[] converted;
            if (val is byte b)
                converted = new byte[1] {b};
            else if (val is sbyte sb)
                converted = new byte[1] {(byte) sb};
            else if (val is short s)
                converted = GetBytesFromInt16(s, e);
            else if (val is ushort us)
                converted = GetBytesFromUInt16(us, e);
            else if (val is int i)
                converted = GetBytesFromInt32(i, e);
            else if (val is uint ui)
                converted = GetBytesFromUInt32(ui, e);
            else if (val is long l)
                converted = GetBytesFromInt64(l, e);
            else if (val is ulong ul)
                converted = GetBytesFromUInt64(ul, e);
            else
                throw new ArgumentException("Argument type incorrect: expected one of the number types");

            return converted;
        }

        public static object BytesToInteger(byte[] dat, Type targetType, Endianness e, int startIndex = 0)
        {
            if (targetType == typeof(byte))
                return dat[startIndex + 0];
            else if (targetType == typeof(sbyte))
                return (sbyte) dat[startIndex + 0];
            else if (targetType == typeof(short))
                return GetInt16(dat, e, startIndex);
            else if (targetType == typeof(ushort))
                return GetUInt16(dat, e, startIndex);
            else if (targetType == typeof(int))
                return GetInt32(dat, e, startIndex);
            else if (targetType == typeof(uint))
                return GetUInt32(dat, e, startIndex);
            else if (targetType == typeof(long))
                return GetInt64(dat, e, startIndex);
            else if (targetType == typeof(ulong))
                return GetUInt64(dat, e, startIndex);
            else
                throw new ArgumentException(
                    $"Target type must be one of the numeric types instead of {targetType.FullName}");
        }

        public static readonly ReadOnlyDictionary<Type, int> IntegerSizes =
            new ReadOnlyDictionary<Type, int>(new Dictionary<Type, int>
            {
                {typeof(byte), 1},
                {typeof(sbyte), 1},
                {typeof(short), 2},
                {typeof(ushort), 2},
                {typeof(int), 4},
                {typeof(uint), 4},
                {typeof(long), 8},
                {typeof(ulong), 8}
            });

        #endregion
    }
}