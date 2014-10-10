namespace SharpCast {
    public class PingMessage : Message {
        public override string Type {
            get { return "PING"; }
        }
    }
}
