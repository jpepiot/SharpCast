namespace SharpCast {
    public class StopRequest : SessionRequest {

        public StopRequest(string sessionId) {
            SessionId = sessionId;
        }

        public override string Type {
            get {
                return "STOP";
            }
        }
    }
}
