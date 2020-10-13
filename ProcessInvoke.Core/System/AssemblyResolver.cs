using System.Reflection;
using System.Runtime.Loader;

namespace System {
    public class AssemblyResolver {

        protected AssemblyLoadContext Context { get; private set; }

        public AssemblyResolver(AssemblyLoadContext Context) {
            this.Context = Context;
        }

        private bool __Enabled;
        public bool Enabled {
            get {
                return __Enabled;
            }
            set {
                if (value != __Enabled) {
                    if (value) {
                        Context.Resolving += Context_Resolving;
                    } else {
                        Context.Resolving -= Context_Resolving;
                    }
                    __Enabled = value;
                }
            }
        }

        protected virtual Assembly? Context_Resolving(AssemblyLoadContext Context, AssemblyName Name) {
            return default;
        }

    }

}