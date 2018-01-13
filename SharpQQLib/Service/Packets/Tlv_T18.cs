using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;

namespace SharpQQ.Service.Packets
{
    // b.bs
    [TlvPacket(0x18)]
    public class Tlv_T18 : StructuredBinaryConvertible
    {
        public Tlv_T18(int qqNumber)
        {
            this.QQNumber = qqNumber;
        }

        [IntegerField(1)]
        public short This_i { get; set; } = 1;

        [IntegerField(2)]
        public int This_j { get; set; } = 1536;

        [IntegerField(3)]
        public int AppId { get; set; } = 16;

        [IntegerField(4)]
        public int Param_i { get; set; } = 0;

        [IntegerField(5)]
        public int QQNumber { get; set; }

        [IntegerField(6)]
        public short Param_i2 { get; set; } = 0;

        [IntegerField(7)]
        public short Unknown { get; set; } = 0;
    }
}