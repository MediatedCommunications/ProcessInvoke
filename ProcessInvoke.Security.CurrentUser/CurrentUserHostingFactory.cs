using System.Reflection;

namespace ProcessInvoke {

    public class CurrentUserHostingFactory : HostingFactory {

        public static CurrentUserHostingFactory Instance { get; private set; } = new CurrentUserHostingFactory();

    }

}
