using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {
    public class Program {

        public static class Default {
            public static Task<int> MainAsync(string[] args) {
                var Instance = new Program();
                return Instance.MainAsync(args);
            }
        }


        protected virtual OutOfProcessServerOptions ParseOptions(string[] args, out Mono.Options.OptionSet Options) {
            var ret = new OutOfProcessServerOptions();
            ret.Parse(args, out Options);

            return ret;
        }

        public virtual async Task<int> MainAsync(string[] args) {
            //LocalPathAssemblyResolver.Instance.Enabled = true;
            
            var ret = -1;

            var Options = ParseOptions(args, out var OptionsSet);
            if (!Options.Valid()) {
                OptionsSet.WriteOptionDescriptions(System.Console.Out);
            } else {
                ret = await MainAsync(Options)
                    .DefaultAwait()
                    ;
            }

            return ret;
        }

        protected virtual async Task<int> MainAsync(OutOfProcessServerOptions ProcessOptions) {
            var ret = -1;
            var Host = default(IpcServerBase);
            try {
                Host = OutOfProcessIpcServer.Create(ProcessOptions);

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


        protected virtual Task DelayTask_StopAsync(IpcServerBase Host) {
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



        protected virtual Task DelayTask_ParentProcessAsync(int ProcessId) {
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
