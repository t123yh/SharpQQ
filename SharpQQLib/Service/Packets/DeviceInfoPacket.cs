using System.Text;
using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;

namespace SharpQQ.Service.Packets
{
    // oicq.wlogin_sdk.b.ag (TLV_T128)
    [TlvPacketContent(0x128)]
    public class DeviceInfoPacket : StructuredBinaryConvertible
    {
        public DeviceInfoPacket()
        {
        }

        public DeviceInfoPacket(string phoneModel, byte[] deviceId, string phoneVendor)
        {
            this.PhoneModel = phoneModel;
            this.DeviceIdentifier = deviceId;
            this.PhoneVendor = phoneVendor;
        }

        [IntegerField(1)]
        public short Constant1 { get; set; } = 0;

        [IntegerField(2)]
        public byte Constant2 { get; set; } = 0;

        [IntegerField(3)]
        public byte Constant3 { get; set; } = 1;

        [IntegerField(4)]
        public byte Constant4 { get; set; } = 0;

        [IntegerField(5)]
        public int Constant5 { get; set; } = 16777216;

        [VariableLengthByteArrayField(6, PrependLengthType.Int16BE, maxLength: 32)]
        public string PhoneModel { get; set; }

        [VariableLengthByteArrayField(7, PrependLengthType.Int16BE, maxLength: 16)]
        public byte[] DeviceIdentifier { get; set; }

        [VariableLengthByteArrayField(8, PrependLengthType.Int16BE, maxLength: 16)]
        public string PhoneVendor { get; set; }
    }
}