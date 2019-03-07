using System.Reflection;

namespace ProcessInvoke {
    public class AdministratorProcessInvoker : ProcessInvoker {
        public static AdministratorProcessInvoker Instance { get; private set; } = new AdministratorProcessInvoker();
    }

    

}
