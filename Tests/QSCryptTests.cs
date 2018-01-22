using System;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpQQ;
using SharpQQ.Utils;

namespace Tests
{
    [TestClass]
    public class QSCryptTests
    {
        private readonly byte[] Key = Encoding.ASCII.GetBytes("TooYoung........");

        [DataTestMethod]
        [DataRow("h")]
        [DataRow("hh")]
        [DataRow("moha")]
        [DataRow("Aloha 'Oe")]
        [DataRow("Apply for Professor")]
        [DataRow("闷声大发财")]
        public void TestEncryptAndDecrypt(string plainText)
        {
            byte[] dat = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = QSCrypt.Encrypt(dat, Key);
            Console.WriteLine(encrypted.ToHex());
            byte[] decrypted = QSCrypt.Decrypt(encrypted, Key);
            CollectionAssert.AreEqual(dat, decrypted);
        }

        [DataTestMethod]
        [DataRow("moha", "C35AE5C24AB54F5B2E79AE9C166B5A6A")]
        [DataRow("闷声大发财", "4366DF443C31B0EF2E16E3C113CE897138B3A2464D23E9B83A3783C6B7CD5714")]
        public void TestDecrypt(string expectedPlainText, string cipherHex)
        {
            byte[] decrypted = QSCrypt.Decrypt(cipherHex.ToBin(), Key);
            byte[] plain = Encoding.UTF8.GetBytes(expectedPlainText);
            CollectionAssert.AreEqual(decrypted, plain);
        }
    }
}