using ProcessInvoke.Hosting.Object;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessInvoke.Hosting.Process {
    public class RootObjectHost : HostBase, IRootObjectHost {
        protected ProcessHostOptions ProcessOptions { get; private set; }

        public RootObjectHost(ProcessHostOptions ProcessOptions) : base(ProcessOptions.ToEndpoint()) {
            this.ProcessOptions = ProcessOptions;
        }

        public async Task<Endpoint?> CloneEndpointAsync() {
            var ret = default(Endpoint);

            var Options = ProcessOptions.Clone();
            Options.ListenOn_Key = Guid.NewGuid().ToString();


            var tret = await CurrentProcessProcessHost.Instance.StartAsync(Options)
                .DefaultAwait()
                ;
            if(tret is { }) {
                ret = Options.ToEndpoint();
            }


            return ret;
        }


        protected ConcurrentDictionary<Type, HostBase> FactoryCache { get; set; } = new ConcurrentDictionary<Type, HostBase>();
        protected ConcurrentDictionary<Type, Endpoint> EndpointCache { get; set; } = new ConcurrentDictionary<Type, Endpoint>();

        public async Task<Endpoint?> HostEndpointAsync(Type Key) {
            var ret = default(Endpoint);
            
            if (!FactoryCache.ContainsKey(Key)) {
                var ChildOptions = new Endpoint(
                    Options.Provider,
                    Options.Host,
                    Options.Port,
                    Guid.NewGuid().ToString()
                );

                var Handler = Activator.CreateInstance(Key);

                if(Handler is { } V1) {
                    var NewProvider = new ObjectHost(ChildOptions, V1);
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

        protected override object GetHandler() {
            return this;
        }

    }

}
