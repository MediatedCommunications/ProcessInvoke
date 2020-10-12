using System;
using System.Collections.Concurrent;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {
    public class MultiIpcServer : IMultiIpcServer {
        protected Endpoint Options { get; private set; }
        public MultiIpcServer(Endpoint Options) {
            this.Options = Options;
        }

        protected ConcurrentDictionary<string, IpcServerBase> FactoryCache { get; set; } = new ConcurrentDictionary<string, IpcServerBase>();
        protected ConcurrentDictionary<string, Endpoint> EndpointCache { get; set; } = new ConcurrentDictionary<string, Endpoint>();


        public virtual async Task<Endpoint?> HostEndpointAsync(string AssemblyFullPath, string AssemblyQualifiedTypeName) {
            var ret = default(Endpoint);

            var Key = $@"{AssemblyFullPath}-{AssemblyQualifiedTypeName}";

            if (!FactoryCache.ContainsKey(Key)) {
                var ChildOptions = new Endpoint(
                    Options.Provider,
                    Options.Host,
                    Options.Port,
                    Guid.NewGuid().ToString()
                );



                var Context = new AssemblyLoadContext(default);

                var ASM = Context.LoadFromAssemblyPath(AssemblyFullPath);

                //Enable resolving assemblies using the .deps files
                {
                    var CustomResolver = new DependencyAssemblyResolver(ASM, Context) {
                        Enabled = true
                    };
                    CustomResolver.Ignore();
                }


                if (ASM.GetType(AssemblyQualifiedTypeName) is { } TypeToCreate) {


                    var Handler = Activator.CreateInstance(TypeToCreate);

                    if (Handler is { } V1) {
                        var NewProvider = new IpcServer(ChildOptions, V1);
                        var HostingLocation = await NewProvider.StartHostingAsync()
                            .DefaultAwait()
                            ;

                        FactoryCache[Key] = NewProvider;
                        EndpointCache[Key] = HostingLocation;
                        ret = HostingLocation;

                    }
                }
            } else {
                ret = EndpointCache[Key];
            }
            return ret;
        }
    }

}
