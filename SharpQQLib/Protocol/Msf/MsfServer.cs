using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading;
using SharpQQ.Protocol.Msf.Packets;
using Nito.AsyncEx;

namespace SharpQQ.Protocol.Msf
{
    public class MsfServer
    {
        const string DefaultAddress = "msfxg.3g.qq.com";
        const int DefaultPort = 14000;

        static readonly ReadOnlyCollection<string> NoEncryptionOperations = new ReadOnlyCollection<string>(new string[]{
            "heartbeat.ping",
            "heartbeat.alive",
            "client.correcttime"});

        static readonly ReadOnlyCollection<string> ZeroEncryptionOperations = new ReadOnlyCollection<string>(new string[]{
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
            "connauthsvr.sdk_auth_api_emp"});


        public bool Connected
        {
            get
            {
                return this._baseClient != null && this._baseClient.Connected;
            }
        }

        Dictionary<int, MsfTask> _taskList = new Dictionary<int, MsfTask>();
        private TcpClient _baseClient;
        private CancellationToken _globalCancellationToken;
        private AsyncProducerConsumerQueue<MsfTask> _waitingTasks = new AsyncProducerConsumerQueue<MsfTask>();

        public long QQNumber { get; }

        public AccountAuthInfo AuthInfo { get; set; }

        public MsfGeneralInfo MsfInfo { get; }

        public byte[] CurrentCookie { get; private set; }

        public MsfServer(long qqNumber, MsfGeneralInfo msfInfo, AccountAuthInfo auth = null)
        {
            this.QQNumber = qqNumber;
            this.AuthInfo = auth;
            this.MsfInfo = msfInfo;
        }

        #region Sequence
        private int _nextSequence = 10000;
        public int IncreaseSequence()
        {
            return Interlocked.Increment(ref _nextSequence) - 1;
        }
        #endregion

        public Task<MsfResult> DoRequest(string opName, byte[] data)
        {
            MsfTask task = new MsfTask();
            task.Sequence = this.IncreaseSequence();
            task.CompletionSource = new TaskCompletionSource<MsfResult>();
            task.OperationName = opName;
            task.Data = data;
            this._waitingTasks.EnqueueAsync(task);
            return task.CompletionSource.Task;
            /*
             */
        }

        private byte[] EncodeRequest(MsfTask task)
        {
            RequestHead head = new RequestHead();
            head.Sequence = task.Sequence;
            head.AppId = this.MsfInfo.AppId;
            head.NetworkType = this.MsfInfo.NetworkType;
            if (this.AuthInfo?.A2 != null)
                head.A2 = this.AuthInfo.A2;
            else
                head.A2 = new byte[0];

            head.OperationName = task.OperationName;
            head.Cookie = this.CurrentCookie;
            head.IMEI = this.MsfInfo.IMEI;
            head.IMSIRevision = this.MsfInfo.IMSIRevision;
           head.KSID = this.MsfInfo.KSID;
            SsoRequestContent content = new SsoRequestContent();
            content.Head = head;
            content.Payload = task.Data;
            SsoRequest req = new SsoRequest();
            if (NoEncryptionOperations.Contains(task.OperationName))
            {
                req.EncryptionType = SsoEncryptionType.NotEncrypted;
                req.D2 = new byte[0];
                req.EncryptedContent = content.GetBinary();
                req.
            }
        }

        private async Task SendLoop()
        {
            while (!_globalCancellationToken.IsCancellationRequested)
            {
                if (!this.Connected)
                {
                    await this.ConnectAsync();
                }
                try
                {
                    var task = await this._waitingTasks.DequeueAsync();

                }
                catch (Exception e)
                {

                }
            }
        }

        private async Task ConnectAsync()
        {
            if (this._baseClient != null)
            {
                this._baseClient.Dispose();
            }
            this._baseClient = new TcpClient();
            await this._baseClient.ConnectAsync(DefaultAddress, DefaultPort);
            this.CurrentCookie = Utils.BinaryUtils.UnifiedRandomBytes(4);
        }
    }
}