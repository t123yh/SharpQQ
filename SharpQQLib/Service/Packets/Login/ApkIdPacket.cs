using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;

namespace SharpQQ.Service.Packets.Login
{
    [TlvPacketContent(322)]
    public class ApkIdPacket : StructuredBinaryConvertible
    {
        [IntegerField(1)]
        public short Unknown1 { get; set; } = 0;

        [VariableLengthByteArrayField(2, PrependLengthType.Int16BE)]
        public string AppId { get; set; } = Constants.PacketName;
    }
}