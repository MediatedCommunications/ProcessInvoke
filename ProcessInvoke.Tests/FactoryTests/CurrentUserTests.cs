using NUnit.Framework;
using ProcessInvoke.Server.OutOfProcess;

namespace ProcessInvoke.Tests {
    [TestFixture]
    public class CurrentUserTests : FactoryTests {
        protected override OutOfProcessFactory GetFactory() => CurrentUserHostingFactory.Instance;
    }

}
