namespace ProcessInvoke.Hosting.Object {
    public class ObjectHost : HostBase {
        public object Handler { get; protected set; }

        public ObjectHost(Endpoint Options, object Handler) : base(Options) {
            this.Handler = Handler;
        }

        protected override object GetHandler() {
            return Handler;
        }

    }

}
