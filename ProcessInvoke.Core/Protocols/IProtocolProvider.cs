using ProcessInvoke.Server.OutOfProcess;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessInvoke.Protocols {
    public interface IProtocolProvider {
        Task<T> ConnectAsync<T>(Endpoint Endpoint, OutOfProcessClientOptions? Options = default) where T : class;

        Task StartListeningAsync(Endpoint Endpoint, CancellationToken Token, Func<Object> GetHandler);

    }
}
