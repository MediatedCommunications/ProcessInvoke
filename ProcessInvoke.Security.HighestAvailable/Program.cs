using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessInvoke.Security.HighestAvailable {
    public static class Program {
        public static Task<int> Main(string[] args) {
            return ProcessInvoke.Program.Main(args);
        }
    }
}
