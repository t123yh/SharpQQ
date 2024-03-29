using System;
using SharpQQ.Binarizer.Structured;

namespace SharpQQ.Protocol.Msf.Packets
{
    public class SsoResponse : StructuredBinaryConvertible
    {
        public override void Check()
        {
            if (this.SsoVersion != 10)
            {
                throw new Exception($"Incorrect SsoVersion {this.SsoVersion}, should be 10.");
            }
        }

        [IntegerField(1)]
        public int SsoVersion { get; set; } = 10;

        [IntegerField(2)]
        public SsoEncryptionType EncryptionType { get; set; }

        [IntegerField(3)]
        protected byte _AlwaysZero { get; set; }

        [VariableLengthByteArrayField(4, PrependLengthType.Int32BE, PrependLengthTransform.IncludeLengthField)]
        public string QQNumber { get; set; }

        [VariableLengthByteArrayField(5, PrependLengthType.None)]
        public byte[] EncryptedContent { get; set; }
    }
}