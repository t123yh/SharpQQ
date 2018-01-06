namespace SharpQQ.Protocol.Msf
{
    public class AccountAuthInfo
    {
        public long QQNumber { get; set; }
        
        public byte[] A2 { get; set; }

        public byte[] D2 { get; set; }

        public byte[] EncryptKey { get; set; }
    }
}