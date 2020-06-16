namespace ProcessInvoke.Core {
    public class ObjectHost : ObjectHostBase {
        public object Handler { get; protected set; }

        public ObjectHost(HostedObjectEndpoint Options, object Handler) : base(Options) {
            this.Handler = Handler;
        }

        protected override object GetHandler() {
            return Handler;
        }

    }

}
