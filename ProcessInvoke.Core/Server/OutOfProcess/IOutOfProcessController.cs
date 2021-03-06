﻿using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {

    public interface IOutOfProcessController : IMultiIpcServer, ICloneableIpcServer, IStoppableIpcServer {
        
    }

    public static class IRootObjectHostExtensions {

        public static async Task<IOutOfProcessController?> CloneAsync(this ICloneableIpcServer This) {
            var EP = await This.CloneEndpointAsync()
                .DefaultAwait()
                ;

            var ret = await Protocols.ProtocolProvider.TryConnectAsync<IOutOfProcessController>(EP)
                .DefaultAwait()
                ;

            return ret;
        }

        public static async Task<TInterface?> HostAsync<TInterface, TImplementation>(this IOutOfProcessController This) where TImplementation : TInterface where TInterface : class {
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
