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

                    var Delays = new LinkedList<Task>();

                    var AddAll = !(ProcessOptions.Terminate_OnStop || ProcessOptions.Terminate_OnParentProcessExit);

                    if(AddAll || ProcessOptions.Terminate_OnStop) {
                        Delays.AddLast(DelayTask_Stop(Host.StartToken));
                    }

                    if((AddAll || ProcessOptions.Terminate_OnParentProcessExit) && ProcessOptions.ParentProcess_ID is int ProcessID) {
                        Delays.AddLast(DelayTask_ParentProcess(ProcessID));
                    }

                    await Task.WhenAny(Delays)
                        .ConfigureAwait(false)
                        ;

                }
            } catch(Exception ex) {
                ex.Equals(ex);
            }



            return ret;
        }


        static Task DelayTask_Stop(CancellationToken Token) {
            var ret = Task.Run(async() => {

                try {
                    await Task.Delay(Timeout.InfiniteTimeSpan, Token)
                        .ConfigureAwait(false)
                        ;
                } catch {

                }
            });

            return ret;
        }

        static Task DelayTask_ParentProcess(int ProcessId) {
            var ret = Task.Run(() => {
                try {
                    var Parent = System.Diagnostics.Process.GetProcessById(ProcessId);
                    Parent?.WaitForExit();


                } catch (Exception ex) {
                    ex.Equals(ex);
                }
            });


            return ret;
        }

    }

    

    

}
