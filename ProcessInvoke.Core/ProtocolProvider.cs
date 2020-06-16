using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessInvoke {

    public interface IProtocolProvider {
        Task<T> ConnectAsync<T>(HostedObjectEndpoint Endpoint, ProcessClientOptions? Options = default) where T : class;

        Task StartListeningAsync(HostedObjectEndpoint Endpoint, CancellationToken Token, Func<Object> GetHandler);

    }

    public static class ProtocolProvider {
        public static string Default { get; private set; } = typeof(Providers.NamedPipes.NamedPipeProtocolProvider).AssemblyQualifiedName;

        public static IProtocolProvider? GetProvider(string? ProviderName) {
            var NewProviderName = Default;
            if (!string.IsNullOrWhiteSpace(ProviderName)) {
                NewProviderName = ProviderName;
            }

            var T = Type.GetType(NewProviderName);
            var ret = Activator.CreateInstance(T) as IProtocolProvider;

            return ret;
        }

        public static IProtocolProvider? TryGetProvider(string? ProviderName) {
            var ret = default(IProtocolProvider);
            try {
                ret = GetProvider(ProviderName);
            } catch { 
            
            }
            
            return ret;
        }

        public static async Task<T?> TryConnectAsync<T>(HostedObjectEndpoint? Endpoint, ProcessClientOptions? Options = default) where T : class {
            var ret = default(T);

            if (Endpoint is { } V1 && TryGetProvider(Endpoint?.Provider) is { } V2) {
                try {
                    ret = await V2.ConnectAsync<T>(V1, Options);
                } catch {

                }
            }

            return ret;
        }



    }
}
