using System.Threading.Tasks;

namespace ProcessInvoke.Tests {
    public class RemoteObject : IRemoteObject {
        public Task<int> HostingProcessIdAsync() {
            var ret = System.Diagnostics.Process.GetCurrentProcess().Id;

            return Task.FromResult(ret);
        }
    }

}
