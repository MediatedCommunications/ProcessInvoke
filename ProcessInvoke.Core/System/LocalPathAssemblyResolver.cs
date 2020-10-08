using System.Reflection;

namespace System {
    public class LocalPathAssemblyResolver : AssemblyResolver {
        public static LocalPathAssemblyResolver Instance { get; private set; } = new LocalPathAssemblyResolver();

        protected LocalPathAssemblyResolver() { 
        
        }

        protected override Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args) {
            var ret = default(Assembly?);

            var AsmName = new AssemblyName(args.Name);

            if (Assembly.GetEntryAssembly() is { } V1) {

                var Location = V1.Location;

                if (System.IO.Path.GetDirectoryName(Location) is { } Path) {

                    var NewLocation = System.IO.Path.Combine(Path, AsmName.Name + ".dll");
                    if (System.IO.File.Exists(NewLocation)) {
                        ret = System.Reflection.Assembly.LoadFile(NewLocation);
                    }
                }

            }


            return ret;
        }

    }

}