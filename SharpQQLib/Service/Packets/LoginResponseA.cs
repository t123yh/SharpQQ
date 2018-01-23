using SharpQQ.Binarizer.Structured;

namespace SharpQQ.Service.Packets
{
    public class LoginResponseA : StructuredBinaryConvertible
    {
        [VariableLengthByteArrayField(1, PrependLengthType.Int16BE, PrependLengthTransform.NoTransform)]
        public byte[] PeerKey { get; set; }
        
        [VariableLengthByteArrayField(2, PrependLengthType.None)]
        public byte[] EncryptedData { get; set; }
    }
}