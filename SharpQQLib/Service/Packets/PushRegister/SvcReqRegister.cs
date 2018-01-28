using SharpQQ.Protocol.Jce;

namespace SharpQQ.Service.Packets.PushRegister
{
    public class SvcReqRegister : JceStruct
    {
        [JceField(0, true)]
        public long Uin { get; set; }

        [JceField(1, true)]
        public long Bid { get; set; }

        [JceField(2, true)]
        public byte ConnType { get; set; }

        [JceField(3, true)]
        public string Other { get; set; }

        [JceField(4, false)]
        public int? Status { get; set; } = 11;

        [JceField(5, false)]
        public byte? OnlinePush { get; set; }

        [JceField(6, false)]
        public byte? IsOnline { get; set; }

        [JceField(7, false)]
        public byte? ShowOnline { get; set; }

        [JceField(8, false)]
        public byte? KikPC { get; set; }

        [JceField(9, false)]
        public byte? KikWeak { get; set; }

        [JceField(10, false)]
        public long? Timestamp { get; set; }

        [JceField(11, false)]
        public long? OSVersion { get; set; }

        [JceField(12, false)]
        public long? NetType { get; set; }

        [JceField(13, false)]
        public string BuildVer { get; set; }

        [JceField(14, false)]
        public byte? RegType { get; set; }

        [JceField(15, false)]
        public byte[] DevParam { get; set; }

        [JceField(16, false)]
        public byte[] DeviceId { get; set; }

        [JceField(17, false)]
        public int? LocaleId { get; set; }

        [JceField(18, false)]
        public byte? SlientPush { get; set; }

        [JceField(19, false)]
        public string DevName { get; set; }

        [JceField(20, false)]
        public string DevType { get; set; }

        [JceField(21, false)]
        public string OSVer { get; set; }

        [JceField(22, false)]
        public byte? OpenPush { get; set; }

        [JceField(23, false)]
        public int? LargeSeq { get; set; }

        [JceField(24, false)]
        public int? LastWatchStartTime { get; set; }

        [JceField(25, false)]
        public byte[] BindUin { get; set; }

        [JceField(26, false)]
        public long? OldSSOIP { get; set; }

        [JceField(27, false)]
        public long? NewSSOIP { get; set; }
    }
}