namespace SharpCast {
    using System;
    using System.Collections.Generic;

    public class Player {

        private readonly Channel _channel;
        private readonly HashSet<string> _sessions = new HashSet<string>();

        public Player(string host, int port = 8009) {
            _channel = new Channel(host, port);
            _channel.Status += (sender, response) => {
                var statusResponse = response as StatusResponse;
                if (statusResponse != null) {
                    OnStatusChanged(statusResponse.Status);
                    return;
                }
                var mediaSatusResponse = response as MediaStatusResponse;
                if (mediaSatusResponse != null
                    && mediaSatusResponse.Statuses != null
                    && mediaSatusResponse.Statuses.Count > 0) {
                    OnMediaStatusChanged(mediaSatusResponse.Statuses[0]);
                }
            };
        }

        public void Connect() {
            _channel.Connect();
        }

        public event EventHandler<MediaStatus> MediaStatusChanged;

        public event EventHandler<Status> StatusChanged;

        public Application LaunchApp(string appId) {
            EnsureChannelIsConnected();
            StatusResponse response = _channel.SendRequest(Channel.NS_CAST_RECEIVER, new LaunchRequest(appId), Channel.DEFAULT_RECEIVER_ID) as StatusResponse;
            if (response != null && response.Status != null) {
                return response.Status.Applications[0];
            }

            return null;
        }

        public void StopApp() {
            EnsureChannelIsConnected();
            Application runningApplication = EnsureApplicationIsRunning();
            _channel.SendRequest(Channel.NS_CAST_RECEIVER, new StopRequest(runningApplication.SessionId), Channel.DEFAULT_RECEIVER_ID);
        }

        public Status GetStatus() {
            EnsureChannelIsConnected();
            StatusResponse response = _channel.SendRequest(Channel.NS_CAST_RECEIVER, new StatusRequest(), Channel.DEFAULT_RECEIVER_ID) as StatusResponse;
            return response != null ? response.Status : null;
        }

        public bool GetAppAvailability(string applicationId) {
            GetAppAvailabilityResponse response = _channel.SendRequest(Channel.NS_CAST_RECEIVER, new GetAppAvailabilityRequest(new[] { applicationId }), Channel.DEFAULT_RECEIVER_ID) as GetAppAvailabilityResponse;
            if (response != null && response.Availability != null) {
                return response.Availability.ContainsKey(applicationId) && response.Availability[applicationId].Equals("APP_AVAILABLE");
            }

            return false;
        }

        public void SetVolume(double level) {
            EnsureChannelIsConnected();
            _channel.SendRequest(Channel.NS_CAST_RECEIVER, new SetVolumeRequest(new Volume(level, false)), Channel.DEFAULT_RECEIVER_ID);
        }

        public void SetMuted(bool mute) {
            EnsureChannelIsConnected();
            _channel.SendRequest(Channel.NS_CAST_RECEIVER, new SetVolumeRequest(new Volume(null, mute)), Channel.DEFAULT_RECEIVER_ID);
        }

        public void LoadVideo(Uri contentUri, string contentType, MovieMediaMetadata metadata, bool autoPlay = true, StreamType streamType = StreamType.BUFFERED) {
            Load(contentUri, contentType, metadata);
        }

        public void LoadPhoto(Uri contentUri, string contentType, PhotoMediaMetadata metadata, bool autoPlay = true, StreamType streamType = StreamType.BUFFERED) {
            Load(contentUri, contentType, metadata);
        }

        public void LoadMusic(Uri contentUri, string contentType, MusicTrackMediaMetadata metadata, bool autoPlay = true, StreamType streamType = StreamType.BUFFERED) {
            Load(contentUri, contentType, metadata);
        }

        public void Load(Uri contentUri, string contentType, MediaMetadata metadata, bool autoPlay = true, StreamType streamType = StreamType.BUFFERED) {
            EnsureChannelIsConnected();
            Application runningApplication = EnsureApplicationIsRunning();
            if (runningApplication.ApplicationId != Channel.DEFAULT_APP_ID) {
                LaunchApp(Channel.DEFAULT_APP_ID);
                runningApplication = EnsureApplicationIsRunning();
            }

            StartSession(runningApplication.TransportId);

            Response response = _channel.SendRequest(Channel.NS_CAST_MEDIA, new LoadRequest(runningApplication.SessionId, new Media {
                ContentId = contentUri.ToString(),
                ContentType = contentType,
                Metadata = metadata,
                StreamType = streamType
            }, autoPlay), runningApplication.TransportId);

            HandleResponse(response);
        }

