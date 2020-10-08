using System.Threading.Tasks;

namespace ProcessInvoke.Tests {
    public interface IRemoteObject {
        Task<int> HostingProcessIdAsync();
    }

}
