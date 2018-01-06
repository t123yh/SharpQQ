using System.Text;
using SharpQQ.Binarizer.Structured;

namespace SharpQQ.Protocol.Msf.Packets
{
    public class SsoRequestContent : StructuredBinaryPacket
    {
        [VariableLengthSubstructField(1, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public RequestHead Header { get; set; }

        [VariableLengthByteArrayField(2, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public byte[] Payload { get; set; }
    }
}