using System;
using SharpQQ.Binarizer.Structured;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace SharpQQ.Protocol.Msf.Packets
{
    public enum SsoEncryptionType : byte
    {
        NotEncrypted = 0,
        EncryptedByKey = 1,
        EncryptedByZero = 2
    }

    public class SsoRequest : StructuredBinaryPacket
    {
        [IntegerField(1)]
        public int SsoVersion { get; set; } = 10;

        [IntegerField(2)]
        public SsoEncryptionType EncryptionType { get; set; }

        [VariableLengthByteArrayField(3, PrependLengthType.Int16BE, PrependLengthTransform.IncludeLengthField)]
        public byte[] D2 { get; set; }

        [IntegerField(4)]
        protected byte _AlwaysZero { get; set; } = 0;

        [VariableLengthByteArrayField(5, PrependLengthType.Int16BE, PrependLengthTransform.IncludeLengthField)]
        public string QQNumber { get; set; }

        [VariableLengthByteArrayField(6, PrependLengthType.None)]
        public byte[] EncryptedContent { get; set; }

        public override void Check()
        {
            if (this.SsoVersion != 10)
            {
                throw new Exception("Incorrect SsoVersion, should be 10.");
            }
        }
    }
}