using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessInvoke.Hosting.Process {
    public class ProcessHostOptions {
        public string ListenOn_Provider { get; set; } = string.Empty;
        public string ListenOn_Host { get; set; } = string.Empty;
        public string ListenOn_Port { get; set; } = string.Empty;
        public string ListenOn_Key { get; set; } = string.Empty;

        public int? ParentProcess_ID { get; set; }
        public bool Terminate_OnParentProcessExit { get; set; }
        public bool Terminate_OnStop { get; set; }

        public Endpoint ToEndpoint() {
            return new Endpoint(
                ListenOn_Provider,
                ListenOn_Host,
                ListenOn_Port,
                ListenOn_Key
            );
        }

        public virtual ProcessHostOptions Clone() {
            var ret = new ProcessHostOptions() {
                ListenOn_Provider = ListenOn_Provider,
                ListenOn_Host = ListenOn_Host,
                ListenOn_Port = ListenOn_Port,
                ListenOn_Key = ListenOn_Key,

                ParentProcess_ID = ParentProcess_ID,

                Terminate_OnParentProcessExit = Terminate_OnParentProcessExit,
                Terminate_OnStop = Terminate_OnStop,
            };

            return ret;
        }


        public bool Valid() {
            return true
                && !string.IsNullOrEmpty(ListenOn_Provider)
                && !string.IsNullOrEmpty(ListenOn_Host)
                && !string.IsNullOrEmpty(ListenOn_Port)
                && !string.IsNullOrEmpty(ListenOn_Key)
                && ((Terminate_OnParentProcessExit && ParentProcess_ID != null) || (!Terminate_OnParentProcessExit))
                ;
        }

        public override string ToString() {

            var args = new SortedDictionary<String, String>() {
                {nameof(ListenOn_Provider), ListenOn_Provider },
                {nameof(ListenOn_Host), ListenOn_Host },
                {nameof(ListenOn_Port), ListenOn_Port },
                {nameof(ListenOn_Key), ListenOn_Key },
                {nameof(ParentProcess_ID), $@"{ParentProcess_ID}"},
                {nameof(Terminate_OnParentProcessExit), Terminate_OnParentProcessExit.ToString() },
                {nameof(Terminate_OnStop), Terminate_OnStop.ToString() },
            };

            var Elements = (
                from item in args
                where !string.IsNullOrEmpty(item.Value)
                let value = $@"--{item.Key}=""{item.Value}"""
                select value
                ).ToList();

            var ret = string.Join(" ", Elements);

            return ret;
        }

        public static ProcessHostOptions Parse(string[] Args) {
            return Parse(Args, out var _);
        }

        public static ProcessHostOptions Parse(string[] Args, out Mono.Options.OptionSet Options) {
            var ret = new ProcessHostOptions();

            Options = new Mono.Options.OptionSet() {
                "This",
                "Is",
                "Another",
                { $@"{nameof(ParentProcess_ID)}=", "the process ID of the parent process", (int x)=> ret.ParentProcess_ID = x },
                { $@"{nameof(Terminate_OnParentProcessExit)}=", "if true, will exit when the parent process exits", (bool x)=> ret.Terminate_OnParentProcessExit = x },
                { $@"{nameof(Terminate_OnStop)}=", "if true, will exit when 'Stop' is invoked", (bool x)=> ret.Terminate_OnParentProcessExit = x },
                { $@"{nameof(ListenOn_Provider)}=", "the provider to use for listening", (string x)=> ret.ListenOn_Provider = x },
                { $@"{nameof(ListenOn_Host)}=", "the host to listen on", (string x)=> ret.ListenOn_Host = x },
                { $@"{nameof(ListenOn_Port)}=", "the port to listen on", (string x)=> ret.ListenOn_Port = x },
                { $@"{nameof(ListenOn_Key)}=", "the key to listen on", (string x)=> ret.ListenOn_Key = x }
            };

            Options.Parse(Args);

            return ret;
        }

    }
}
