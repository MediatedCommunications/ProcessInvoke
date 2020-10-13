using System;
using System.Reflection;
using System.Runtime.Loader;

namespace ProcessInvoke {
    public class PathAssemblyResolver : AssemblyResolver {
        protected string Path { get; private set; }

        private static string GetPath(Assembly Asm) {
            var Location = Asm.Location;
            var ret = System.IO.Path.GetDirectoryName(Location) ?? string.Empty;

            return ret;
        }

        public PathAssemblyResolver(Assembly Asm, AssemblyLoadContext Context) : this(GetPath(Asm), Context) { }

        public PathAssemblyResolver(string Path, AssemblyLoadContext Context) : base(Context) {
            this.Path = Path;
        }

        protected override Assembly? Context_Resolving(AssemblyLoadContext Context, AssemblyName Name) {
            var ret = default(Assembly?);

            var PotentialPath = System.IO.Path.Combine(Path, $@"{Name.Name}.dll");

            if (System.IO.File.Exists(PotentialPath)) {
                ret = Context.LoadFromAssemblyPath(PotentialPath);
            }

            return ret;
        }

    }
}
