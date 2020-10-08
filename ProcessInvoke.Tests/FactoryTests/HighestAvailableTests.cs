using NUnit.Framework;
using ProcessInvoke.Server.OutOfProcess;

namespace ProcessInvoke.Tests {
    [TestFixture]
    public class HighestAvailableTests : FactoryTests {
        protected override OutOfProcessFactory GetFactory() => HighestAvailableHostingFactory.Instance;
    }

}
