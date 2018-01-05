using System;
using System.Linq;

namespace SharpQQ.Protocol
{
    public class TeaCrypt
    {
        public const int BlockSize = 8;

        private static uint[] GetIntegerKey(byte[] keyBytes)
        {
            const int keySize = 4;
            if (keyBytes.Length != keySize * 4)
            {
                throw new ArgumentException("keyBytes must be 16 bytes long.");
            }
            uint[] result = new uint[keySize];
            for (int i = 0; i < keySize; i++)
            {
                result[i] = BitConverter.ToUInt32(keyBytes.Skip(i * 4).Take(4).Reverse().ToArray(), 0);
            }
            return result;
        }

        private static (uint, uint) ParseBytes(byte[] src)
        {
            if (src.Length != 8)
            {
                throw new ArgumentException("Source length must be 8 bytes.");
            }
            uint y = BitConverter.ToUInt32(src.Take(4).Reverse().ToArray(), 0),
                z = BitConverter.ToUInt32(src.Skip(4).Take(4).Reverse().ToArray(), 0);

            return (y, z);
        }

        private static byte[] ToBytes(uint a, uint b)
        {
            return BitConverter.GetBytes(a).Reverse().Concat(BitConverter.GetBytes(b).Reverse()).ToArray();
        }

        const uint TeaDelta = 0x9E3779B9;
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