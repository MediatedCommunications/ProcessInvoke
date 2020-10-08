using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {
    public class StoppableIpcServer : IStoppableIpcServer {
        protected IpcServerBase Server { get; private set; }

        public StoppableIpcServer(IpcServerBase Server) {
            this.Server = Server;
        }
        
        public Task StopAsync() {
            return Server.StopAsync();
        }
    }

}
