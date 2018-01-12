using System.Text;
using SharpQQ.Binarizer.Structured;

namespace SharpQQ.Protocol.Msf.Packets
{
    public class RequestHead : StructuredBinaryConvertible
    {
        [IntegerField(1)]
        public int Sequence { get; set; }

        [IntegerField(2)]
        public int AppId { get; set; }

        [IntegerField(3)]
        protected int AppId2 { get { return this.AppId; } set { this.AppId = value; } }

        [IntegerField(4)]
        public byte NetworkType { get; set; }

        [FixedLengthByteArrayField(5, 11)]
        protected byte[] _Empty { get; set; } = new byte[11];

        [VariableLengthByteArrayField(6, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public byte[] A2 { get; set; }

        [VariableLengthByteArrayField(7, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public string OperationName { get; set; }

        [VariableLengthByteArrayField(8, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public byte[] Cookie { get; set; }

        [VariableLengthByteArrayField(9, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public string IMEI { get; set; }

        [VariableLengthByteArrayField(10, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public byte[] KSID { get; set; }

        [VariableLengthByteArrayField(11, PrependLengthType.Int16BE, PrependLengthTransform.IncludeLengthField)]
        public string IMSIRevision { get; set; }

        [VariableLengthByteArrayField(12, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        protected byte[] _EmptyField { get; set; } = new byte[0];
    }
}