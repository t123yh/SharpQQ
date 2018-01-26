using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;

namespace SharpQQ.Service.Packets
{
    [TlvPacketContent(0x105)]
    public class CaptchaImagePacket : StructuredBinaryConvertible
    {
        [VariableLengthByteArrayField(1, PrependLengthType.Int16BE, PrependLengthTransform.NoTransform)]
        public byte[] Token { get; set; }
        
        [VariableLengthByteArrayField(2, PrependLengthType.Int16BE, PrependLengthTransform.NoTransform)]
        public byte[] ImageJpeg { get; set; }
    }
}