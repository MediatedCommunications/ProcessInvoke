using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessInvoke {
    public class ProcessServerOptions {
        public string ListenOn_Host { get; set; }
        public string ListenOn_Port { get; set; }

        public int? ParentProcess_ID { get; set; }
        public bool Terminate_OnParentProcessExit { get; set; }
        public bool Terminate_OnStop { get; set; }

        public virtual ProcessServerOptions Clone() {
            var ret = new ProcessServerOptions() {
                ListenOn_Host = ListenOn_Host,
                ListenOn_Port = ListenOn_Port,

                ParentProcess_ID = ParentProcess_ID,

                Terminate_OnParentProcessExit = Terminate_OnParentProcessExit,
                Terminate_OnStop = Terminate_OnStop,
            };

            return ret;
        }


        public bool Valid() {
            return true
                && !string.IsNullOrEmpty(ListenOn_Host)
                && !string.IsNullOrEmpty(ListenOn_Port)
                && ((Terminate_OnParentProcessExit && ParentProcess_ID != null) || (!Terminate_OnParentProcessExit))
                ;
        }

        public override string ToString() {
            
            var args = new SortedDictionary<String, String>() {
                {nameof(ListenOn_Host), ListenOn_Host },
                {nameof(ListenOn_Port), ListenOn_Port },
                {nameof(ParentProcess_ID), ParentProcess_ID?.ToString() },
                {nameof(Terminate_OnParentProcessExit), Terminate_OnParentProcessExit.ToString() },
                {nameof(Terminate_OnStop), Terminate_OnStop.ToString() },
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
                { $@"{nameof(Terminate_OnParentProcessExit)}=", "if true, will exit when the parent process exits", (bool x)=> ret.Terminate_OnParentProcessExit = x },
                { $@"{nameof(Terminate_OnStop)}=", "if true, will exit when 'Stop' is invoked", (bool x)=> ret.Terminate_OnParentProcessExit = x },
                { $@"{nameof(ListenOn_Host)}=", "the host to listen on", (string x)=> ret.ListenOn_Host = x },
                { $@"{nameof(ListenOn_Port)}=", "the port to listen on", (string x)=> ret.ListenOn_Port = x }
            };

            Options.Parse(Args);

            return ret;
        }

    }
        

}
