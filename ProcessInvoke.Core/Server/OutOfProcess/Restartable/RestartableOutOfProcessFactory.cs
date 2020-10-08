using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess.Restartable {
    public class RestartableOutOfProcessFactory : IOutOfProcessFactory {
        public IOutOfProcessFactory Child { get; private set; }
        public RestartableOutOfProcessFactory(IOutOfProcessFactory Child) {
            this.Child = Child;
        }

        public Task<IOutOfProcessController> StartAsync(OutOfProcessServerOptions? ServerOptions = default, OutOfProcessClientOptions? ClientOptions = default) {
            var ret = (IOutOfProcessController)new RestartableOutOfProcessController(Child, ServerOptions, ClientOptions);
            return Task.FromResult(ret);
        }
    }
}
