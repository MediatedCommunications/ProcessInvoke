using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        protected HashSet<string> LoadedAssemblies { get; private set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        protected List<DependencyAssemblyResolver> DependencyAssemblyResolvers { get; private set; } = new List<DependencyAssemblyResolver>();

        protected HashSet<string> LoadedPaths { get; private set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        protected List<PathAssemblyResolver> PathAssemblyResolvers { get; private set; } = new List<PathAssemblyResolver>();


        public virtual async Task<Endpoint?> HostEndpointAsync(string AssemblyFullPath, string AssemblyQualifiedTypeName) {
            var ret = default(Endpoint);

            var AssemblyFolderPath = System.IO.Path.GetDirectoryName(AssemblyFullPath);

            var Key = $@"{AssemblyFullPath}-{AssemblyQualifiedTypeName}";

            if (!FactoryCache.ContainsKey(Key)) {
                var ChildOptions = new Endpoint(
                    Options.Provider,
                    Options.Host,
                    Options.Port,
                    Guid.NewGuid().ToString()
                );



                var Context = AssemblyLoadContext.Default;
                var ASM = Context.LoadFromAssemblyPath(AssemblyFullPath);

                {
                    if (LoadedAssemblies.Add(AssemblyFullPath)) {
                        DependencyAssemblyResolvers.Add(new DependencyAssemblyResolver(ASM, Context) { Enabled = true });
                    }
                }

                {
                    //Turn them off and then back on so that the delegates order correctly.
                    foreach (var item in PathAssemblyResolvers) {
                        item.Enabled = false;
                        item.Enabled = true;
                    }

                    //Add our new one.
                    if (AssemblyFolderPath is { } V1 && LoadedPaths.Add(V1)) {
                        PathAssemblyResolvers.Add(new PathAssemblyResolver(V1, Context) { Enabled = true });
                    }
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
