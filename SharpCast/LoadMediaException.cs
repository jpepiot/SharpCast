namespace SharpCast {
    using System;

    public class LoadMediaException : Exception {
        public LoadMediaException(string code, string message)
            : base(message) {
            Code = code;
        }

        public string Code { get; set; }
    }
}
