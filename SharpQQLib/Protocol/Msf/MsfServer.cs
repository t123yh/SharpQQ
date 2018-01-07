using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using SharpQQ.Protocol.Msf.Packets;
using Nito.AsyncEx;
using Org.BouncyCastle.Asn1.Cms;
using SharpQQ.Utils;

namespace SharpQQ.Protocol.Msf
{
    public class MsfServer
    {
        private const string DefaultAddress = "msfxg.3g.qq.com";
        private const int DefaultPort = 14000;

        public bool Connected => this._baseClient != null && this._baseClient.Connected;

        private readonly Dictionary<int, MsfTask> _taskList = new Dictionary<int, MsfTask>();
        private readonly AsyncManualResetEvent _connectedEvent;
        private TcpClient _baseClient;
        private readonly CancellationTokenSource _globalCancellationTokenSource;
        private readonly AsyncProducerConsumerQueue<MsfTask> _waitingTasks = new AsyncProducerConsumerQueue<MsfTask>();

        public event EventHandler ConnectionFailed;
        
        public long QQNumber { get; }
        public AccountAuthInfo AuthInfo { get; set; }
        public MsfGeneralInfo MsfInfo { get; }
        public byte[] CurrentCookie { get; private set; }

        public MsfServer(long qqNumber, MsfGeneralInfo msfInfo, AccountAuthInfo auth = null)
        {
            this.QQNumber = qqNumber;
            this.AuthInfo = auth;
            this.MsfInfo = msfInfo;
            this._globalCancellationTokenSource = new CancellationTokenSource();
            this._connectedEvent = new AsyncManualResetEvent(false);
        }

        #region Sequence

        private int _nextSequence = 10000;

        public int IncreaseSequence()
        {
            return Interlocked.Increment(ref _nextSequence) - 1;
        }

        #endregion

        public Task<MsfResult> DoRequest(string opName, byte[] data, int timeout = Timeout.Infinite)
        {
            var task = new MsfTask
            {
                Sequence = this.IncreaseSequence(),
                CompletionSource = new TaskCompletionSource<MsfResult>(),
                OperationName = opName,
                Data = data
            };
            this._waitingTasks.EnqueueAsync(task);
            _taskList.Add(task.Sequence, task);
            if (timeout != Timeout.Infinite)
            {
                var source = new CancellationTokenSource(timeout);
                source.Token.Register(() =>
                {
                    task.CompletionSource.TrySetCanceled(source.Token);
                    if (_taskList.ContainsKey(task.Sequence))
                        _taskList.Remove(task.Sequence);
                }, false);
            }

            return task.CompletionSource.Task;
        }


        private async Task SendLoop()
        {
            while (!_globalCancellationTokenSource.Token.IsCancellationRequested)
            {
                await _connectedEvent.WaitAsync(_globalCancellationTokenSource.Token);
                var task = await this._waitingTasks.DequeueAsync(_globalCancellationTokenSource.Token);

                try
                {
                    var rawData = SsoHelper.EncodeRequest(this.IncreaseSequence(), this.QQNumber, task.OperationName,
                        task.Data, this.CurrentCookie, this.MsfInfo, this.AuthInfo);
                    await this._baseClient.WritePacketAsync(rawData);
                }
                catch (IOException ex)
                {
                    // Network error occurred, push it to queue again.
                    await this._waitingTasks.EnqueueAsync(task, _globalCancellationTokenSource.Token);
                    HandleConnectionFailure();
                    continue;
                }
                catch (Exception ex) // Other exceptions: throw.
                {
                    task.CompletionSource.TrySetException(ex);
                    continue;
                }
            }
        }

        private async Task ReceiveLoop()
        {
            while (!_globalCancellationTokenSource.Token.IsCancellationRequested)
            {
                await _connectedEvent.WaitAsync(_globalCancellationTokenSource.Token);
                try
                {
                    var packet = await this._baseClient.ReadPacketAsync();
                    var content = SsoHelper.DecodeResponse(packet, this.AuthInfo);
                    if (_taskList.ContainsKey(content.Header.Sequence))
                    {
                        var result = new MsfResult()
                        {
                            ErrorMessage = content.Header.ErrorMessage,
                            ReturnCode = content.Header.ReturnCode,
                            ResponsePayload = content.Payload
                        };
                        int taskId = content.Header.Sequence;
                        _taskList[taskId].CompletionSource.TrySetResult(result);
                        _taskList.Remove(taskId);
                    }
                }
                catch (IOException ex)
                {
                    HandleConnectionFailure();
                    continue;
                }
                catch (Exception ex)
                {
                    // TODO: Implement error handling.
                    Console.WriteLine($"Warning: unhandled exception in msf receive loop. {ex.ToString()}");
                }
            }
        }

        private void HandleConnectionFailure()
        {
            this._baseClient?.Dispose();
            this._connectedEvent.Reset();
            this.ConnectionFailed?.Invoke(this, new EventArgs());
        }

        public async Task ConnectAsync(int defaultTimeout = 5000)
        {
            this._baseClient?.Dispose();
            
            this._baseClient = new TcpClient();
            this.CurrentCookie = BinaryUtils.UnifiedRandomBytes(4);

            try
            {
                await this._baseClient.ConnectAsync(DefaultAddress, DefaultPort);
                var cts = new CancellationTokenSource(defaultTimeout);
                cts.Token.Register(() => this._baseClient.Dispose(), false);
                await this._baseClient.WritePacketAsync(new MsfNegotiationPacket().GetBinary());
                var response = await this._baseClient.ReadPacketAsync();
                var receivedNegotiationPacket = new MsfNegotiationPacket();
                receivedNegotiationPacket.ParseFrom(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("Unable to establish a connection to MSF server, " +
                                    "probably because of a timeout or an incorrect server address.", ex);
            }
            
            this._connectedEvent.Set();
        }
    }
}