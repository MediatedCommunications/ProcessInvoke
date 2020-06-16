using System;
using System.Threading.Tasks;

namespace ProcessInvoke.Security.HighestAvailable {
    static class Program {
        public static Task<int> Main(string[] args) {
            return ProcessInvoke.Hosting.Process.Program.MainAsync(args);
        }
    }
}
