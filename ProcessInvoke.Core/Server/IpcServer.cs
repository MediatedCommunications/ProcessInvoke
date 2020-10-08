using System;

namespace ProcessInvoke.Server {
    public class IpcServer : IpcServerBase {

        private object? __Handler;
        public object? Handler {
            get {
                return __Handler;
            }
            set {
                if(Source != default) {
                    throw new ArgumentException($@"{nameof(Handler)} cannot be set once the server is started.", nameof(Handler));
                }
                __Handler = value;
            }
        }
        

        public IpcServer(Endpoint Options) : base(Options) {

        }

        public IpcServer(Endpoint Options, object Handler) : base(Options) {
            this.Handler = Handler;
        }

        protected override object GetHandler() {
            if(Handler == default) {
                throw new ArgumentException($@"{nameof(Handler)} has not been set.", nameof(Handler));
            }
            
            return Handler;
        }

    }

}
