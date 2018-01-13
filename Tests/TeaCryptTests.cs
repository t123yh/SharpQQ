using System;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpQQ;
using SharpQQ.Utils;

namespace Tests
{
    [TestClass]
    public class TeaCryptTests
    {
        private readonly byte[] Data = Encoding.ASCII.GetBytes("TooYoung");
        private readonly byte[] Key = Encoding.UTF8.GetBytes("Sometimes naïve"); // 16 bytes long because `ï` is 2 bytes
        private readonly byte[] Cipher = BinaryUtils.HexToBin("774522845C2D529A");
        
        [TestMethod]
        public void TestEncrypt()
        {
            var encrypted = TeaCrypt.EncryptBlock(Data, Key);
            CollectionAssert.AreEqual(encrypted, Cipher);
        }

        [TestMethod]
        public void TestDecrypt()
        {
            var decrypted = TeaCrypt.DecryptBlock(Cipher, Key);
            CollectionAssert.AreEqual(decrypted, Data);
        }
    }
}