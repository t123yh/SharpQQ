using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpQQ.Protocol.Msf;

namespace Tests
{
    [TestClass]
    public class MsfServerTest
    {
        private static readonly byte[] KSID = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 };

        private static readonly MsfGeneralInfo GlobalMsfInfo = new MsfGeneralInfo()
        {
            AppId = 537051018,
            IMEI = "867981879491101",
            IMSIRevision = "460110500720615|A1.1.5.9114",
            KSID = KSID,
            NetworkType = 1
        };
        

        [TestMethod]
        public async Task TestConnect()
        {
            var server = new MsfServer(2260128230, GlobalMsfInfo, null);
            await server.ConnectAsync();
        }
    }
}