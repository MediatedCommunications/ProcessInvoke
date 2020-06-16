using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessInvoke.Core {
    public abstract class ObjectHostBase {
        protected HostedObjectEndpoint Options { get; private set; }
        protected CancellationTokenSource? Source { get; private set; }

        public ObjectHostBase(HostedObjectEndpoint Options) {
            this.Options = Options;
        }

        public event EventHandler? Stopped;

        public Task StopAsync() {
            Source?.Cancel();
            Source = default;

            Stopped?.Invoke(this, default);

            return Task.CompletedTask;
        }

        public Task<HostedObjectEndpoint> StartHostingAsync() {
            if(Source != default) {
                throw new InvalidOperationException("Hosting the object has already started");
            }
            Source = new CancellationTokenSource();

            var ret = new HostedObjectEndpoint(
                Options.Provider,
                Options.Host,
                Options.Port,
                Options.Key
            );

            
            _ = Task.Run(() => StartListeningAsync(ret));

            return Task.FromResult(ret);
        }

        protected async Task StartListeningAsync(HostedObjectEndpoint EP) {
            var P = ProtocolProvider.GetProvider(EP.Provider);

            if(P is { } Provider && Source?.Token is { } Token) {
                await Provider.StartListeningAsync(EP, Token, GetHandler);
            }

            
        }



        protected abstract object GetHandler();


    }
}
