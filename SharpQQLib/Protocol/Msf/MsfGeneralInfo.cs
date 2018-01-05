namespace SharpQQ.Protocol.Msf
{
    public class MsfGeneralInfo
    {
        public int AppId { get; set; } = Constants.AppId;

        public byte NetworkType { get; set; } = 1;

        public string IMEI { get; set; }

        public string IMSIRevision { get; set; }

        public byte[] KSID { get; set; }
    }
}