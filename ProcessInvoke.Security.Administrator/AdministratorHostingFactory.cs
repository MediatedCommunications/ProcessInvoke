using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessInvoke.Server.OutOfProcess {
    public class AdministratorHostingFactory : OutOfProcessFactory {
        public static AdministratorHostingFactory Instance { get; private set; } = new AdministratorHostingFactory();
    }
}
