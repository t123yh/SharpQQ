using SharpQQ.Binarizer;
using SharpQQ.Binarizer.Tlv;
using SharpQQ.Utils;

namespace SharpQQ.Service.Packets.Login
{
    [TlvPacketContent(0x116)]
    public class Tlv_T116 : IBinaryConvertible
    {
        public void ParseFrom(BinaryBufferReader reader)
        {
            throw new System.NotImplementedException();
        }

        public void WriteTo(BinaryBufferWriter writer)
        {
            writer.WriteByteArray("000017FF7C00010400015F5E10E2".ToBin());
        }
    }
}