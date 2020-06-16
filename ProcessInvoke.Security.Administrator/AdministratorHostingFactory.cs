using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessInvoke {
    public class AdministratorHostingFactory : HostingFactory {
        public static AdministratorHostingFactory Instance { get; private set; } = new AdministratorHostingFactory();
    }
}
