using SharpQQ.Protocol.Msf.Packets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SharpQQ.Protocol.Msf
{
    public class MsfTask
    {
        public int Sequence { get; set; }

        public string OperationName { get; set; }

        public byte[] Data { get; set; }

        public TaskCompletionSource<MsfResult> CompletionSource { get; set; }
    }
    
    public class MsfResult
    {
        public int ReturnCode { get; set; }

        public string ErrorMessage { get; set; }

        public byte[] ResponsePayload { get; set; }
    }
}