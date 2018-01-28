using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;

namespace SharpQQ.Service.Packets.Login
{
    [TlvPacketContent(326)]
    public class ErrorMessagePacket : StructuredBinaryConvertible
    {
        [IntegerField(1)]
        public int Unknown1 { get; set; }

        [VariableLengthByteArrayField(2, PrependLengthType.Int16BE)]
        public string Title { get; set; }

        [VariableLengthByteArrayField(3, PrependLengthType.Int16BE)]
        public string Message { get; set; }

        [IntegerField(4)]
        public short ErrorType { get; set; }

        [VariableLengthByteArrayField(5, PrependLengthType.Int16BE)]
        public byte[] OtherInfo { get; set; }
    }
}