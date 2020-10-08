using System.Reflection;

namespace System {
    public class AssemblyResolver {
        private bool __Enabled;
        public bool Enabled {
            get {
                return __Enabled;
            }
            set {
                if (value != __Enabled) {
                    if (value) {
                        System.AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                    } else {
                        System.AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                    }
                    __Enabled = value;
                }
            }
        }

        protected virtual Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args) {
            return default;
        }
    }

}