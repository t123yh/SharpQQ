using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;
using Nito.AsyncEx;
using Org.BouncyCastle.Security;
using SharpQQ.Protocol.Msf;
using SharpQQ.Service;
using SharpQQ.Utils;

namespace CliTest
{
    class Program
    {
        private static readonly byte[] KSID = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8};

        static void Main(string[] args)
        {
            AsyncContext.Run(TestQQ);
        }

        private static async Task TestQQ()
        {
            string deviceIdFile = "device-identity.dat.yml";
            string accountTokenFile = "account-token.dat.yml";
            bool showHelp = false;

            var options = new OptionSet
            {
                {"d|device-identity=", "Device Identity File", (file) => deviceIdFile = file},
                {"a|account-token=", "Account Token File", (file) => accountTokenFile = file},
                {"h|help", "Show this message and exit", h => showHelp = h != null},
            };
            if (showHelp)
            {
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            MsfServer msf = null;
            var deviceIdentity = await ConfigLoader.LoadConfig(deviceIdFile, DeviceIdentity.AskForInfo);
            var accountToken = await ConfigLoader.LoadConfig(accountTokenFile, async () =>
            {
                long qqNumber = long.Parse(await Utils.Prompt("QQ Number: "));
                byte[] password = (await Utils.Prompt("Password: ")).ComputeMD5();
                msf = await InitializeMsfServer(deviceIdentity, qqNumber);
                return await TestLogin.Login(msf, password, deviceIdentity);
            });
            
            // Found accountToken, Msf Server not loaded
            if (msf == null)
                msf = await InitializeMsfServer(deviceIdentity, accountToken.QQNumber);

            msf.AuthInfo = new AccountAuthInfo()
            {
                A2 = accountToken.A2.ToBin(),
                D2 = accountToken.D2.ToBin(),
                EncryptKey = accountToken.EncryptKey.ToBin(),
                QQNumber = accountToken.QQNumber
            };
        }

        private static async Task<MsfServer> InitializeMsfServer(DeviceIdentity dev, long qqNumber)
        {
            var ksid = new byte[16];
            Utils.GlobalRandom.NextBytes(ksid);
            var msfInfo = new MsfGeneralInfo()
            {
                AppId = SharpQQ.Constants.AppId,
                IMEI = dev.IMEI,
                IMSIRevision = dev.IMSI + "|A1.1.5.9114",
                KSID = ksid,
            };
            var server = new MsfServer(qqNumber, msfInfo);
            server.ConnectionFailed += async delegate
            {
                int failCount = 0;
                while (true)
                {
                    int delay = (int) (Math.Pow(1.5, failCount) * 100);
                    Console.WriteLine($"Connection failure, retry after {delay / 1000.0}s");
                    await Task.Delay(delay);
                    try
                    {
                        await server.ConnectAsync();
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unable to connect to server: {ex.Message}");
                        failCount++;
                    }
                }
            };
            await server.ConnectAsync();
            return server;
        }

    }
}