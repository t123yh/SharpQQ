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
        [TlvPacketContent(0x6666)]
        private class TlvTestConvertibleA : StructuredBinaryConvertible
        {
            [IntegerField(1)]
            public uint TestField { get; set; } = 0;
        }

        [TlvPacketContent(0x2333)]
        private class TlvTestConvertibleB : StructuredBinaryConvertible
        {
            [IntegerField(1)]
            public byte A { get; set; }

            [IntegerField(2)]
            public ushort B { get; set; }
        }

        [TestMethod]
        public void TestEncode()
        {
            var col = new TlvConvertibleCollection(0x12DF);
            col.Add(new TlvTestConvertibleA() {TestField = 0xFEE1DEAD});
            col.Add(new TlvTestConvertibleB() {A = 192, B = 60817});
            var result = col.GetBinary();
            Console.WriteLine(BinaryUtils.ToHex(result));
            CollectionAssert.AreEqual(result, "12DF000266660004FEE1DEAD23330003C0ED91".ToBin());
        }

        [TestMethod]
        public void TestDecode()
        {
            var dat = "12DF000266660004FEE1DEAD23330003C0ED91".ToBin();
            var col = new TlvConvertibleCollection();
            col.ParseFrom(dat);
            var p1 = col.Get<TlvTestConvertibleA>();
            var p2 = col.Get<TlvTestConvertibleB>();
            Assert.AreEqual(p1.TestField, 0xFEE1DEAD);
            Assert.AreEqual(p2.A, 192);
            Assert.AreEqual(p2.B, 60817);
        }
    }
}