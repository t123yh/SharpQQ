using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;

namespace SharpQQ.Service.Packets
{
    [TlvPacketContent(2)]
    public class CaptchaResultPacket : StructuredBinaryConvertible
    {
        [IntegerField(1)]
        public short _Unknown1 { get; set; } = 0;
        
        [VariableLengthByteArrayField(2, PrependLengthType.Int16BE, PrependLengthTransform.NoTransform)]
        public string CaptchaText { get; set; }
        
        [VariableLengthByteArrayField(3, PrependLengthType.Int16BE, PrependLengthTransform.NoTransform)]
        public byte[] CaptchaToken { get; set; }
    }
}