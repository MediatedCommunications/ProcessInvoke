using System.Reflection;

namespace ProcessInvoke.Server.OutOfProcess {
    public class HighestAvailableHostingFactory : OutOfProcessFactory {
        public static HighestAvailableHostingFactory Instance { get; private set; } = new HighestAvailableHostingFactory();
    }

}
