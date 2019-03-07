using System.Reflection;

namespace ProcessInvoke {

    public class CurrentUserProcessInvoker : ProcessInvoker {

        public static CurrentUserProcessInvoker Instance { get; private set; } = new CurrentUserProcessInvoker();

    }

}
