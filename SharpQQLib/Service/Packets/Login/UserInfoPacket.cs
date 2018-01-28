using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;

namespace SharpQQ.Service.Packets.Login
{
    [TlvPacketContent(282)]
    public class UserInfoPacket : StructuredBinaryConvertible
    {
        [IntegerField(1)]
        public short Face { get; set; }
        
        [IntegerField(2)]
        public byte Age { get; set; }
        
        [IntegerField(3)]
        public byte Gender { get; set; }
        
        [VariableLengthByteArrayField(4, PrependLengthType.Int8, PrependLengthTransform.NoTransform)]
        public string Nickname { get; set; }
    }
}