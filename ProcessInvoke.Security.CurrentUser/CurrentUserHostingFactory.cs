using System.Reflection;

namespace ProcessInvoke.Server.OutOfProcess {

    public class CurrentUserHostingFactory : OutOfProcessFactory {

        public static CurrentUserHostingFactory Instance { get; private set; } = new CurrentUserHostingFactory();

    }

}
