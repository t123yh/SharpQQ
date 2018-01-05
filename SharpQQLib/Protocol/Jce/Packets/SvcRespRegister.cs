using SharpQQ.Protocol.Jce;

namespace SharpQQ.Protocol.Jce.Packets
{
    public class SvcRespRegister
    {
        [JceField(0, true)]
        public long Uin { get; set; }

        [JceField(1, true)]
        public long Bid { get; set; }

        [JceField(2, true)]
        public byte ReplyCode { get; set; }

        [JceField(3, true)]
        public string Result { get; set; }

        [JceField(4, false)]
        public long? ServerTime { get; set; }

        [JceField(5, false)]
        public byte? LogQQ { get; set; }

        [JceField(6, false)]
        public byte? NeedKik { get; set; }

        [JceField(7, false)]
        public byte? UpdateFlag { get; set; }

        [JceField(8, false)]
        public long? TimeStamp { get; set; }

        [JceField(9, false)]
        public byte? CrashFlag { get; set; }

        [JceField(10, false)]
        public string ClientIP { get; set; }

        [JceField(11, false)]
        public int? ClientPort { get; set; }

        [JceField(12, false)]
        public int? HelloInterval { get; set; }

        [JceField(13, false)]
        public long? LargeSeq { get; set; }

        [JceField(14, false)]
        public byte LargeSeqUpdate { get; set; }
    }
}