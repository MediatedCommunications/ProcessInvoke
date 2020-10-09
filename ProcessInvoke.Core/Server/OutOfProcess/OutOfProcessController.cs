using System;
using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {
    public class OutOfProcessController : IOutOfProcessController {
        readonly StoppableIpcServer V3;
        readonly MultiIpcServer V1;
        readonly OutOfProcessCloneableIpcServer V2;

        public OutOfProcessController(IpcServerBase Server, OutOfProcessServerOptions Options, OutOfProcessFactory? Factory = default) {
            V3 = new StoppableIpcServer(Server);
            V1 = new MultiIpcServer(Options.ToEndpoint());
            V2 = new OutOfProcessCloneableIpcServer(Options, Factory);
        }

        public Task<Endpoint?> CloneEndpointAsync() {
            return V2.CloneEndpointAsync();
        }

        public Task<Endpoint?> HostEndpointAsync(Type Key) {
            return V1.HostEndpointAsync(Key);
        }

        public Task StopAsync() {
            return V3.StopAsync();
        }
    }


}
