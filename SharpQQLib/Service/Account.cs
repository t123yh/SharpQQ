using SharpQQ.Protocol.Msf;

namespace SharpQQ.Service
{
    public class QQAccount
    {
        public long QQNumber { get; set; }

        public AccountAuthInfo Auth { get; set; }  = new AccountAuthInfo();
        
        public string Nickname { get; set; }
    }
}