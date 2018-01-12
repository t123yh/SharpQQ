using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpQQ.Binarizer;
using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;
using SharpQQ.Utils;

namespace Tests.Binarizer
{
    [TestClass]
    public class TlvTests
    {
        [TlvPacket(0x6666)]
        private class TlvTestPacketA : StructuredBinaryPacket
        {
            [IntegerField(1)]
            public uint TestField { get; set; } = 0;
        }

        [TlvPacket(0x2333)]
        private class TlvTestPacketB : StructuredBinaryPacket
        {
            [IntegerField(1)]
            public byte A { get; set; }

            [IntegerField(2)]
            public ushort B { get; set; }
        }

        [TestMethod]
        public void TestEncode()
        {
            var col = new TlvPacketCollection(0x12DF);
            col.Add(new TlvTestPacketA() {TestField = 0xFEE1DEAD});
            col.Add(new TlvTestPacketB() {A = 192, B = 60817});
            var result = col.GetBinary();
            Console.WriteLine(BinaryUtils.BinToHex(result));
            CollectionAssert.AreEqual(result, BinaryUtils.HexToBin("12DF000266660004FEE1DEAD23330003C0ED91"));
        }

        [TestMethod]
        public void TestDecode()
        {
            var dat = BinaryUtils.HexToBin("12DF000266660004FEE1DEAD23330003C0ED91");
            var col = new TlvPacketCollection();
            col.ParseFrom(dat);
            var p1 = col.Get<TlvTestPacketA>();
            var p2 = col.Get<TlvTestPacketB>();
            Assert.AreEqual(p1.TestField, 0xFEE1DEAD);
            Assert.AreEqual(p2.A, 192);
            Assert.AreEqual(p2.B, 60817);
        }
    }
}