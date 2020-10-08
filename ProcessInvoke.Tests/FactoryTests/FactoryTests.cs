using NUnit.Framework;
using System.Threading.Tasks;
using ProcessInvoke.Server.OutOfProcess;

namespace ProcessInvoke.Tests {
    public abstract class FactoryTests {
        protected abstract OutOfProcessFactory GetFactory();


        [Test]
        public Task TestInvokingAsync() {
            return TestInvokingAsync(GetFactory());
        }

        private async Task TestInvokingAsync(OutOfProcessFactory Invoker) {
            var Host = await Invoker.StartAsync();

            var RemoteObject = await Host.HostAsync<IRemoteObject, RemoteObject>();
            var RemoteProcessID = await RemoteObject.HostingProcessIdAsync();

            var MyProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;

            Assert.AreNotEqual(MyProcessID, RemoteProcessID);

        }

        [Test]
        public Task TestCloningAsync() {
            return TestCloningAsync(GetFactory());
        }

        private async Task TestCloningAsync(OutOfProcessFactory Invoker) {
            var RemoteObject0 = new RemoteObject();

            var Host1 = await Invoker.StartAsync();
            var RemoteObject1 = await Host1.HostAsync<IRemoteObject, RemoteObject>();

            var Host2 = await Host1.CloneAsync();
            var RemoteObject2 = await Host2.HostAsync<IRemoteObject, RemoteObject>();

            var V0 = await RemoteObject0.HostingProcessIdAsync();
            var V1 = await RemoteObject1.HostingProcessIdAsync();
            var V2 = await RemoteObject2.HostingProcessIdAsync();

            Assert.AreNotEqual(V0, V1);
            Assert.AreNotEqual(V0, V2);
            Assert.AreNotEqual(V1, V2);
        }

    }

}
