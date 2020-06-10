using System;
using System.Threading;
using Zyan.Communication;

namespace ProcessInvoke {
    public class ProcessHost : IProcessHost {
        public string Name { get; private set; }
        public string Port { get; private set; }
        public CancellationToken StartToken { get; private set; }
        private CancellationTokenSource StartTokenSource { get; set; }
        public void Stop() {
            StartTokenSource.Cancel();
        }

        public TInterface Register<TInterface, TImplementation>() {
            var ret = default(TInterface);
            var Instance = Activator.CreateInstance<TImplementation>();
            if (Instance is TInterface R) {
                ret = R;
            }

            this.Host.RegisterComponent<TInterface, TImplementation>(Instance);

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

}
