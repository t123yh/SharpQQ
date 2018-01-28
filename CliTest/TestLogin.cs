using System;
using System.IO;
using System.Threading.Tasks;
using SharpQQ.Protocol.Msf;
using SharpQQ.Service;
using SharpQQ.Utils;

namespace CliTest
{
    public static class TestLogin
    {
        public static async Task<AccountToken> Login(MsfServer msf, byte[] passwordMD5, DeviceIdentity dev)
        {
            try
            {
                var result = await LoginHelper.Login(msf, msf.QQNumber, passwordMD5, dev.QQDeviceInfo, PromptCaptcha);
                Console.WriteLine($"OK! User name: {result.NickName}");
                return new AccountToken()
                {
                    A2 = result.Auth.A2.ToHex(),
                    D2 = result.Auth.D2.ToHex(),
                    EncryptKey = result.Auth.EncryptKey.ToHex(),
                    QQNumber = msf.QQNumber
                };
            }
            catch (QQException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        private static async Task<string> PromptCaptcha(string promptText, byte[] jpegImage)
        {
            string imgFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".jpg");
            Console.WriteLine(promptText);
            await File.WriteAllBytesAsync(imgFileName, jpegImage);
            Console.WriteLine($"Captcha image has been written to {imgFileName}");
            Console.Write("Please input captcha: ");
            return await Task.Run(() => Console.ReadLine());
        }
    }
}