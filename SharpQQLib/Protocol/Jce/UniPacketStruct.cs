using System.Collections.Generic;

namespace SharpQQ.Protocol.Jce
{
    public class UniPacketStruct : JceStruct
    {
        [JceField(1, true)]
        public int Version { get; set; } = 3;

        [JceField(2, true)]
        public byte PacketType { get; set; } = 0;

        [JceField(3, true)]
        public int MessageType { get; set; } = 0;

        [JceField(4, true)]
        public int RequestId { get; set; } = 0;

        [JceField(5, true)]
        public string ServantName { get; set; }

        [JceField(6, true)]
        public string FuncName { get; set; }

        [JceField(7, true)]
        public byte[] Buffer { get; set; }

        [JceField(8, true)]
        public int Timeout { get; set; } = 0;

        [JceField(9, true)]
        public Dictionary<string, string> Context { get; set; }

        [JceField(10, true)]
        public Dictionary<string, string> Status { get; set; }
    }
}