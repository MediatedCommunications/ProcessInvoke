using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProcessInvoke {
    public abstract class HostingFactory {

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

        protected virtual Process StartProcess(ProcessHostOptions Options) {
            var FN = FileNameToLaunch();

            var PSI = new System.Diagnostics.ProcessStartInfo() {
                FileName = FN,
                UseShellExecute = true,
                Arguments = Options.ToString(),
            };

            var Proc = System.Diagnostics.Process.Start(PSI);

            return Proc;
        }

        protected virtual ProcessHostOptions DefaultServerOptions() {
            var ret = new ProcessHostOptions() {
                ListenOn_Provider = ProtocolProvider.Default,
                ListenOn_Host = $@"{Guid.NewGuid()}",
                ListenOn_Port = $@"{Guid.NewGuid()}",
                ListenOn_Key = $@"{Guid.NewGuid()}",
                ParentProcess_ID = System.Diagnostics.Process.GetCurrentProcess().Id,
                Terminate_OnParentProcessExit = true,
                Terminate_OnStop = true,
            };

            return ret;
        }

        protected virtual ProcessClientOptions DefaultClientOptions() {
            var ret = new ProcessClientOptions() {
                OnDispose_Stop = true,
                OnDispose_Kill = true,
                OnConnect_Attempts_TotalTimeOut = TimeSpan.FromSeconds(10),
                OnConnect_Attempts_Minimum = 5,
            };

            return ret;
        }

        public async Task<IRootObjectHost> StartAsync(ProcessHostOptions? ServerOptions = default, ProcessClientOptions? ClientOptions = default) {
            ServerOptions = ServerOptions ?? DefaultServerOptions();
            ServerOptions = ServerOptions.Clone();

            ClientOptions = ClientOptions ?? DefaultClientOptions();
            ClientOptions = ClientOptions.Clone();

            if (ServerOptions.ParentProcess_ID == null) {
                ServerOptions.ParentProcess_ID = System.Diagnostics.Process.GetCurrentProcess().Id;
            }

            var Process = StartProcess(ServerOptions);

            var Provider = ProtocolProvider.GetProvider(ServerOptions.ListenOn_Provider);
            var ret = await Provider.ConnectAsync<IRootObjectHost>(ServerOptions.ToEndpoint(), ClientOptions);

            return ret;
        }


    }
}
