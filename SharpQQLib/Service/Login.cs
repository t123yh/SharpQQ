using System;
using System.Linq;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SharpQQ.Binarizer;
using SharpQQ.Binarizer.Structured;
using SharpQQ.Binarizer.Tlv;
using SharpQQ.Protocol.Msf;
using SharpQQ.Service.Packets;
using SharpQQ.Utils;

namespace SharpQQ.Service
{
    public static class LoginHelper
    {
        public struct DeviceInfo
        {
            public string DeviceModel;
            public string DeviceVendor;
            public byte[] DeviceIdentifier;
        }

        public static (byte[], byte[], byte[], IBasicAgreement) GenerateLoginRequest(long qqNumber, string IMEI, byte[] KSID,
            byte[] passwordMD5, DeviceInfo deviceInfo)
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            // ------------------------------------
            // Create login TLV collection
            var accountInfo =
                new AccountInfoPacket(qqNumber, passwordMD5, deviceInfo.DeviceIdentifier, (int) timestamp);
            var accountEncryptionKey = passwordMD5.Concat(MyBitConverter.GetBytesFromInt64(qqNumber, Endianness.Big))
                .ToArray().ComputeMD5();
            var encryptedAccountInfo = QSCrypt.Encrypt(accountInfo.GetBinary(), accountEncryptionKey);

            var environmentInfo = new EnvironmentInfoPacket()
            {
                BuildModelInfo = new BuildModelnfoPacket(deviceInfo.DeviceModel),
                DeviceInfo =
                    new DeviceInfoPacket(deviceInfo.DeviceModel, BinaryUtils.ComputeMD5(deviceInfo.DeviceIdentifier),
                        deviceInfo.DeviceVendor),
                SystemInfo = new SystemInfoPacket()
            };
            var tlvCollection = new TlvConvertibleCollection(0x9)
            {
                new Tlv_T18((int) qqNumber),
                new Tlv_T1((int) qqNumber, (int) timestamp),
                {262, encryptedAccountInfo},
                {278, "000017FF7C00010400015F5E10E2".ToBin()},
                {256, "000100000005000000102002BF8A00000000021610E0".ToBin()},
                {263, "000000000001".ToBin()},
                {264, KSID},
                new ApkIdPacket(),
                {0x144, QSCrypt.Encrypt(environmentInfo.GetBinary(), accountInfo.TGTGTKey)},
                {325, IMEI.ComputeMD5()}
            };

            // ------------------------------------
            // Key generation
            var ecparam = SecNamedCurves.GetByOid(SecObjectIdentifiers.SecP192k1);
            var domain = new ECDomainParameters(ecparam.Curve, ecparam.G, ecparam.N);
            var rnd = new SecureRandom();
            rnd.SetSeed(MiscellaneousUtils.UnifiedRandomBytes(10));
            var gen = new ECKeyPairGenerator();
            gen.Init(new ECKeyGenerationParameters(domain, rnd));
            var myKeyPair = gen.GenerateKeyPair();
            var qqPublicKey = PublicKeyFactory.CreateKey(Constants.QQServerKey);
            var agreement = AgreementUtilities.GetBasicAgreement("ECDH");
            agreement.Init(myKeyPair.Private);
            var sharedSecret = agreement.CalculateAgreement(qqPublicKey);
            var qsKey = sharedSecret.ToByteArrayUnsigned().ComputeMD5();
            var myPublicKeyData = ((ECPublicKeyParameters) myKeyPair.Public).Q.GetEncoded();


            // ------------------------------------
            // Final packing
            var tlvEncrypted = QSCrypt.Encrypt(tlvCollection.GetBinary(), qsKey);
            var loginData = new LoginDataPacket
            {
                QQNumber = (int) qqNumber,
                PublicKey = myPublicKeyData,
                Data = tlvEncrypted.Concat(new byte[] {3}).ToArray()
            };
            var loginDataBinary = loginData.GetBinary();

            var finalPayload = new byte[] {2}
                .Concat(MyBitConverter.GetBytesFromUInt16((ushort) (loginDataBinary.Length + 3), Endianness.Big))
                .Concat(loginDataBinary).ToArray();

            return ( finalPayload, qsKey, accountInfo.TGTGTKey, agreement );
        }

        public static QQAccount DecodeLoginResponse(byte[] response, byte[] qsKey, byte[] tgtgtKey, IBasicAgreement agreement)
        {
            const int startPosition = 16;
            var dataACipher = response.Skip(startPosition).Take(response.Length - startPosition - 1).ToArray();
            var dataA = new LoginResponseA();
            dataA.ParseFrom(QSCrypt.Decrypt(dataACipher, qsKey));
            string prepend = dataA.PeerKey.Length < 30 ? "302E301006072A8648CE3D020106052B8104001F031A00" : "3046301006072A8648CE3D020106052B8104001F03320004";
            var peerRawKey = prepend.ToBin().Concat(dataA.PeerKey).ToArray();
            
            var sharedSecret2 = agreement.CalculateAgreement(PublicKeyFactory.CreateKey(peerRawKey)).ToByteArrayUnsigned();
            var keyB = sharedSecret2.ComputeMD5();
            byte[] dataB = QSCrypt.Decrypt(dataA.EncryptedData, keyB);
            var dataBCollection = new TlvConvertibleCollection();
            dataBCollection.ParseFrom(dataB.Skip(1).ToArray());

            const short dataCTag = 281;
            if (!dataBCollection.TryGetValue(dataCTag, out var dataCCipher))
            {
                if (dataBCollection.TryGet(out ErrorMessagePacket err))
                {
                    throw new LoginException(err.Message, err.ErrorType);
                }
                else
                {
                    throw new QQException("Unknown error during login.");
                }
            }
            var dataC = QSCrypt.Decrypt(dataCCipher, tgtgtKey);
            var dataCCollection = new TlvConvertibleCollection();
            dataCCollection.ParseFrom(dataC);

            const short A2Tag = 266;
            const short D2Tag = 323;
            const short KeyTag = 773;
            var userInfo = dataCCollection.Get<UserInfoPacket>();
            
            var account = new QQAccount
            {
                Auth =
                {
                    A2 = dataCCollection[A2Tag],
                    D2 = dataCCollection[D2Tag],
                    EncryptKey = dataCCollection[KeyTag]
                },
                NickName = userInfo.Nickname
            };

            return account;
        }

        public static async Task<QQAccount> Login(MsfServer msf, long qqNumber, byte[] passwordMD5,
            DeviceInfo deviceInfo)
        {
            var (reqPacket, qsKey, tgtgtKey, agreement) =
                GenerateLoginRequest(qqNumber, msf.MsfInfo.IMEI, msf.MsfInfo.KSID, passwordMD5, deviceInfo);
            Console.WriteLine(reqPacket.ToHex());
            var loginResponse = await msf.DoRequest("wtlogin.login", reqPacket);
            var account = DecodeLoginResponse(loginResponse.ResponsePayload, qsKey, tgtgtKey, agreement);
            return account;
        }
    }
}