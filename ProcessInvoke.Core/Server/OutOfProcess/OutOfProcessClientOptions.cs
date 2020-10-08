using System;
using System.Text;

namespace ProcessInvoke.Server.OutOfProcess {

    public class ClientOptions {
        public TimeSpan OnConnect_Attempts_TotalTimeOut { get; set; }
    }

    public class OutOfProcessClientOptions : ClientOptions {
        public bool OnDispose_Stop { get; set; }
        public bool OnDispose_Kill { get; set; }

        public virtual OutOfProcessClientOptions Clone() {
            var ret = new OutOfProcessClientOptions() {
                OnDispose_Stop = this.OnDispose_Stop,
                OnDispose_Kill = this.OnDispose_Kill,
                OnConnect_Attempts_TotalTimeOut = this.OnConnect_Attempts_TotalTimeOut,
            };

            return ret;
        }

    }


}
