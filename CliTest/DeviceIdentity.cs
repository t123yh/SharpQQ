using System;
using System.Threading.Tasks;

namespace CliTest
{
    public class DeviceIdentity
    {
        public string IMEI { get; set; }
        public string IMSI { get; set; }
        public string AndroidVersion { get; set; }
        public string NetworkOperator { get; set; }
        public string DeviceId { get; set; }

        public static async Task<DeviceIdentity> AskForInfo()
        {
            var instance = new DeviceIdentity();
            
            Console.WriteLine("Device information can be generated here: http://www.myfakeinfo.com/mobile/get-android-device-information.php");
            instance.IMEI = (await Utils.PromptUntil("IMEI: ", Utils.ValidateIMEI, "Invalid IMEI, please generate one")).Trim();
            instance.IMSI = (await Utils.PromptUntil("IMSI (You'd better use a Chinese IMSI): ", Utils.ValidateIMSI)).Trim();

            instance.AndroidVersion = (await Utils.PromptUntil("Android version (e.g. 6.0.0): ", Utils.ValidateAndroidVersion)).Trim();
            instance.NetworkOperator = (await Utils.Prompt("Network operator (e.g. 中国电信): "));
            
            byte[] deviceId = new byte[16];
            Utils.GlobalRandom.NextBytes(deviceId);
            instance.DeviceId = SharpQQ.Utils.BinaryUtils.ToHex(deviceId);

            return instance;
        }
    }
}