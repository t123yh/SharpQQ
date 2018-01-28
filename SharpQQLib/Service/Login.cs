using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SharpQQ.Binarizer;
using SharpQQ.Binarizer.Tlv;
using SharpQQ.Protocol.Msf;
using SharpQQ.Service.Packets;
using SharpQQ.Service.Packets.Login;
using SharpQQ.Utils;

namespace SharpQQ.Service
{
    public delegate Task<string> PromptCaptcha(string promptText, byte[] jpegImage);

    public static class LoginHelper
    {
        public struct DeviceInfo
        {
            public string DeviceModel;
            public string DeviceVendor;
            public byte[] DeviceIdentifier;
            public string NetworkOperator;
            public string AndroidVersion;
        }

        private class QQKey
        {
            private AsymmetricCipherKeyPair KeyPair { get; }
            private IBasicAgreement Agreement { get; }

            public byte[] PublicKeyData => ((ECPublicKeyParameters) KeyPair.Public).Q.GetEncoded();

            public QQKey()
            {
                var ecparam = SecNamedCurves.GetByOid(SecObjectIdentifiers.SecP192k1);
                var domain = new ECDomainParameters(ecparam.Curve, ecparam.G, ecparam.N);
                var rnd = new SecureRandom();
                rnd.SetSeed(MiscellaneousUtils.UnifiedRandomBytes(10));
                var gen = new ECKeyPairGenerator();
                gen.Init(new ECKeyGenerationParameters(domain, rnd));
                this.KeyPair = gen.GenerateKeyPair();
                this.Agreement = AgreementUtilities.GetBasicAgreement("ECDH");
                Agreement.Init(this.KeyPair.Private);
            }

            public byte[] CalculateQSKey(byte[] peerPublicKey)
            {
                var publicKey = PublicKeyFactory.CreateKey(peerPublicKey);
                var sharedSecret = Agreement.CalculateAgreement(publicKey);
                return sharedSecret.ToByteArrayUnsigned().ComputeMD5();
            }
        }

        private static string ExtractCaptchaPrompt(byte[] raw)
        {
            const string expectedName = "pic_reason";
            var reader = new BinaryBufferReader(raw);
            try
            {
                int fieldCount = reader.ReadInt32(Endianness.Big);
                for (int i = 0; i < fieldCount; i++)
                {
                    byte nameLen = reader.ReadByte();
                    string name = Encoding.ASCII.GetString(reader.ReadByteArray(nameLen).ToArray());
                    int contentLen = reader.ReadInt32(Endianness.Big);
                    string content = Encoding.UTF8.GetString(reader.ReadByteArray(contentLen).ToArray());
                    if (name == expectedName)
                    {
                        return content;
                    }
                }
            }
            catch (BinarizerException)
            {
            }

            return "请输入验证码";
        }

        private static (TlvConvertibleCollection, byte[]) EncodeLoginRequest(long qqNumber, string IMEI,
            byte[] KSID,
            byte[] passwordMD5, DeviceInfo deviceInfo)
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            var accountInfo =
                new AccountInfoPacket(qqNumber, passwordMD5, deviceInfo.DeviceIdentifier, (int) timestamp);
            var accountEncryptionKey = passwordMD5.Concat(MyBitConverter.GetBytesFromInt64(qqNumber, Endianness.Big))
                .ToArray().ComputeMD5();
            var encryptedAccountInfo = QSCrypt.Encrypt(accountInfo.GetBinary(), accountEncryptionKey);

            var environmentInfo = new EnvironmentInfoPacket()
            {
                BuildModelInfo = new BuildModelnfoPacket(deviceInfo.DeviceModel),
                DeviceInfo = new DeviceInfoPacket(
                    deviceInfo.DeviceModel,
                    deviceInfo.DeviceIdentifier.ComputeMD5(),
                    deviceInfo.DeviceVendor
                ),
                SystemInfo = new SystemInfoPacket(
                    deviceInfo.AndroidVersion,
                    deviceInfo.NetworkOperator
                )
            };
            var tlvCollection = new TlvConvertibleCollection(0x9)
            {
                new Tlv_T18((int) qqNumber),
                new Tlv_T1((int) qqNumber, (int) timestamp),
                {262, encryptedAccountInfo},
                new Tlv_T116(),
                {256, "000100000005000000102002BF8A00000000021610E0".ToBin()},
                {263, "000000000001".ToBin()},
                {264, KSID},
                new ApkIdPacket(),
                {0x144, QSCrypt.Encrypt(environmentInfo.GetBinary(), accountInfo.TGTGTKey)},
                {325, IMEI.ComputeMD5()}
            };

