using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zyan.Communication;

namespace ProcessInvoke {


    public static class Program {

        public static async Task<int> Main(string[] args) {
            var ret = -1;

            var Options = ProcessServerOptions.Parse(args, out var OptionsSet);
            if (!Options.Valid()) {
                OptionsSet.WriteOptionDescriptions(System.Console.Out);
            } else {
                ret = await Main(Options);
            }

            return ret;
        }

        static async Task<int> Main(ProcessServerOptions ProcessOptions) {
            var ret = -1;
            var Host = default(ProcessHost);

            try {
                Host = new ProcessHost(ProcessOptions);
                ret = 0;
            } catch (Exception ex) {
                ex.Equals(ex);
            }

            try {
                if (Host != null) {

                    if (ProcessOptions.ParentProcess_WaitForExit && ProcessOptions.ParentProcess_ID is int ProcessID) {
                        try {
                            var Parent = System.Diagnostics.Process.GetProcessById(ProcessID);
                            Parent?.WaitForExit();

                        } catch (Exception ex) {
                            ex.Equals(ex);
                        }
                    } else {
                        await Task.Delay(Timeout.InfiniteTimeSpan, Host.StartToken)
                            .ConfigureAwait(false)
                            ;
                    }
                }
            } catch(Exception ex) {
                ex.Equals(ex);
            }



            return ret;
        }

    }

    

}
