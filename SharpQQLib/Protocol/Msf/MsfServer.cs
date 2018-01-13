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
using SharpQQ.Binarizer;
using SharpQQ.Utils;

namespace SharpQQ.Protocol.Msf
{
    public class MsfServer : IDisposable
    {
        private const string DefaultAddress = "msfxg.3g.qq.com";
        private const int DefaultPort = 14000;

        // public bool Connected => this._baseClient != null && this._baseClient.Connected;

        private readonly Dictionary<int, MsfTask> _taskList = new Dictionary<int, MsfTask>();
        private TcpClient _baseClient;
        private readonly CancellationTokenSource _globalCancellationTokenSource;
        private readonly AsyncProducerConsumerQueue<MsfTask> _waitingTasks = new AsyncProducerConsumerQueue<MsfTask>();

        public event EventHandler ConnectionFailed;

        public long QQNumber { get; }
        public AccountAuthInfo AuthInfo { get; set; }
        public MsfGeneralInfo MsfInfo { get; }
        public byte[] CurrentCookie { get; private set; }
        public bool Connected { get; private set; }

        public MsfServer(long qqNumber, MsfGeneralInfo msfInfo, AccountAuthInfo auth = null)
        {
            this.QQNumber = qqNumber;
            this.AuthInfo = auth;
            this.MsfInfo = msfInfo;
            this._globalCancellationTokenSource = new CancellationTokenSource();
        }

        #region Sequence

        private int _nextSequence = 10000;

        public int IncreaseSequence()
        {
            return Interlocked.Increment(ref _nextSequence) - 1;
        }

        #endregion

        public async Task<MsfResult> DoRequest(string opName, byte[] data, int timeout = Timeout.Infinite)
        {
            var task = new MsfTask
            {
                Sequence = this.IncreaseSequence(),
                CompletionSource = new TaskCompletionSource<MsfResult>(),
                OperationName = opName,
                Data = data
            };
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

            await this._waitingTasks.EnqueueAsync(task);

            return await task.CompletionSource.Task;
        }


        private async Task SendLoop()
        {
            var cancellationToken = _globalCancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                var task = await this._waitingTasks.DequeueAsync(cancellationToken);

                try
                {
                    var rawData = SsoHelper.EncodeRequest(task.Sequence, this.QQNumber, task.OperationName,
                        task.Data, this.CurrentCookie, this.MsfInfo, this.AuthInfo);
                    await this._baseClient.WritePacketAsync(rawData);
                }
                catch (IOException ex)
                {
                    // Network error occurred, push it to queue again.
                    await this._waitingTasks.EnqueueAsync(task, cancellationToken);
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
            var cancellationToken = _globalCancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
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
            this.Disconnect();
            this.ConnectionFailed?.Invoke(this, new EventArgs());
        }

        public void Disconnect()
        {
            this._baseClient?.Dispose();
            this._globalCancellationTokenSource.Cancel();
        }

        public async Task ConnectAsync(int defaultTimeout = 1000000)
        {
            this._baseClient?.Dispose();

            this._baseClient = new TcpClient();
            this.CurrentCookie = MiscellaneousUtils.UnifiedRandomBytes(4);

            bool timeout = false;
            var disposeCancellationTokenSouce = new CancellationTokenSource();
            var cancellationTask = Task.Delay(defaultTimeout, disposeCancellationTokenSouce.Token);
#pragma warning disable 4014
            cancellationTask.ContinueWith(_ =>
            {
                timeout = true;
                this._baseClient.Dispose();
            }, TaskContinuationOptions.NotOnCanceled);
#pragma warning restore 4014

            try
            {
                await this._baseClient.ConnectAsync(DefaultAddress, DefaultPort);

                await this._baseClient.WritePacketAsync(new MsfNegotiationConvertible().GetBinary());
                var response = await this._baseClient.ReadPacketAsync();
                var receivedNegotiationPacket = new MsfNegotiationConvertible();
                receivedNegotiationPacket.ParseFrom(response);

                disposeCancellationTokenSouce.Cancel();
            }
            catch (Exception ex)
            {
                if (timeout)
                {
                    throw new Exception("Msf server connection timed out.");
                }
                else
                {
                    disposeCancellationTokenSouce.Cancel();
                    this._baseClient.Dispose();
                    throw new Exception("Unable to establish a connection to MSF server, " +
                                        "probably because of an incorrect server address.", ex);
                }
            }

            var sendTask = SendLoop();
            var receiveTask = ReceiveLoop();
            // TODO: Deal with these two tasks. (Handle failure in loops)
        }

        public void Dispose()
        {
            _baseClient?.Dispose();
            _globalCancellationTokenSource?.Dispose();
        }
    }
}