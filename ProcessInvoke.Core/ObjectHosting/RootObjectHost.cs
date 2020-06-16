using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessInvoke.Core {
    public class RootObjectHost : ObjectHostBase, IRootObjectHost {
        protected ProcessHostOptions ProcessOptions { get; private set; }

        public RootObjectHost(ProcessHostOptions ProcessOptions) : base(ProcessOptions.ToEndpoint()) {
            this.ProcessOptions = ProcessOptions;
        }

        public async Task<HostedObjectEndpoint?> CloneEndpointAsync() {
            var ret = default(HostedObjectEndpoint);

            var Options = ProcessOptions.Clone();
            Options.ListenOn_Key = Guid.NewGuid().ToString();


            var tret = await CurrentProcessProcessHost.Instance.StartAsync(Options);
            if(tret is { }) {
                ret = Options.ToEndpoint();
            }


            return ret;
        }


        protected ConcurrentDictionary<Type, ObjectHostBase> FactoryCache { get; set; } = new ConcurrentDictionary<Type, ObjectHostBase>();
        protected ConcurrentDictionary<Type, HostedObjectEndpoint> EndpointCache { get; set; } = new ConcurrentDictionary<Type, HostedObjectEndpoint>();

        public async Task<HostedObjectEndpoint?> HostEndpointAsync(Type Key) {
            var ret = default(HostedObjectEndpoint);
            
            if (!FactoryCache.ContainsKey(Key)) {
                var ChildOptions = new HostedObjectEndpoint(
                    Options.Provider,
                    Options.Host,
                    Options.Port,
                    Guid.NewGuid().ToString()
                );

                var Handler = Activator.CreateInstance(Key);

                if(Handler is { } V1) {
                    var NewProvider = new ObjectHost(ChildOptions, V1);
                    var HostingLocation = await NewProvider.StartHostingAsync();

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
