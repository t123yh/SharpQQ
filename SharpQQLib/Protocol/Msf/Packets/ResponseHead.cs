using SharpQQ.Binarizer.Structured;

namespace SharpQQ.Protocol.Msf.Packets
{
    public class ResponseHead : StructuredBinaryConvertible
    {
        [IntegerField(1)]
        public int Sequence { get; set; }

        [IntegerField(2)]
        public int ReturnCode { get; set; }

        [VariableLengthByteArrayField(3, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public string ErrorMessage { get; set; }

        [VariableLengthByteArrayField(4, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public string OperationName { get; set; }

        [VariableLengthByteArrayField(5, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public byte[] Cookie { get; set; }

        [IntegerField(6, optional: true)]
        public int Flag { get; set; }

        [IntegerField(7, optional: true)]
        public int ReservedField { get; set; }
    }
}