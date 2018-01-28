using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;

namespace SharpQQ.Service.Packets.Login
{
    [TlvPacketContent(0x16e)]
    public class BuildModelnfoPacket : StructuredBinaryConvertible
    {
        public BuildModelnfoPacket()
        {
        }

        public BuildModelnfoPacket(string model)
        {
            this.BuildModel = model;
        }

        [VariableLengthByteArrayField(1, PrependLengthType.Int16BE)]
        public string BuildModel { get; set; }
    }
}