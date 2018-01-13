using System;
using System.Linq;
using SharpQQ.Utils;

namespace SharpQQ
{
    public class TeaCrypt
    {
        public const int BlockSize = 8;

        private static uint[] GetIntegerKey(byte[] keyBytes)
        {
            const int keySize = 4;
            if (keyBytes.Length != keySize * 4)
            {
                throw new ArgumentException("keyBytes must be 16 bytes long.", "keyBytes");
            }
            uint[] result = new uint[keySize];
            for (int i = 0; i < keySize; i++)
            {
                result[i] =MyBitConverter.GetUInt32(keyBytes, Endianness.Big, i * 4);
            }
            return result;
        }

        private static (uint, uint) ParseBytes(byte[] src)
        {
            if (src.Length != 8)
            {
                throw new ArgumentException("Source length must be 8 bytes.", "src");
            }
            uint y = MyBitConverter.GetUInt32(src, Endianness.Big, 0),
                z = MyBitConverter.GetUInt32(src, Endianness.Big, 4);

            return (y, z);
        }

        private static byte[] ToBytes(uint a, uint b)
        {
            return MyBitConverter.IntegerToBytes(a, Endianness.Big)
                .Concat(MyBitConverter.IntegerToBytes(b, Endianness.Big))
                .ToArray();
        }

        private const uint TeaDelta = 0x9E3779B9;
        
        public static byte[] DecryptBlock(byte[] cipherText, byte[] keyBytes)
        {
            uint[] key = GetIntegerKey(keyBytes);
            var (y, z) = ParseBytes(cipherText);

            unchecked
            {
                uint sum = TeaDelta << 4;
                for (int i = 0; i < 0x10; i++)
                {
                    z -= ((y << 4) + key[2]) ^ (y + sum) ^ ((y >> 5) + key[3]);
                    y -= ((z << 4) + key[0]) ^ (z + sum) ^ ((z >> 5) + key[1]);
                    sum -= TeaDelta;
                }
            }

            return ToBytes(y, z);
        }

        public static byte[] EncryptBlock(byte[] original, byte[] keyBytes)
        {
            uint[] key = GetIntegerKey(keyBytes);
            var (y, z) = ParseBytes(original);
            unchecked
            {
                uint sum = 0;
                for (int i = 0; i < 0x10; i++)
                {
                    sum += TeaDelta;
                    y += ((z << 4) + key[0]) ^ (z + sum) ^ ((z >> 5) + key[1]);
                    z += ((y << 4) + key[2]) ^ (y + sum) ^ ((y >> 5) + key[3]);
                }
            }
            return ToBytes(y, z);
        }
    }
}