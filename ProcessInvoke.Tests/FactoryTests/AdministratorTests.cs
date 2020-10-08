using NUnit.Framework;
using ProcessInvoke.Server.OutOfProcess;

namespace ProcessInvoke.Tests {
    [TestFixture]
    public class AdministratorTests : FactoryTests {
        protected override OutOfProcessFactory GetFactory() => AdministratorHostingFactory.Instance;
    }

}
