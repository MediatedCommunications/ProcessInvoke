using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zyan.Communication;
using Zyan.Communication.Protocols.Ipc;

namespace ProcessInvoke {
    public interface IProcessHost : IDisposable {
        void Stop();

        string Echo(string Message);

        I Register<I, T>();

    }

    public class ProcessHost : IProcessHost {
        public string Name { get; private set; }
        public string Port { get; private set; }
        public CancellationToken StartToken { get; private set; }
        private CancellationTokenSource StartTokenSource { get; set; }
        public void Stop() {
            StartTokenSource.Cancel();
        }

        public I Register<I, T>() {
            var ret = default(I);
            var Instance = Activator.CreateInstance<T>();
            if (Instance is I R) {
                ret = R;
            }

            this.Host.RegisterComponent<I, T>(Instance);

            return ret;
        }


        public ProcessHost(ProcessServerOptions Options) {
            Name = Options.ListenOn_Host;
            Port = Options.ListenOn_Port;

            var Provider = new Zyan.Communication.Protocols.Ipc.IpcBinaryServerProtocolSetup(Port);

            this.Host = new Zyan.Communication.ZyanComponentHost(Name, Provider);
            this.Host.RegisterComponent<IProcessHost, ProcessHost>(this);

            StartTokenSource = new CancellationTokenSource();
            StartToken = StartTokenSource.Token;

        }

        protected ZyanComponentHost Host { get; private set; }

        public string Echo(string Message) {
            Console.WriteLine(Message);

            return $@"""{Message}"" Received!";
        }

        ~ProcessHost() {
            Dispose();
        }

        public void Dispose() {
            Host?.Dispose();
        }
    }

    public class ProcessClient : IProcessHost {
        public Process Process { get; private set; }
        public IProcessHost Proxy { get; private set; }
        public ProcessServerOptions ServerOptions { get; private set; }
        public ProcessClientOptions ClientOptions { get; private set; }

        public ProcessClient(Process Process, ProcessServerOptions ServerOptions, ProcessClientOptions ClientOptions) {
            this.Process = Process;
            this.ServerOptions = ServerOptions;
            this.ClientOptions = ClientOptions;
           
            var protocol = new IpcBinaryClientProtocolSetup();
            var url = protocol.FormatUrl(ServerOptions.ListenOn_Port, ServerOptions.ListenOn_Host);

            var SW = System.Diagnostics.Stopwatch.StartNew();

            var Exception = default(Exception);

            while(this.Proxy == null && SW.Elapsed < ClientOptions.OnConnect_TimeOut) {
                try {
                    var connection = new ZyanConnection(url, protocol);
                    this.Proxy = connection.CreateProxy<IProcessHost>();
                } catch (Exception ex) {
                    Exception = ex;
                }
            }

            if(this.Proxy == null) {
                throw Exception;
            }

        }

        public void Stop() {
            this.Proxy.Stop();
        }

        public string Echo(string Message) {
            return this.Proxy.Echo(Message);
        }

        public I Register<I, T>() {
            return this.Proxy.Register<I, T>();
        }

        private bool __Disposed = false;
        public void Dispose() {
            if (!__Disposed) {
                if (ClientOptions.OnDispose_Terminate) {
                    try {
                        Stop();
                    } catch(Exception ex) {
                        ex.Equals(ex);
                    }

                    try {
                        if (!Process.HasExited) {
                            Process.Kill();
                        }
                    } catch (Exception ex) {
                        ex.Equals(ex);
                    }

                }
            }
            __Disposed = true;
        }

        ~ProcessClient() {
            Dispose();
        }
    }

}
