using System.Reflection.Emit;

namespace ProcessInvoke {
    public class Endpoint {
        public string Provider { get; private set; } = string.Empty;
        public string Host { get; private set; } = string.Empty;
        public string Port { get; private set; } = string.Empty;
        public string Key { get; private set; } = string.Empty;

        public Endpoint(string Provider, string Host, string Port, string Key) {
            this.Provider = Provider;
            this.Host = Host;
            this.Port = Port;
            this.Key = Key;
        }

    }


}
