﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {
    class CurrentProcessProcessHost : OutOfProcessFactory {

        public static CurrentProcessProcessHost Instance { get; private set; } = new CurrentProcessProcessHost();

        protected override Assembly AssemblyToLaunch() {
            return System.Reflection.Assembly.GetEntryAssembly();
        }


    }
}
