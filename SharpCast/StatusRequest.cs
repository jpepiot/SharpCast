namespace SharpCast {
    public class StatusRequest : Request {
        public override string Type {
            get {
                return "GET_STATUS";
            }
        }
    }
}
