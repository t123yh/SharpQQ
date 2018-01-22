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
        [DataTestMethod]
        [DataRow("TooYoung", "Sometimes naïve", "774522845C2D529A")]
        [DataRow("12345678", "1234567890ABCDEF", "6F129CC8EF171F21")]
        [DataRow("2[fb.0dv", "&^ayd?f3. tcq,y7", "187A1214FE7F5134")]
        public void TestEncryptDecrypt(string dataStr, string keyStr, string cipherHex)
        {
            byte[] data = Encoding.UTF8.GetBytes(dataStr);
            byte[] key = Encoding.UTF8.GetBytes(keyStr);
            byte[] cipher = cipherHex.ToBin();
            
            var encrypted = TeaCrypt.EncryptBlock(data, key);
            Console.WriteLine(encrypted.ToHex());
            var decrypted = TeaCrypt.DecryptBlock(cipher, key);
            CollectionAssert.AreEqual(encrypted, cipher);
            CollectionAssert.AreEqual(decrypted, data);
        }
    }
}