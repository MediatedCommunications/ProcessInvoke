using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {
    public interface ICloneableIpcServer {
        Task<Endpoint?> CloneEndpointAsync();
    }

}
