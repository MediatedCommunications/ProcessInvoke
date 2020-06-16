using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessInvoke.Providers.NamedPipes
{
    public class NamedPipeProtocolProvider : IProtocolProvider {
        public async Task<T> ConnectAsync<T>(HostedObjectEndpoint Endpoint, ProcessClientOptions? Options = default) where T : class {
            var StreamName = Endpoint.StreamName();
            var C = new NamedPipeClientStream(".", StreamName, PipeDirection.InOut, PipeOptions.Asynchronous);

            var Delay = Timeout.Infinite;
            if (Options?.OnConnect_Attempts_TotalTimeOut > TimeSpan.Zero) {
                Delay = (int)Options.OnConnect_Attempts_TotalTimeOut.TotalMilliseconds;
            }

            await C.ConnectAsync(Delay);

            var ret = StreamJsonRpc.JsonRpc.Attach<T>(C);
            return ret;
        }

        public async Task StartListeningAsync(HostedObjectEndpoint Endpoint, CancellationToken Token, Func<Object> GetHandler) {
            var EP = Endpoint.StreamName();

            while (true) {
                
                if (!Token.IsCancellationRequested) {
                    try {
                        var Security = new PipeSecurity();
                        {
                            var Everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                            var AllowEveryone = new PipeAccessRule(Everyone, PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                            Security.SetAccessRule(AllowEveryone);
                        }

                        var C = new NamedPipeServerStream(EP, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, Security);
                        await C.WaitForConnectionAsync(Token);
                        _ = Task.Run(() => ProcessConnectionAsync(C, GetHandler));
                    } catch (TaskCanceledException) {

                    }
                } else {
                    break;
                }
            }

        }

        protected async Task ProcessConnectionAsync(Stream S, Func<Object> GetHandler) {
            var Instance = GetHandler();
            var RPC = JsonRpc.Attach(S, Instance);
            await RPC.Completion;
        }
    }
}