        public void Play() {
            EnsureChannelIsConnected();
            Application runningApplication = EnsureApplicationIsRunning();
            MediaStatus mediaStatus = GetMediaStatus(runningApplication.TransportId);
            if (mediaStatus == null) {
                throw new InvalidOperationException("No media running");
            }

            StartSession(runningApplication.TransportId);
            Response response = _channel.SendRequest(Channel.NS_CAST_MEDIA, new PlayRequest(runningApplication.SessionId, mediaStatus.MediaSessionId), runningApplication.TransportId);
            HandleResponse(response);
        }

        public void Pause() {
            EnsureChannelIsConnected();
            Application runningApplication = EnsureApplicationIsRunning();
            MediaStatus mediaStatus = GetMediaStatus(runningApplication.TransportId);
            if (mediaStatus == null) {
                throw new InvalidOperationException("No media running");
            }

            StartSession(runningApplication.TransportId);
            Response response = _channel.SendRequest(Channel.NS_CAST_MEDIA, new PauseRequest(runningApplication.SessionId, mediaStatus.MediaSessionId), runningApplication.TransportId);
            HandleResponse(response);
        }

        public void Seek(double position) {
            EnsureChannelIsConnected();
            Application runningApplication = EnsureApplicationIsRunning();
            MediaStatus mediaStatus = GetMediaStatus(runningApplication.TransportId);
            if (mediaStatus == null) {
                throw new InvalidOperationException("No media running");
            }

            StartSession(runningApplication.TransportId);
            Response response = _channel.SendRequest(Channel.NS_CAST_MEDIA, new SeekRequest(runningApplication.SessionId, mediaStatus.MediaSessionId, position), runningApplication.TransportId);
            HandleResponse(response);
        }

        public Application GetRunningApp() {
            EnsureChannelIsConnected();
            Status status = GetStatus();
            return status.Applications != null && status.Applications.Count > 0 ? status.Applications[0] : null;
        }

        public void Close() {
            if (_channel != null) {
                _channel.Dispose();
            }
        }

        private MediaStatus GetMediaStatus(string transportId) {
            Response response = _channel.SendRequest(Channel.NS_CAST_MEDIA, new StatusRequest(), transportId);
            MediaStatusResponse statusResponse = response as MediaStatusResponse;
            if (statusResponse != null && statusResponse.Statuses != null && statusResponse.Statuses.Count > 0) {
                return statusResponse.Statuses[0];
            }

            return null;
        }

        private void HandleResponse(Response response) {
            MediaStatusResponse mediaStatusResponse = response as MediaStatusResponse;
            if (mediaStatusResponse != null) {
                if (mediaStatusResponse.Statuses != null && mediaStatusResponse.Statuses.Count > 0) {
                    OnMediaStatusChanged(mediaStatusResponse.Statuses[0]);
                }

                return;
            }

            LoadFailedResponse loadFailedResponse = response as LoadFailedResponse;
            if (loadFailedResponse != null) {
                throw new LoadMediaException(loadFailedResponse.Code, loadFailedResponse.Details);
            }

            InvalidRequestResponse invalidRequestResponse = response as InvalidRequestResponse;
            if (invalidRequestResponse != null) {
                throw new InvalidRequestException(invalidRequestResponse.Reason);
            }
        }

        private void StartSession(string destinationId) {
            if (!_sessions.Contains(destinationId)) {
                _channel.SendMessage(Channel.NS_CAST_CONNECTION, new ConnectMessage(), destinationId);
                _sessions.Add(destinationId);
            }
        }

        private Application EnsureApplicationIsRunning() {
            Application application = GetRunningApp();
            if (application == null) {
                throw new InvalidOperationException("No running application");
            }

            return application;
        }

        private void EnsureChannelIsConnected() {
            if (_channel == null) {
                throw new InvalidOperationException("Not connected to chromecast");
            }

            if (!_channel.IsConnected) {
                _channel.Connect();
            }
        }

        private void OnMediaStatusChanged(MediaStatus status) {
            if (MediaStatusChanged != null) {
                MediaStatusChanged(this, status);
            }
        }

        private void OnStatusChanged(Status status) {
            if (StatusChanged != null) {
                StatusChanged(this, status);
            }
        }
    }
}
