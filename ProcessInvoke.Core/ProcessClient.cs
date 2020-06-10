using System;
using System.Diagnostics;
using Zyan.Communication;
using Zyan.Communication.Protocols.Ipc;

namespace ProcessInvoke {

    public class ProcessClient {
        public Process Process { get; private set; }
        public IProcessHost Proxy { get; private set; }
        public ProcessServerOptions ServerOptions { get; private set; }
        public ProcessClientOptions ClientOptions { get; private set; }

        public IProcessHost GetConnection() {
            var protocol = new IpcBinaryClientProtocolSetup();
            var url = protocol.FormatUrl(ServerOptions.ListenOn_Port, ServerOptions.ListenOn_Host);

            var SW = System.Diagnostics.Stopwatch.StartNew();

            var Exception = default(Exception);

            var ret = default(IProcessHost);

            for (var ConnectionAttempts = 0; SW.Elapsed < ClientOptions.OnConnect_Attempts_TotalTimeOut || ConnectionAttempts < ClientOptions.OnConnect_Attempts_Minimum; ConnectionAttempts += 1) {
                try {
                    var connection = new ZyanConnection(url, protocol);
                    connection.PollingEnabled = true;
                    ret = connection.CreateProxy<IProcessHost>();
                    break;
                } catch (Exception ex) {
                    Exception = ex;
                }
            }

            if (ret is null) {
                throw Exception;
            }

            return ret;
        }


        public ProcessClient(Process Process, ProcessServerOptions ServerOptions, ProcessClientOptions ClientOptions) {
            this.Process = Process;
            this.ServerOptions = ServerOptions;
            this.ClientOptions = ClientOptions;
        }


        private bool __Disposed = false;
        public void Dispose() {
            if (!__Disposed) {

                if(ClientOptions?.OnDispose_Kill == true) {
                    try {
                        if (Process?.HasExited == false) {
                            Process.Kill();
                        }
                    } catch (Exception ex) {
                        ex.Equals(ex);
                    }
                }


                GC.SuppressFinalize(this);

            }
            __Disposed = true;
        }

        ~ProcessClient() {
            Dispose();
        }
    }

}
