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

        private static async Task TestInvokingAsync(OutOfProcessFactory Invoker) {
            var Host = await Invoker.StartAsync();

            var RemoteObject = await Host.HostAsync<IRemoteTestObject, RemoteTestObject>();
            
            Assert.IsNotNull(RemoteObject);
            
            if (RemoteObject is { }) {

                var RemoteProcessID = await RemoteObject.HostingProcessIdAsync();

                var MyProcessID = System.Environment.ProcessId;

                Assert.AreNotEqual(MyProcessID, RemoteProcessID);
            }

        }

        [Test]
        public Task TestCloningAsync() {
            return TestCloningAsync(GetFactory());
        }

        private static async Task TestCloningAsync(OutOfProcessFactory Invoker) {
            var RemoteObject0 = new RemoteTestObject();

            var Host1 = await Invoker.StartAsync();
            var RemoteObject1 = await Host1.HostAsync<IRemoteTestObject, RemoteTestObject>();

            var Host2 = await Host1.CloneAsync();

            Assert.IsNotNull(Host1);
            Assert.IsNotNull(Host2);

            if (Host2 is { }) {

                var RemoteObject2 = await Host2.HostAsync<IRemoteTestObject, RemoteTestObject>();

                Assert.IsNotNull(RemoteObject0);
                Assert.IsNotNull(RemoteObject1);
                Assert.IsNotNull(RemoteObject2);

                if (RemoteObject0 is { } && RemoteObject1 is { } && RemoteObject2 is { }) {

                    var V0 = await RemoteObject0.HostingProcessIdAsync();
                    var V1 = await RemoteObject1.HostingProcessIdAsync();
                    var V2 = await RemoteObject2.HostingProcessIdAsync();

                    Assert.AreNotEqual(V0, V1);
                    Assert.AreNotEqual(V0, V2);
                    Assert.AreNotEqual(V1, V2);
                }
            }
        }

        [Test]
        public Task TestDerivedClasses_None_Async() {
            return TestDerivedClassesAsync(GetFactory(), ResultType.None);
        }

        [Test]
        public Task TestDerivedClasses_Base_Async() {
            return TestDerivedClassesAsync(GetFactory(), ResultType.BaseResult);
        }

        [Test]
        public Task TestDerivedClasses_Derived_Async() {
            return TestDerivedClassesAsync(GetFactory(), ResultType.DerivedResult);
        }

        private static async Task TestDerivedClassesAsync(OutOfProcessFactory Invoker, params ResultType[] Tests) {
            var Host = await Invoker.StartAsync();

            var RemoteObject = await Host.HostAsync<IRemoteTestObject, RemoteTestObject>();

            Assert.IsNotNull(RemoteObject);
            if (RemoteObject is { }) {

                foreach (var Test in Tests) {
                    var Value = await RemoteObject.GetValueAsync(Test);

                    Assert.AreEqual(Test, Value.GetTypeProperty(), "Type Property Does Not Match");
                    Assert.AreEqual(Test, Value.GetResultType(), "Object Type Does Not Match");

                }

            }

        }



    }

}
