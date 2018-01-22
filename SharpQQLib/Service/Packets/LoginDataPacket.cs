using System.Runtime.InteropServices;
using SharpQQ.Binarizer.Structured;
using SharpQQ.Utils;
using PrependLengthType = SharpQQ.Binarizer.Structured.PrependLengthType;

namespace SharpQQ.Service.Packets
{
    public class LoginDataPacket : StructuredBinaryConvertible
    {
        [IntegerField(1)]
        public ushort _Field1 { get; set; } = 8001;

        [IntegerField(2)]
        public ushort _Field2 { get; set; } = 2064;

        [IntegerField(3)]
        public ushort _Field3 { get; set; } = 1;
        
        [IntegerField(4)]
        public int QQNumber { get; set; }

        [IntegerField(5)]
        public byte _Field5 { get; set; } = 3;

        [IntegerField(6)]
        public byte _Field6 { get; set; } = 135;

        [IntegerField(7)]
        public byte _Field7 { get; set; } = 0;

        [IntegerField(8)]
        public int _Field8 { get; set; } = 2;

        [IntegerField(9)]
        public int _Field9 { get; set; } = 0;

        [IntegerField(10)]
        public int _Field10 { get; set; } = 0;
        
        // starts k
        [IntegerField(11)]
        public byte _Field11 { get; set; } = 1;

        [IntegerField(12)]
        public byte _Field12 { get; set; } = 1;

        // t_c
        [FixedLengthByteArrayField(13, length: 16)]
        public byte[] _Random { get; set; } = MiscellaneousUtils.UnifiedRandomBytes(16);

        [IntegerField(14)]
        public ushort _Field14 { get; set; } = 0x102;
        
        [VariableLengthByteArrayField(15, PrependLengthType.Int16BE, PrependLengthTransform.NoTransform)]
        public byte[] PublicKey { get; set; }
        
        [VariableLengthByteArrayField(16, PrependLengthType.None, PrependLengthTransform.NoTransform)]
        public byte[] Data { get; set; }
    }
}