using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessInvoke.Hosting.Process {
    public static class Program {

        public static async Task<int> MainAsync(string[] args) {
            var ret = -1;

            var Options = ProcessHostOptions.Parse(args, out var OptionsSet);
            if (!Options.Valid()) {
                OptionsSet.WriteOptionDescriptions(System.Console.Out);
            } else {
                ret = await MainAsync(Options)
                    .DefaultAwait()
                    ;
            }

            return ret;
        }

        static async Task<int> MainAsync(ProcessHostOptions ProcessOptions) {
            var ret = -1;
            var Host = default(RootObjectHost);
            try {
                Host = new RootObjectHost(ProcessOptions);

                await Host.StartHostingAsync()
                    .DefaultAwait()
                    ;


                ret = 0;
            } catch (Exception ex) {
                ex.Equals(ex);
            }

            try {
                if (Host != null) {

                    var Delays = new LinkedList<Task>();

                    var AddAll = !(ProcessOptions.Terminate_OnStop || ProcessOptions.Terminate_OnParentProcessExit);

                    if (AddAll || ProcessOptions.Terminate_OnStop) {
                        Delays.AddLast(DelayTask_StopAsync(Host));
                    }

                    if ((AddAll || ProcessOptions.Terminate_OnParentProcessExit) && ProcessOptions.ParentProcess_ID is int ProcessID) {
                        Delays.AddLast(DelayTask_ParentProcessAsync(ProcessID));
                    }

                    await Task.WhenAny(Delays)
                        .DefaultAwait()
                        ;

                }
            } catch (Exception ex) {
                ex.Equals(ex);
            }



            return ret;
        }

        
        static Task DelayTask_StopAsync(RootObjectHost Host) {
            var CTS = new CancellationTokenSource();
            Host.Stopped += (x, y) => {
                CTS?.Cancel();
            };

            var ret = Task.Run(async () => {

                try {
                    await Task.Delay(Timeout.InfiniteTimeSpan, CTS.Token)
                        .DefaultAwait()
                        ;
                } catch {

                }
            });

            return ret;
        }

        

        static Task DelayTask_ParentProcessAsync(int ProcessId) {
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
