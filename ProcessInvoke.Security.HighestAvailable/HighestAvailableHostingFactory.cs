using System.Reflection;

namespace ProcessInvoke {
    public class HighestAvailableHostingFactory : HostingFactory {
        public static HighestAvailableHostingFactory Instance { get; private set; } = new HighestAvailableHostingFactory();
    }

}
