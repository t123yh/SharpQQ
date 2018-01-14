using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;

namespace SharpQQ.Service.Packets
{
    [TlvPacketContent(0x144)]
    public class EnvironmentInfoPacket : StructuredBinaryConvertible
    {
        [IntegerField(1)]
        public ushort FieldCount { get; set; } = 3;

        [SelfReadSubstructField(2)]
        public TlvPacket<SystemInfoPacket> SystemInfo { get; set; }

        [SelfReadSubstructField(3)]
        public TlvPacket<DeviceInfoPacket> DeviceInfo { get; set; }

        [SelfReadSubstructField(4)]
        public TlvPacket<BuildModelnfoPacket> BuildModelInfo { get; set; }
    }
}