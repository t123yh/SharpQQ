using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;

namespace SharpQQ.Service.Packets
{
    [TlvPacketContent(0x16e)]
    public class BuildModelnfoPacket : StructuredBinaryConvertible
    {
        [VariableLengthByteArrayField(1, PrependLengthType.Int16BE)]
        public string BuildModel { get; set; }
    }
}