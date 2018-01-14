using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;
using SharpQQ.Utils;

namespace SharpQQ.Service.Packets
{
    [TlvPacketContent(1)]
    public class Tlv_T1 : StructuredBinaryConvertible
    {
        public Tlv_T1()
        {
        }

        public Tlv_T1(int qq, int timestamp)
        {
            this.QQNumber = qq;
            this.ServerTime = timestamp;
        }

        [IntegerField(1)]
        public short This_i { get; set; } = 1;

        [IntegerField(2)]
        public int Random1 { get; set; } = MiscellaneousUtils.UnifiedRandomInt();

        [IntegerField(3)]
        public int QQNumber { get; set; }

        [IntegerField(4)]
        public int ServerTime { get; set; }

        [FixedLengthByteArrayField(5, length: 4)]
        public byte[] Param_bArr { get; set; } = {0, 0, 0, 0};

        [IntegerField(6)]
        public short Unknown { get; set; } = 0;
    }
}