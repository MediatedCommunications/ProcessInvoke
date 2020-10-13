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

    public class DependencyAssemblyResolver : AssemblyResolver {
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

        protected override Assembly? Context_Resolving(AssemblyLoadContext Context, AssemblyName Name) {
            var ret = default(Assembly?);


            if (DependencyContext?.RuntimeLibraries is { } RuntimeLibraries) {

                var PotentialLibrary = (
                    from x in RuntimeLibraries
                    where string.Equals(x.Name, Name.Name, StringComparison.OrdinalIgnoreCase)
                    select x
                    ).FirstOrDefault();


                if (PotentialLibrary is { } Library) {
                    var wrapper = new CompilationLibrary(
                        Library.Type,
                        Library.Name,
                        Library.Version,
                        Library.Hash,
                        Library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                        Library.Dependencies,
                        Library.Serviceable);

                    var assemblies = new List<string>();
                    this.Resolver.TryResolveAssemblyPaths(wrapper, assemblies);
                    if (assemblies.Count > 0) {
                        ret = this.Context.LoadFromAssemblyPath(assemblies[0]);
                    }
                }
            }

            if (ret == default) {
 
            }

            return ret;
        }
    }
}
