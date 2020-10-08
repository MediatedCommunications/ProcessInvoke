using System;
using System.Threading.Tasks;

namespace ProcessInvoke.Security.CurrentUser {
    static class Program {
        public static Task<int> Main(string[] args) {
            return ProcessInvoke.Server.OutOfProcess.Program.Default.MainAsync(args);
        }
    }
}
