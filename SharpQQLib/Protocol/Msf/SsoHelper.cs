using System;
using System.Collections.ObjectModel;
using System.Linq;
using SharpQQ.Binarizer;
using SharpQQ.Protocol.Msf.Packets;

namespace SharpQQ.Protocol.Msf
{
    public static class SsoHelper
    {
        private static readonly ReadOnlyCollection<string> NoEncryptionOperations = new ReadOnlyCollection<string>(
            new[]
            {
                "heartbeat.ping",
                "heartbeat.alive",
                "client.correcttime"
            });

        private static readonly ReadOnlyCollection<string> ZeroEncryptionOperations = new ReadOnlyCollection<string>(
            new[]
            {
                "login.auth",
                "wtlogin.login",
                "login.chguin",
                "grayuinpro.check",
                "wtlogin.name2uin",
                "wtlogin.exchange_emp",
                "wtlogin.trans_emp",
                "account.requestverifywtlogin_emp",
                "account.requestrebindmblwtLogin_emp",
                "connauthsvr.get_app_info_emp",
                "connauthsvr.get_auth_api_list_emp",
                "connauthsvr.sdk_auth_api_emp"
            });

        public static byte[] EncodeRequest(int sequence, long qqNumber, string operationName, byte[] payload,
            byte[] cookie, MsfGeneralInfo msfInfo, AccountAuthInfo auth = null)
        {
            var head = new RequestHead
            {
                Sequence = sequence,
                AppId = msfInfo.AppId,
                NetworkType = 1,
                A2 = auth?.A2 ?? new byte[0],
                OperationName = operationName,
                Cookie = cookie,
                IMEI = msfInfo.IMEI,
                IMSIRevision = msfInfo.IMSIRevision,
                KSID = msfInfo.KSID
            };

            var content = new SsoRequestContent
            {
                Header = head,
                Payload = payload
            }.GetBinary();
            var req = new SsoRequest() {QQNumber = qqNumber.ToString()};
            if (NoEncryptionOperations.Contains(operationName.ToLowerInvariant()))
            {
                req.EncryptionType = SsoEncryptionType.NotEncrypted;
                req.D2 = new byte[0];
                req.EncryptedContent = content;
            }
            else if (ZeroEncryptionOperations.Contains(operationName.ToLowerInvariant()))
            {
                req.EncryptionType = SsoEncryptionType.EncryptedByZero;
                req.D2 = new byte[0];
                req.EncryptedContent = QSCrypt.Encrypt(content, Enumerable.Repeat((byte) 0, 16).ToArray());
            }
            else
            {
                if (auth == null)
                {
                    throw new ArgumentNullException("AuthInfo not set. No available key to encrypt.");
                }

                if (auth.QQNumber != qqNumber)
                {
                    throw new ArgumentException("AuthInfo QQ number mismatch.");
                }

                req.EncryptionType = SsoEncryptionType.EncryptedByKey;
                req.D2 = auth.D2;
                req.EncryptedContent = QSCrypt.Encrypt(content, auth.EncryptKey);
            }

            return req.GetBinary();
        }

        public static SsoResponseContent DecodeResponse(byte[] rawResponse, AccountAuthInfo auth)
        {
            var response = new SsoResponse();
            response.ParseFrom(new BinaryBufferReader(rawResponse));

            byte[] content;
            switch (response.EncryptionType)
            {
                case SsoEncryptionType.NotEncrypted:
                    content = response.EncryptedContent;
                    break;
                case SsoEncryptionType.EncryptedByZero:
                    content = QSCrypt.Decrypt(response.EncryptedContent, QSCrypt.ZeroKey);
                    break;
                case SsoEncryptionType.EncryptedByKey:
                    if (auth == null)
                    {
                        throw new ArgumentNullException("AuthInfo not set. No available key to decrypt.");
                    }
                    else if (auth.QQNumber.ToString() != response.QQNumber)
                    {
                        throw new ArgumentException("AuthInfo QQ number mismatch.");
                    }

                    content = QSCrypt.Decrypt(response.EncryptedContent, auth.EncryptKey);
                    break;
                default:
                    throw new ArgumentException($"Incorrect SsoEncryptionType: {response.EncryptionType}");
            }
            var responseContent = new SsoResponseContent();
            responseContent.ParseFrom(content);
            return responseContent;
        }
    }
}