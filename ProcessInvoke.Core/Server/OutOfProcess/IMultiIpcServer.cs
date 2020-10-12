using System;
using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {
    public interface IMultiIpcServer {
        Task<Endpoint?> HostEndpointAsync(string Assembly, string AssemblyQualifiedTypeName);
    }

}
