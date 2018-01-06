using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpQQ.Protocol
{
    public class QSCrypt
    {
        public static readonly byte[] ZeroKey = Enumerable.Repeat((byte)0, 16).ToArray();
        
        public static int GetEncryptedSize(int originalSize)
        {
            int totalSize = originalSize + 10,
                remainder = totalSize % 8;
            return totalSize + (remainder > 0 ? 8 - remainder : 0);
        }

        static Random randomGenerator = new Random();

        static byte getRandomByte()
        {
            // return (byte)(randomGenerator.Next() % 255);
            return 0;
        }

        static byte XOR(byte a, byte b)
        {
            return (byte)(a ^ b);
        }

        static byte[] GenerateByteArray(int len, byte initial = 0)
        {
            return Enumerable.Repeat(initial, len).ToArray();
        }

        const int TeaBlockSize = TeaCrypt.BlockSize;

        static byte[] TeaDecryptStream(byte[] cipherText, byte[] key)
        {
            if (cipherText.Length % TeaBlockSize != 0)
            {
                throw new ArgumentException($"Cipher text length must be a multiple of block size ${TeaBlockSize}, given {cipherText.Length}.");
            }

            List<byte> plainText = new List<byte>(cipherText.Length);
            byte[] lastCipherText = GenerateByteArray(TeaBlockSize),
                lastInputText = GenerateByteArray(TeaBlockSize);

            for (int i = 0; i < cipherText.Length / TeaBlockSize; i++)
            {
                var encryptedBlock = cipherText
                    .Skip(i * TeaBlockSize)
                    .Take(TeaBlockSize).ToArray();

                var cipherBlock = encryptedBlock.Zip(lastInputText, XOR).ToArray();
                var decrypted = TeaCrypt.DecryptBlock(cipherBlock, key).ToArray();

                plainText.AddRange(decrypted.Zip(lastCipherText, XOR));

                lastInputText = decrypted;
                lastCipherText = encryptedBlock;
            }
            return plainText.ToArray();
        }

        static byte[] TeaEncryptStream(byte[] plainText, byte[] key)
        {
            if (plainText.Length % TeaBlockSize != 0)
            {
                throw new ArgumentException($"Plain data length must be a multiple of block size ${TeaBlockSize}, given {plainText.Length}.");
            }

            List<byte> cipherText = new List<byte>(plainText.Length);
            byte[] lastCipherText = GenerateByteArray(TeaBlockSize),
                lastInputText = GenerateByteArray(TeaBlockSize);
            for (int i = 0; i < plainText.Length / TeaBlockSize; i++)
            {
                var input = plainText
                    .Skip(i * TeaBlockSize)
                    .Take(TeaBlockSize)
                    .Zip(lastCipherText, XOR)
                    .ToArray();

                var cipherBlock = TeaCrypt.EncryptBlock(input, key);
                var encryptedBlock = cipherBlock.Zip(lastInputText, XOR).ToArray();

                lastInputText = input;
                lastCipherText = encryptedBlock;

                cipherText.AddRange(encryptedBlock);
            }
            return cipherText.ToArray();
        }

        public static byte[] Encrypt(byte[] plainText, byte[] key)
        {
            int remSize = (plainText.Length + 10) % 8;
            int paddingSize = remSize != 0 ? 8 - remSize : 0;
            byte[] padding = Enumerable
                .Repeat(0, paddingSize + 3)
                .Select(x => getRandomByte())
                .ToArray();
            // The last 8 bits of the first padding byte indicates the padding size.
            padding[0] = (byte)(padding[0] & 0xF8 | paddingSize);

            const int zeroSize = 7;
            byte[] padded = padding
                .Concat(plainText)
                .Concat(GenerateByteArray(zeroSize))
                .ToArray();
            return TeaEncryptStream(padded, key);
        }

        public static byte[] Decrypt(byte[] cipherText, byte[] key)
        {
            byte[] teaDecrypted = TeaDecryptStream(cipherText, key);
            int paddingSize = (teaDecrypted[0] & 7) + 3;
            return teaDecrypted.Skip(paddingSize).Take(teaDecrypted.Length - paddingSize - 7).ToArray();
        }
    }
}