            return (tlvCollection, accountInfo.TGTGTKey);
        }

        private static TlvConvertibleCollection EncodeCaptchaResult(byte[] contextToken, string captchaText,
            byte[] captchaToken)
        {
            return new TlvConvertibleCollection(0x2)
            {
                new CaptchaResultPacket() {CaptchaText = captchaText, CaptchaToken = captchaToken},
                {8, "0000000008040000".ToBin()},
                {260, contextToken},
                new Tlv_T116(),
            };
        }

        private static QQAccount DecodeLoginResponse(byte[] dataC)
        {
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

        private static async Task<TlvConvertibleCollection> DoRequest(MsfServer msf, long qqNumber,
            TlvConvertibleCollection data, QQKey key)
        {
            var qsKey = key.CalculateQSKey(Constants.QQServerKey);
            var tlvEncrypted = QSCrypt.Encrypt(data.GetBinary(), qsKey);
            var loginData = new LoginDataPacket
            {
                QQNumber = (int) qqNumber,
                PublicKey = key.PublicKeyData,
                Data = tlvEncrypted.Concat(new byte[] {3}).ToArray()
            };
            var loginDataBinary = loginData.GetBinary();

            var finalPayload = new byte[] {2}
                .Concat(MyBitConverter.GetBytesFromUInt16((ushort) (loginDataBinary.Length + 3), Endianness.Big))
                .Concat(loginDataBinary).ToArray();

            var loginResponse = await msf.DoRequest("wtlogin.login", finalPayload);
            var responseData = loginResponse.ResponsePayload;

            const int startPosition = 16;
            var dataACipher = responseData.Skip(startPosition).Take(responseData.Length - startPosition - 1).ToArray();
            var dataA = new LoginResponseA();
            dataA.ParseFrom(QSCrypt.Decrypt(dataACipher, qsKey));
            string prepend = dataA.PeerKey.Length < 30
                ? "302E301006072A8648CE3D020106052B8104001F031A00"
                : "3046301006072A8648CE3D020106052B8104001F03320004";
            var peerRawKey = prepend.ToBin().Concat(dataA.PeerKey).ToArray();

            var keyB = key.CalculateQSKey(peerRawKey);
            byte[] dataB = QSCrypt.Decrypt(dataA.EncryptedData, keyB);
            var dataBCollection = new TlvConvertibleCollection();
            dataBCollection.ParseFrom(dataB.Skip(1).ToArray());
            return dataBCollection;
        }

        public static async Task<QQAccount> Login(MsfServer msf, long qqNumber, byte[] passwordMD5,
            DeviceInfo deviceInfo, PromptCaptcha promptCaptcha)
        {
            var encKey = new QQKey();

            var (loginPacket, tgtgtKey) =
                EncodeLoginRequest(qqNumber, msf.MsfInfo.IMEI, msf.MsfInfo.KSID, passwordMD5, deviceInfo);

            var nextReqPacket = loginPacket;

            byte[] dataC;

            while (true)
            {
                var dataBCollection = await DoRequest(msf, qqNumber, nextReqPacket, encKey);

                const short dataCTag = 281, contextTokenTag = 260, captchaReasonTag = 357;
                if (dataBCollection.TryGetValue(dataCTag, out var dataCCipher))
                {
                    dataC = QSCrypt.Decrypt(dataCCipher, tgtgtKey);
                    break;
                }
                else if (dataBCollection.TryGet(out ErrorMessagePacket err)) // Login Error
                {
                    throw new LoginException(err.Message, err.ErrorType);
                }
                else if (dataBCollection.TryGet(out CaptchaImagePacket captchaImage)) // Requires CAPTCHA
                {
                    var contextToken = dataBCollection[contextTokenTag];
                    var promptText = ExtractCaptchaPrompt(dataBCollection[captchaReasonTag]);
                    if (promptCaptcha != null)
                    {
                        var captchaText = await promptCaptcha(promptText, captchaImage.ImageJpeg);
                        nextReqPacket = EncodeCaptchaResult(contextToken, captchaText, captchaImage.Token);
                        continue;
                    }
                    else
                    {
                        throw new QQException("Captcha required, but no captcha prompt method provided.");
                    }
                }
                else
                {
                    throw new QQException("Unknown error during login.");
                }
            }

            var account = DecodeLoginResponse(new byte[2] {0, 0}.Concat(dataC).ToArray());
            return account;
        }
    }
}