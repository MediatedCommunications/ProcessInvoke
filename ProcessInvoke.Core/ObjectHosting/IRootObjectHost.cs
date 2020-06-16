using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ProcessInvoke {
    public interface IRootObjectHost {
        Task<HostedObjectEndpoint?> CloneEndpointAsync();
        Task<HostedObjectEndpoint?> HostEndpointAsync(Type Key);
        Task StopAsync();
    }

    public static class IRootObjectHostExtensions {

        public static async Task<IRootObjectHost?> CloneAsync(this IRootObjectHost This) {
            var EP = await This.CloneEndpointAsync();

            var ret = await ProtocolProvider.TryConnectAsync<IRootObjectHost>(EP);

            return ret;
        }

        public static async Task<TInterface?> HostAsync<TInterface, TImplementation>(this IRootObjectHost This) where TImplementation : TInterface where TInterface : class {
            var Key = typeof(TImplementation);
            var EP = await This.HostEndpointAsync(Key);

            var ret = await ProtocolProvider.TryConnectAsync<TInterface>(EP);

            return ret;
        }

    }

}
