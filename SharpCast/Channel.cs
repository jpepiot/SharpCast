namespace SharpCast {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using extensions.api.cast_channel;
    using Google.ProtocolBuffers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Timer = System.Timers.Timer;

    internal class Channel {

        public const string NS_CAST_MEDIA = "urn:x-cast:com.google.cast.media";
        public const string NS_CAST_RECEIVER = "urn:x-cast:com.google.cast.receiver";
        public const string NS_CAST_CONNECTION = "urn:x-cast:com.google.cast.tp.connection";
        public const string NS_CAST_HEARTBEAT = "urn:x-cast:com.google.cast.tp.heartbeat";
        public const string NS_CAST_DEVICEAUTH = "urn:x-cast:com.google.cast.tp.deviceauth";
        public const string DEFAULT_RECEIVER_ID = "receiver-0";
        public const string DEFAULT_APP_ID = "CC1AD845";

        private const int PING_PERIOD = 20 * 1000;
        private const int REQUEST_TIMEOUT = 10 * 1000;

        private readonly string _host;
        private readonly int _port;
        private readonly Timer _pingTimer;
        private readonly string _sourceId;
        private readonly ConcurrentDictionary<int, ResponseHolder> _responses = new ConcurrentDictionary<int, ResponseHolder>();
        private Socket _socket;
        private SslStream _sslStream;
        private int _requestId;
        private Thread _readThread;
        private bool _connected;

        public Channel(string host, int port) {
            _host = host;
            _port = port;
            _pingTimer = new Timer();
            _pingTimer.Interval = PING_PERIOD;
            _sourceId = "sender-" + new Random().Next(1, short.MaxValue);
            _pingTimer.Elapsed += (sender, args) =>
            {
                try {
                    SendMessage(NS_CAST_HEARTBEAT, new PingMessage(), DEFAULT_RECEIVER_ID);
                }
                catch (IOException ex) {
                    Debug.WriteLine(ex.Message);
                }
            };
        }

        public event EventHandler<Response> Status;

        public bool IsConnected {
            get {
                return _connected;
            }
        }

        public void Connect() {

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            _socket.Connect(_host, _port);

            _sslStream = new SslStream(new NetworkStream(_socket, true), false, OnCertificateValidation);
            _sslStream.AuthenticateAsClient(_host);

            DeviceAuthMessage authMessage = DeviceAuthMessage.CreateBuilder()
                .SetChallenge(AuthChallenge.CreateBuilder().Build())
                .Build();

            SendMessage(NS_CAST_DEVICEAUTH, authMessage.ToByteString(), DEFAULT_RECEIVER_ID);
            CastMessage response = ReadMessage();
            DeviceAuthMessage authResponse = DeviceAuthMessage.ParseFrom(response.PayloadBinary);
            if (authResponse.HasError) {
                throw new IOException("Authentication failed: " + authResponse.Error.ErrorType);
            }

            SendMessage(NS_CAST_CONNECTION, new ConnectMessage(), DEFAULT_RECEIVER_ID);
            _pingTimer.Start();
            _connected = true;

            _readThread = new Thread(() =>
            {
                while (_connected) {
                    CastMessage message = ReadMessage();
                    if (message.PayloadType == CastMessage.Types.PayloadType.STRING) {
                        Debug.WriteLine("RESPONSE : " + message.PayloadUtf8);
                        dynamic responseObject = JObject.Parse(message.PayloadUtf8);
                        Type type = Response.GetResponseType((string)responseObject.type);
                        if (type != null) {
                            if (type == typeof(PingResponse)) {
                                SendMessage(NS_CAST_HEARTBEAT, new PongMessage(), DEFAULT_RECEIVER_ID);
                            }
                            else {
                                var rsp = JsonConvert.DeserializeObject(message.PayloadUtf8, type, new JsonSerializerSettings {
                                    Converters = new List<JsonConverter> { new MediaMetadataJsonConverter() }
                                }) as Response;
                                if (rsp != null) {
                                    if (_responses.ContainsKey(rsp.RequestId)) {
                                        ResponseHolder responseHolder;
                                        if (_responses.TryGetValue(rsp.RequestId, out responseHolder)) {
                                            responseHolder.Response = rsp;
                                            responseHolder.Signal.Set();
                                        }
                                    }
                                    else {
                                        if (Status != null) {
                                            Status(this, rsp);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else {
                        Debug.WriteLine("Received unhandled payload type : " + message.PayloadType);
                    }
                }
            });

            _readThread.Start();
        }


        public Response SendRequest(string ns, Request request, string destinationId) {
            int requestId = Interlocked.Increment(ref _requestId);
            request.RequestId = requestId;
            SendMessage(ns, request, destinationId);
            ResponseHolder responseHolder = new ResponseHolder();
            _responses[requestId] = responseHolder;
            // Wait for response
            bool signaled = responseHolder.Signal.WaitOne(REQUEST_TIMEOUT);
            if (!signaled) {
                throw new TimeoutException(string.Format("Could not get response for the request {0}", request.GetType().Name));
            }

            responseHolder.Signal.Close();
            ResponseHolder holder;
            _responses.TryRemove(requestId, out holder);
            return responseHolder.Response;
        }

        public void SendMessage(string ns, Message command, string destinationId) {
            SendMessage(CastMessage.CreateBuilder()
               .SetProtocolVersion(CastMessage.Types.ProtocolVersion.CASTV2_1_0)
               .SetSourceId(_sourceId)
               .SetDestinationId(destinationId)
               .SetNamespace(ns)
               .SetPayloadType(CastMessage.Types.PayloadType.STRING)
               .SetPayloadUtf8(JsonConvert.SerializeObject(command))
               .Build());
        }

        public void SendMessage(string ns, ByteString byteString, string destinationId) {
            SendMessage(CastMessage.CreateBuilder()
                            .SetProtocolVersion(CastMessage.Types.ProtocolVersion.CASTV2_1_0)
                            .SetSourceId(_sourceId)
                            .SetDestinationId(destinationId)
                            .SetNamespace(ns)
                            .SetPayloadType(CastMessage.Types.PayloadType.BINARY)
                            .SetPayloadBinary(byteString)
                            .Build());
        }

        public void Disconnect() {
            _connected = false;

            if (_sslStream != null) {
                _sslStream.Dispose();
            }

            if (_pingTimer != null) {
                _pingTimer.Stop();
            }
        }

        public void Dispose() {
            if (_connected) {
                Disconnect();
            }

            if (_pingTimer != null) {
                _pingTimer.Dispose();
            }
        }

        private CastMessage ReadMessage() {
            int recv = 0;
            byte[] bytes = new byte[4];
            do {
                int read = _sslStream.Read(bytes, recv, 4 - recv);
                if (read == 0) {
                    break;
                }

                recv += read;
            } while (recv < 4);

            if (BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }

            int size = BitConverter.ToInt32(bytes, 0);
            byte[] buffer = new byte[size];
            recv = 0;
            do {
                int read = _sslStream.Read(buffer, recv, size - recv);
                if (read == 0) {
                    break;
                }

                recv += read;
            } while (recv < size);

            return CastMessage.ParseFrom(buffer);
        }

        private void SendMessage(CastMessage message) {
            byte[] bytes = BitConverter.GetBytes(message.SerializedSize);
            if (BitConverter.IsLittleEndian) {
                Array.Reverse(bytes);
            }

            _sslStream.Write(bytes);
            message.WriteTo(_sslStream);
        }

        private static bool OnCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors) {
            return true;
        }

        private class ResponseHolder {

            private readonly ManualResetEvent _signal = new ManualResetEvent(false);

            public ManualResetEvent Signal {
                get { return _signal; }
            }

            public Response Response { get; set; }
        }
    }
}
