using Mono.Options;
using System.Net;
using System.Threading;

namespace ProcessInvoke.Server.OutOfProcess {

    public class OutOfProcessIpcServer {

        public static IpcServer Create(OutOfProcessServerOptions Options, OutOfProcessFactory? Factory = default) {
            var EP = Options.ToEndpoint();

            var Server = new IpcServer(EP);
            var Handler = new OutOfProcessController(Server, Options, Factory);
            Server.Handler = Handler;

            return Server;
        }

        

    }


}
