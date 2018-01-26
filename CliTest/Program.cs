using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using SharpQQ.Protocol.Msf;
using SharpQQ.Service;
using SharpQQ.Utils;

namespace CliTest
{
    class Program
    {
        private static readonly byte[] KSID = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8};

        public static readonly MsfGeneralInfo GlobalMsfInfo = new MsfGeneralInfo()
        {
            AppId = 537051018,
            IMEI = "867981879491234",
            IMSIRevision = "460220500720615|A1.1.5.9114",
            KSID = KSID,
            NetworkType = 1
        };
        
        public static readonly MsfGeneralInfo GlobalMsfInfo1 = new MsfGeneralInfo()
        {
            AppId = 537051018,
            IMEI = "237981444441234",
            IMSIRevision = "460244501999823|A1.1.5.9114",
            KSID = KSID,
            NetworkType = 1
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            AsyncContext.Run(TestQQ);
        }

        private static async Task TestQQ()
        {
            var rnd = new Random();
            long qqNumber = 2260128230;
            byte[] password = BinaryUtils.ComputeMD5("qq.test123");
            byte[] deviceIdPlain = Encoding.ASCII.GetBytes("sssddd222");
            var server = new MsfServer(qqNumber, GlobalMsfInfo1);
            await server.ConnectAsync();
            try
            {
                var result = await LoginHelper.Login(server, qqNumber, password, new LoginHelper.DeviceInfo()
                {
                    DeviceIdentifier = deviceIdPlain,
                    DeviceModel = "huawei",
                    DeviceVendor = "Mate 10"
                }, PromptCaptcha);
                Console.WriteLine($"OK! User name: {result.NickName}");
            }
            catch (QQException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task<string> PromptCaptcha(string promptText, byte[] jpegImage)
        {
            string imgFileName = Path.GetTempFileName() + ".jpg";
            Console.WriteLine(promptText);
            await File.WriteAllBytesAsync(imgFileName, jpegImage);
            Console.WriteLine($"Captcha image has been written to {imgFileName}");
            Console.Write("Please input captcha: ");
            return await Task.Run(() => Console.ReadLine());
        }
    }
}