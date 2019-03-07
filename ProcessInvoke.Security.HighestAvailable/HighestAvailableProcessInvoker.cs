using System.Reflection;

namespace ProcessInvoke {
    public class HighestAvailableProcessInvoker : ProcessInvoker {
        public static HighestAvailableProcessInvoker Instance { get; private set; } = new HighestAvailableProcessInvoker();
    }

    

}
