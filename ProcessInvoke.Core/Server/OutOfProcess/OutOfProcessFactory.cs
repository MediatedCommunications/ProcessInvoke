using ProcessInvoke.Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {

    public interface IOutOfProcessFactory {
        Task<IOutOfProcessController> StartAsync(OutOfProcessServerOptions? ServerOptions = default, OutOfProcessClientOptions? ClientOptions = default);
    }

    public abstract class OutOfProcessFactory : IOutOfProcessFactory {

        protected virtual Assembly AssemblyToLaunch() {
            return this.GetType().Assembly;
        }

        protected virtual string FileNameToLaunch() {
            var P = AssemblyToLaunch();
            var ret = P.Location;


            if(ret.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)) {
                var Test = System.IO.Path.ChangeExtension(ret, ".exe");

                if (System.IO.File.Exists(Test)) {
                    ret = Test;
                }

            }


            return ret;
        }

        protected virtual Task StartHostAsync(OutOfProcessServerOptions Options) {
            var FN = FileNameToLaunch();

            var WindowStyle = Debugger.IsAttached
                ? ProcessWindowStyle.Normal
                : ProcessWindowStyle.Hidden
                ;

            var PSI = new System.Diagnostics.ProcessStartInfo() {
                FileName = FN,
                UseShellExecute = true,
                Arguments = Options.ToString(),
                WindowStyle = WindowStyle,
            };

            System.Diagnostics.Process.Start(PSI);

            return Task.CompletedTask;
        }

        protected virtual OutOfProcessServerOptions DefaultServerOptions() {
            var ret = new OutOfProcessServerOptions() {
                ListenOn_Provider = ProtocolProvider.Default,
                ListenOn_Host = $@"{Guid.NewGuid()}",
                ListenOn_Port = $@"{Guid.NewGuid()}",
                ListenOn_Key = $@"{Guid.NewGuid()}",
                ParentProcess_ID = Environment.ProcessId,
                Terminate_OnParentProcessExit = true,
                Terminate_OnStop = true,
            };

            return ret;
        }

        protected virtual OutOfProcessClientOptions DefaultClientOptions() {
            var ret = new OutOfProcessClientOptions() {
                OnDispose_Stop = true,
                OnDispose_Kill = true,
                OnConnect_Attempts_TotalTimeOut = TimeSpan.FromSeconds(10),
            };

            return ret;
        }

        public async Task<IOutOfProcessController> StartAsync(OutOfProcessServerOptions? ServerOptions = default, OutOfProcessClientOptions? ClientOptions = default) {
            ServerOptions ??= DefaultServerOptions();
            ServerOptions = ServerOptions.Clone();

            ClientOptions ??= DefaultClientOptions();
            ClientOptions = ClientOptions.Clone();

            if (ServerOptions.ParentProcess_ID == null) {
                ServerOptions.ParentProcess_ID = Environment.ProcessId;
            }

            await StartHostAsync(ServerOptions)
                .DefaultAwait()
                ;

            var Provider = ProtocolProvider.GetProvider(ServerOptions.ListenOn_Provider);
            var ret = await Provider.ConnectAsync<IOutOfProcessController>(ServerOptions.ToEndpoint(), ClientOptions)
                .DefaultAwait()
                ;

            return ret;
        }


    }
}
