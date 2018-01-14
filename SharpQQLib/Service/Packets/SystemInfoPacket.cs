using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;

namespace SharpQQ.Service.Packets
{
    // oicq.wlogin_sdk.b.ac (TLV_T124)
    [TlvPacketContent(0x124)]
    public class SystemInfoPacket : StructuredBinaryConvertible
    {
        [VariableLengthByteArrayField(1, PrependLengthType.Int16BE, maxLength: 16)]
        public string OS { get; set; } = Constants.OS;

        [VariableLengthByteArrayField(2, PrependLengthType.Int16BE, maxLength: 16)]
        public string OSVersion { get; set; } = Constants.AndroidVersion;

        [IntegerField(3)]
        public short NetworkType { get; set; } = 2;

        [VariableLengthByteArrayField(4, PrependLengthType.Int16BE, maxLength: 16)]
        public string Operator { get; set; } = Constants.Operator;

        [VariableLengthByteArrayField(5, PrependLengthType.Int16BE, maxLength: 32)]
        public byte[] Unknown1 { get; set; } = new byte[0];

        [VariableLengthByteArrayField(6, PrependLengthType.Int16BE, maxLength: 16)]
        public string APNString { get; set; } = "wifi";
    }
}