namespace SharpQQ.Service
{
    public class QQDeviceInfo
    {
        public string DeviceModel { get; set; }
        public string DeviceVendor { get; set; }
        public byte[] DeviceIdentifier { get; set; }
        public string NetworkOperator { get; set; }
        public string AndroidVersion { get; set; }
    }
}