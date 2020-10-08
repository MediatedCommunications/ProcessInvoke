using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {
    public class MultiIpcServer : IMultiIpcServer {
        protected Endpoint Options { get; private set; }
        public MultiIpcServer(Endpoint Options) {
            this.Options = Options;
        }

        protected ConcurrentDictionary<Type, IpcServerBase> FactoryCache { get; set; } = new ConcurrentDictionary<Type, IpcServerBase>();
        protected ConcurrentDictionary<Type, Endpoint> EndpointCache { get; set; } = new ConcurrentDictionary<Type, Endpoint>();

        public virtual async Task<Endpoint?> HostEndpointAsync(Type Key) {
            var ret = default(Endpoint);

            if (!FactoryCache.ContainsKey(Key)) {
                var ChildOptions = new Endpoint(
                    Options.Provider,
                    Options.Host,
                    Options.Port,
                    Guid.NewGuid().ToString()
                );

                var Handler = Activator.CreateInstance(Key);

                if (Handler is { } V1) {
                    var NewProvider = new IpcServer(ChildOptions, V1);
                    var HostingLocation = await NewProvider.StartHostingAsync()
                        .DefaultAwait()
                        ;

                    FactoryCache[Key] = NewProvider;
                    EndpointCache[Key] = HostingLocation;
                    ret = HostingLocation;

                }
            } else {
                ret = EndpointCache[Key];
            }
            return ret;
        }
    }

}
