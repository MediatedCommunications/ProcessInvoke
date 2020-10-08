using NUnit.Framework;
using ProcessInvoke.Protocols;
using ProcessInvoke.Protocols.NamedPipes;
using System;
using System.Threading.Tasks;
using System.Windows.Markup;
using ProcessInvoke.Server;
using ProcessInvoke.Server.OutOfProcess;
using System.Diagnostics;
using System.Linq;

namespace ProcessInvoke.Tests {

    [TestFixture]
    public class OtherTests {
      
         

        [Test]
        public async Task SelfHostingAsync() {
            var Endpoint = new Endpoint(ProtocolProvider.Default, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var Server = new IpcServer(Endpoint, new RemoteObject());
            await Server.StartHostingAsync();

            var Provider = new NamedPipeProtocolProvider();
            var Client = await Provider.ConnectAsync<IRemoteObject>(Endpoint);

            await Client.HostingProcessIdAsync();

        }

        [Test]
        public async Task TestInProcessFactoryAsync()
        {
            var Type = typeof(RemoteObject).AssemblyQualifiedName;


            var Host = await InProcessFactory.Instance.StartAsync();

            var RemoteObject = await Host.HostAsync<IRemoteObject, RemoteObject>();
            var RemoteProcessID = await RemoteObject.HostingProcessIdAsync();

            var MyProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;

            Assert.AreEqual(MyProcessID, RemoteProcessID);



        }


    }

    public class InProcessFactory : OutOfProcessFactory
    {
        public static InProcessFactory Instance { get; private set; } = new InProcessFactory();

        protected override Task StartHostAsync(OutOfProcessServerOptions Options)
        {
            var args = Options.ToList().Select(x => x.Replace($@"""", "")).ToArray();

            _ = Task.Run(() => ProcessInvoke.Server.OutOfProcess.Program.Default.MainAsync(args));

            return Task.CompletedTask;
        }
    }

}
