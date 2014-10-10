namespace SharpCast {
    using System;

    public class InvalidRequestException : Exception {
        public InvalidRequestException(string reason)
            : base(reason) {
        }
    }
}
