using ProcessInvoke.Server.OutOfProcess;
using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessInvoke.Protocols.NamedPipes
{
    public class NamedPipeProtocolProvider : IProtocolProvider {

        public virtual string StreamName(Endpoint EP) {
            var ret = $@"{EP.Provider}-{EP.Host}-{EP.Port}-{EP.Key}";
            return ret;
        }


        public virtual async Task<T> ConnectAsync<T>(Endpoint Endpoint, OutOfProcessClientOptions? Options = default) where T : class {
            var EP = StreamName(Endpoint);
            var C = new NamedPipeClientStream(".", EP, PipeDirection.InOut, PipeOptions.Asynchronous);

            var Delay = Timeout.Infinite;
            if (Options?.OnConnect_Attempts_TotalTimeOut > TimeSpan.Zero) {
                Delay = (int)Options.OnConnect_Attempts_TotalTimeOut.TotalMilliseconds;
            }

            await C.ConnectAsync(Delay)
                .DefaultAwait()
                ;

            var ret = StreamJsonRpc.JsonRpc.Attach<T>(C);
            return ret;
        }



        private NamedPipeServerStream CreatePipe(string EP) {
            var ret = default(NamedPipeServerStream?);

            if (OperatingSystem.IsWindows()) {

                var Security = new PipeSecurity();
                var Everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                var AllowEveryone = new PipeAccessRule(Everyone, PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                Security.SetAccessRule(AllowEveryone);

                ret = NamedPipeServerStreamAcl.Create(EP, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, Security);
            } else {
                ret = new NamedPipeServerStream(EP, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0);
            }

            return ret;
        }

        public virtual async Task StartListeningAsync(Endpoint Endpoint, CancellationToken Token, Func<Object> GetHandler) {
            var EP = StreamName(Endpoint);

            while (!Token.IsCancellationRequested) {
                try {

                    var C = CreatePipe(EP);
                   
                    await C.WaitForConnectionAsync(Token)
                        .DefaultAwait()
                        ;

                    _ = Task.Run(() => ProcessConnectionAsync(C, GetHandler));

                } catch (TaskCanceledException ex) {
                    ex.Ignore();
                }
            }
        }

        protected virtual async Task ProcessConnectionAsync(Stream S, Func<Object> GetHandler) {

            var Instance = GetHandler();
            
            var RPC = JsonRpc.Attach(S, Instance);
            await RPC.Completion
                .DefaultAwait()
                ;
        }
    }
}
