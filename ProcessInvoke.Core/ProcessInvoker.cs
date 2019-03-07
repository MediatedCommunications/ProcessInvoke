using System;
using System.Diagnostics;
using System.Reflection;

namespace ProcessInvoke {
    public abstract class ProcessInvoker {

        protected virtual Assembly AssemblyToLaunch() {
            return this.GetType().Assembly;
        }

        protected virtual Process StartProcess(ProcessServerOptions Options) {
            var P = AssemblyToLaunch();
            var FN = P.Location;

            var PSI = new System.Diagnostics.ProcessStartInfo() {
                FileName = FN,
                UseShellExecute = true,
                Arguments = Options.ToString(),
            };

            var Proc = System.Diagnostics.Process.Start(PSI);

            return Proc;
        }

        protected virtual ProcessServerOptions DefaultServerOptions() {
            var ret = new ProcessServerOptions() {
                ListenOn_Host = $@"{Guid.NewGuid()}_{Guid.NewGuid()}",
                ListenOn_Port = $@"{Guid.NewGuid()}_{Guid.NewGuid()}",
                ParentProcess_ID = System.Diagnostics.Process.GetCurrentProcess().Id,
                ParentProcess_WaitForExit = true,
            };

            return ret;
        }

        protected virtual ProcessClientOptions DefaultClientOptions() {
            var ret = new ProcessClientOptions() {
                OnDispose_Terminate = true,
                OnConnect_TimeOut = TimeSpan.FromSeconds(5)
            };

            return ret;
        }

        public IProcessHost TryStart() {
            return TryStart(null, null);
        }

        public IProcessHost TryStart(ProcessServerOptions ServerOptions, ProcessClientOptions ClientOptions) {
            TryStart(ServerOptions, ClientOptions, out var ret);
            return ret;
        }

        public bool TryStart(out IProcessHost Host) {
            return TryStart(null, null, out Host);
        }

        public bool TryStart(ProcessServerOptions ServerOptions, ProcessClientOptions ClientOptions, out IProcessHost Host) {
            var ret = false;
            Host = null;
            try {
                Host = Start(ServerOptions, ClientOptions);
                ret = true;
            } catch(Exception ex) {
                ex.Equals(ex);
            }
            return ret;
        }

        public IProcessHost Start() {
            return Start(null, null);
        }

        public IProcessHost Start(ProcessServerOptions ServerOptions, ProcessClientOptions ClientOptions) {
            ServerOptions = ServerOptions ?? DefaultServerOptions();
            ServerOptions = ServerOptions.Clone();

            ClientOptions = ClientOptions ?? DefaultClientOptions();
            ClientOptions = ClientOptions.Clone();

            if(ServerOptions.ParentProcess_ID == null) {
                ServerOptions.ParentProcess_ID = System.Diagnostics.Process.GetCurrentProcess().Id;
            }

            var Process = StartProcess(ServerOptions);

            var ret = new ProcessClient(Process, ServerOptions, ClientOptions);

            return ret;
        }

    }


}
