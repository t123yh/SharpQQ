using SharpQQ.Binarizer.Structured;

namespace SharpQQ.Protocol.Msf.Packets
{
    public class SsoResponseContent : StructuredBinaryPacket
    {
        [VariableLengthSubstructField(1, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public ResponseHead Header { get; set; }

        [VariableLengthSubstructField(2, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public byte[] Payload { get; set; }
    }
}