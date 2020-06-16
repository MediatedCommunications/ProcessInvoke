using NUnit.Framework;
using ProcessInvoke.Hosting.Process;
using System;
using System.Threading.Tasks;


namespace ProcessInvoke.Tests {
    [TestFixture]
    public class UnitTest1 {
        [Test]
        public Task CurrentUser() {
            return TestInvoke(CurrentUserHostingFactory.Instance);
        }

        [Test]
        public Task HighestAvailable() {
            return TestInvoke(HighestAvailableHostingFactory.Instance);
        }

        [Test]
        public Task AdministratorUser() {
            return TestInvoke(AdministratorHostingFactory.Instance);
        }

        private async Task TestInvoke(HostingFactory Invoker) {
            var Host = await Invoker.StartAsync();

            var RemoteObject = await Host.HostAsync<IRemoteObject, RemoteObject>();
            var RemoteProcessID = await RemoteObject.HostingProcessId();

            var MyProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;
            
            Assert.AreNotEqual(MyProcessID, RemoteProcessID);

        }

    }


    public interface IRemoteObject {
        Task<int> HostingProcessId();
    }

    public class RemoteObject : MarshalByRefObject, IRemoteObject {
        public Task<int> HostingProcessId() {
            var ret = System.Diagnostics.Process.GetCurrentProcess().Id;

            return Task.FromResult(ret);
        }
    }

}
