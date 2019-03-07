using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessInvoke {

    public class ProcessClientOptions {
        public bool OnDispose_Terminate { get; set; }

        public TimeSpan OnConnect_TimeOut { get; set; }

        public virtual ProcessClientOptions Clone() {
            var ret = new ProcessClientOptions() {
                OnDispose_Terminate = this.OnDispose_Terminate,
                OnConnect_TimeOut = this.OnConnect_TimeOut,
            };

            return ret;
        }

    }

    public class ProcessServerOptions {
        public string ListenOn_Host { get; set; }
        public string ListenOn_Port { get; set; }

        public int? ParentProcess_ID { get; set; }
        public bool ParentProcess_WaitForExit { get; set; }

        

        public virtual ProcessServerOptions Clone() {
            var ret = new ProcessServerOptions() {
                ListenOn_Host = ListenOn_Host,
                ListenOn_Port = ListenOn_Port,

                ParentProcess_ID = ParentProcess_ID,

                ParentProcess_WaitForExit = ParentProcess_WaitForExit,
            };

            return ret;
        }


        public bool Valid() {
            return true
                && !string.IsNullOrEmpty(ListenOn_Host)
                && !string.IsNullOrEmpty(ListenOn_Port)
                && ((ParentProcess_WaitForExit && ParentProcess_ID != null) || (!ParentProcess_WaitForExit))
                ;
        }

        public override string ToString() {
            
            var args = new Dictionary<String, String>() {
                {nameof(ListenOn_Host), ListenOn_Host },
                {nameof(ListenOn_Port), ListenOn_Port },
                {nameof(ParentProcess_ID), ParentProcess_ID?.ToString() },
                {nameof(ParentProcess_WaitForExit), ParentProcess_WaitForExit.ToString() },
            };

            var Elements = (
                from item in args
                where !string.IsNullOrEmpty(item.Value)
                let value = $@"--{item.Key}={item.Value}"
                select value
                ).ToList();

            var ret = string.Join(" ", Elements);

            return ret;
        }

        public static ProcessServerOptions Parse(string[] Args) {
            return Parse(Args, out var _);
        }

        public static ProcessServerOptions Parse(string[] Args, out Mono.Options.OptionSet Options) {
            var ret = new ProcessServerOptions();

            Options = new Mono.Options.OptionSet() {
                "This",
                "Is",
                "Another",
                { $@"{nameof(ParentProcess_ID)}=", "the process ID of the parent process", (int x)=> ret.ParentProcess_ID = x },
                { $@"{nameof(ParentProcess_WaitForExit)}=", "if true, will wait for the parent process to exit", (bool x)=> ret.ParentProcess_WaitForExit = x },
                { $@"{nameof(ListenOn_Host)}=", "the host to listen on", (string x)=> ret.ListenOn_Host = x },
                { $@"{nameof(ListenOn_Port)}=", "the port to listen on", (string x)=> ret.ListenOn_Port = x }
            };

            Options.Parse(Args);

            return ret;
        }

    }
        

}
