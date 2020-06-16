using ProcessInvoke.Hosting;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ProcessInvoke {
    public interface IRootObjectHost {
        Task<Endpoint?> CloneEndpointAsync();
        Task<Endpoint?> HostEndpointAsync(Type Key);
        Task StopAsync();
    }

    public static class IRootObjectHostExtensions {

        public static async Task<IRootObjectHost?> CloneAsync(this IRootObjectHost This) {
            var EP = await This.CloneEndpointAsync()
                .DefaultAwait()
                ;

            var ret = await Protocols.ProtocolProvider.TryConnectAsync<IRootObjectHost>(EP)
                .DefaultAwait()
                ;

            return ret;
        }

        public static async Task<TInterface?> HostAsync<TInterface, TImplementation>(this IRootObjectHost This) where TImplementation : TInterface where TInterface : class {
            var Key = typeof(TImplementation);
            var EP = await This.HostEndpointAsync(Key)
                .DefaultAwait()
                ;

            var ret = await Protocols.ProtocolProvider.TryConnectAsync<TInterface>(EP)
                .DefaultAwait()
                ;

            return ret;
        }

    }

}
