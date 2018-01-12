using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpQQ.Protocol.Msf.Packets;
using SharpQQ.Binarizer;
using SharpQQ.Utils;

namespace Tests
{
    [TestClass]
    public class SsoRequestTests
    {
        [TestMethod]
        public void TestDecode()
        {
            var req = new SsoRequest();
            req.ParseFrom(BinaryUtils.HexToBin("0000000A00000000040000000005300000008300008CD62002BF8A2002BF8A01000000000000000000000000000004000000134865617274626561742E416C69766500000008D38EDE0E00000012333533363236303738393138353300000014606F8005C35A0AF29F1D0A0E8A064274001E7C3436303131303530303836303432337C41312E312E352E393131340000000400000004"));
            Assert.AreEqual(req.EncryptionType, SsoEncryptionType.NotEncrypted);
            var content = new SsoRequestContent();
            content.ParseFrom(req.EncryptedContent);
            Assert.AreEqual(content.Header.OperationName.ToLowerInvariant(), "heartbeat.alive");
        }
    }
}