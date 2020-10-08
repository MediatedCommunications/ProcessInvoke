using ProcessInvoke.Server.OutOfProcess;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace ProcessInvoke.Protocols {

    public static class ProtocolProvider {
        public static string Default { get; private set; } = typeof(Protocols.NamedPipes.NamedPipeProtocolProvider).AssemblyQualifiedName ?? string.Empty;

        public static IProtocolProvider GetProvider(string? ProviderName) {
            var ret = TryGetProvider(ProviderName);

            if(ret == default) {
                throw new ArgumentOutOfRangeException(nameof(ProviderName), $@"Unable to find {ProviderName}");
            }

            return ret;
        }

        public static IProtocolProvider? TryGetProvider(string? ProviderName) {
            var ret = default(IProtocolProvider?);
            
            var NewProviderName = Default;
            if (!string.IsNullOrWhiteSpace(ProviderName)) {
                NewProviderName = ProviderName;
            }

            if (Type.GetType(NewProviderName) is { } T) {
                ret = Activator.CreateInstance(T) as IProtocolProvider;
            }

            return ret;
        }

        public static async Task<T?> TryConnectAsync<T>(Endpoint? Endpoint, OutOfProcessClientOptions? Options = default) where T : class {
            var ret = default(T);

            if (Endpoint is { } V1 && TryGetProvider(Endpoint?.Provider) is { } V2) {
                try {
                    ret = await V2.ConnectAsync<T>(V1, Options)
                        .DefaultAwait()
                        ;
                } catch {

                }
            }

            return ret;
        }



    }
}
