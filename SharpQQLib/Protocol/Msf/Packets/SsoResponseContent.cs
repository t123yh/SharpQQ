using SharpQQ.Binarizer.Structured;

namespace SharpQQ.Protocol.Msf.Packets
{
    public class SsoResponseContent : StructuredBinaryConvertible
    {
        [VariableLengthSubstructField(1, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public ResponseHead Header { get; set; }

        [VariableLengthByteArrayField(2, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public byte[] Payload { get; set; }
    }
}