using System;
using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {
    public class OutOfProcessCloneableIpcServer : ICloneableIpcServer {
        protected OutOfProcessServerOptions ProcessOptions { get; private set; }
        protected OutOfProcessFactory Factory { get; private set; }

        public OutOfProcessCloneableIpcServer(OutOfProcessServerOptions ProcessOptions, OutOfProcessFactory? Factory = default) {
            this.ProcessOptions = ProcessOptions;
            this.Factory = Factory ?? CurrentProcessProcessHost.Instance;
        }

        public virtual async Task<Endpoint?> CloneEndpointAsync() {
            var ret = default(Endpoint);

            var Options = ProcessOptions.Clone();
            Options.ListenOn_Key = Guid.NewGuid().ToString();


            var tret = await Factory.StartAsync(Options)
                .DefaultAwait()
                ;
            if (tret is { }) {
                ret = Options.ToEndpoint();
            }


            return ret;
        }
    }

}
