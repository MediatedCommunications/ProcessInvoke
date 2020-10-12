using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProcessInvoke {
    internal sealed class DependencyAssemblyResolver : AssemblyResolver {
        private readonly ICompilationAssemblyResolver Resolver;
        private readonly DependencyContext DependencyContext;

        public DependencyAssemblyResolver(Assembly Assembly, AssemblyLoadContext AssemblyContext) : base(AssemblyContext) {
            this.Assembly = Assembly;
            this.DependencyContext = DependencyContext.Load(this.Assembly);

            var Resolvers = new List<ICompilationAssemblyResolver>() {
                new ReferenceAssemblyPathResolver(),
                new PackageCompilationAssemblyResolver(),
            };

            if (!Assembly.IsDynamic) {
                var AsmPath = Path.GetDirectoryName(Assembly.Location);
                Resolvers.Add(new AppBaseCompilationAssemblyResolver(AsmPath));
            }

            this.Resolver = new CompositeCompilationAssemblyResolver(Resolvers.ToArray());
        }

        public Assembly Assembly { get; }

        protected override Assembly? Context_Resolving(AssemblyLoadContext context, AssemblyName name) {
            var ret = default(Assembly?);

            var library = (
                from x in DependencyContext.RuntimeLibraries
                where string.Equals(x.Name, name.Name, StringComparison.OrdinalIgnoreCase)
                select x
                ).FirstOrDefault();

                
            if (library is { }) {
                var wrapper = new CompilationLibrary(
                    library.Type,
                    library.Name,
                    library.Version,
                    library.Hash,
                    library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                    library.Dependencies,
                    library.Serviceable);

                var assemblies = new List<string>();
                this.Resolver.TryResolveAssemblyPaths(wrapper, assemblies);
                if (assemblies.Count > 0) {
                    ret = this.Context.LoadFromAssemblyPath(assemblies[0]);
                }
            }

            return ret;
        }
    }
}
