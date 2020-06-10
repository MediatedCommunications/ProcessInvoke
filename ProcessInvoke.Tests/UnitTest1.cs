using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProcessInvoke.Tests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void CurrentUser() {
            TestInvoke(CurrentUserProcessInvoker.Instance);
        }

        [TestMethod]
        public void HighestAvailable() {
            TestInvoke(HighestAvailableProcessInvoker.Instance);
        }

        [TestMethod]
        public void AdministratorUser() {
            TestInvoke(AdministratorProcessInvoker.Instance);
        }

        private void TestInvoke(ProcessInvoker Invoker) {
            var Host = Invoker.TryStart();

            var Connection = Host.GetConnection();
            var Service = Connection.Register<IRemoteObject, RemoteObject>();

            var MyProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;
            var RemoteProcessID = Service.HostingProcessId();

            Assert.AreNotEqual(MyProcessID, RemoteProcessID);

        }

    }


    public interface IRemoteObject {
        int HostingProcessId();
    }

    public class RemoteObject : MarshalByRefObject, IRemoteObject {
        public int HostingProcessId() {
            return System.Diagnostics.Process.GetCurrentProcess().Id;
        }
    }

}
