namespace SharpQQ.Protocol.Msf
{
    public class MsfResult
    {
        public int ReturnCode { get; set; }

        public string ErrorMessage { get; set; }

        public byte[] ResponsePayload { get; set; }
    }
}