using NUnit.Framework;
using ProcessInvoke.Protocols;
using ProcessInvoke.Protocols.NamedPipes;
using System;
using System.Threading.Tasks;
using System.Windows.Markup;
using ProcessInvoke.Server;

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


    }

}
