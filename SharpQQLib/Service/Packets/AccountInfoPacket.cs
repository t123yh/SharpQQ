using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;
using SharpQQ.Utils;
using PrependLengthType = SharpQQ.Binarizer.Structured.PrependLengthType;

namespace SharpQQ.Service.Packets
{
    // oicq.wlogin_sdk.b.i
    public class AccountInfoPacket : StructuredBinaryConvertible
    {
        public AccountInfoPacket(long qqNumber, byte[] passwordMD5, byte[] deviceId, int timestamp)
        {
            this.QQNumber = qqNumber;
            this.PasswordMD5 = passwordMD5;
            this.DeviceIdentifier = deviceId;
            this.QQNumberStr = qqNumber.ToString();
            this.Timestamp = timestamp;
        }

        [IntegerField(1)]
        public short This_a { get; set; } = 4; // this.a

        [IntegerField(2)]
        public int Random1 { get; set; } = MiscellaneousUtils.UnifiedRandomInt();

        [IntegerField(3)]
        public int This_i { get; set; } = 5; // this.i

        [IntegerField(4)]
        public int Param_j { get; set; } = 16; // param.j

        [IntegerField(5)]
        public int Param_i { get; set; } = 0; // param.i

        [IntegerField(6)]
        public long QQNumber { get; set; }

        [IntegerField(7)]
        public int Timestamp { get; set; } // param.bArr

        [FixedLengthByteArrayField(8, length: 4)]
        public byte[] Param_bArr2 { get; set; } = new byte[4]; //param.bArr2

        [IntegerField(9)]
        public byte Param_i2 { get; set; } = 1;

        [FixedLengthByteArrayField(10, length: 16)]
        public byte[] PasswordMD5 { get; set; }

        [FixedLengthByteArrayField(11, length: 16)]
        public byte[] TGTGTKey { get; set; } = MiscellaneousUtils.UnifiedRandomBytes(16);

        [IntegerField(12)]
        public int Unknown1 { get; set; } = 0;

        [IntegerField(13)]
        public byte Param_i3 { get; set; } = 1;

        [FixedLengthByteArrayField(14, length: 16)]
        public byte[] DeviceIdentifier { get; set; }

        [IntegerField(15)]
        public int AppVersion { get; set; } = Constants.AppId;

        [IntegerField(16)]
        public int Param_i4 { get; set; } = 1;

        [VariableLengthByteArrayField(17, PrependLengthType.Int16BE)]
        public string QQNumberStr { get; set; }
    }
}