using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess.Restartable {

    public class RestartableOutOfProcessController : IOutOfProcessController {
        public IOutOfProcessFactory Factory { get; private set; }
        public IOutOfProcessController? Instance { get; private set; }

        private OutOfProcessServerOptions? ServerOptions { get; set; }
        private OutOfProcessClientOptions? ClientOptions { get; set; }

        public RestartableOutOfProcessController(IOutOfProcessFactory Factory, OutOfProcessServerOptions? ServerOptions = default, OutOfProcessClientOptions? ClientOptions = default) {
            this.Factory = Factory;
            this.ServerOptions = ServerOptions;
            this.ClientOptions = ClientOptions;
        }

        private async Task<T> InvokeAsync<T>(Func<IOutOfProcessController, Task<T>> Action) {
            var ret = default(T);

            var Retry = false;
            var Success = false;
            var Failures = 0;
            

            do {
                if (Instance == default) {
                    try {
                        Instance = await Factory.StartAsync(ServerOptions, ClientOptions)
                            .DefaultAwait()
                            ;
                    } catch (Exception ex) {
                        ex.Ignore();

                        throw new Exception("Unable to restart host");
                    }
                }
                try {
                    ret = await Action(Instance)
                        .DefaultAwait()
                        ;
                    Retry = false;
                    Success = true;
                } catch (Exception ex) {
                    ex.Ignore();

                    Instance = default;
                    Retry = true;
                    Failures += 1;
                }
            } while (Retry == true && Failures <= 1);

            return ret;
        }

        private Task InvokeAsync(Func<IOutOfProcessController, Task> Action) {
            return InvokeAsync(async x => {
                await Action(x)
                .DefaultAwait();

                return true;
            });
        }

        public Task<Endpoint?> CloneEndpointAsync() {
            return InvokeAsync(x => x.CloneEndpointAsync());
        }

        public Task<Endpoint?> HostEndpointAsync(Type Key) {
            return InvokeAsync(x => x.HostEndpointAsync(Key));
        }

        public Task StopAsync() {
            return InvokeAsync(x => x.StopAsync());
        }
    }
}
