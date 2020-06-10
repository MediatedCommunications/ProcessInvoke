using System;
using System.Text;

namespace ProcessInvoke {

    public class ProcessClientOptions {
        public bool OnDispose_Stop { get; set; }
        public bool OnDispose_Kill { get; set; }

        public TimeSpan OnConnect_Attempts_TotalTimeOut { get; set; }
        public long OnConnect_Attempts_Minimum { get; set; }

        public virtual ProcessClientOptions Clone() {
            var ret = new ProcessClientOptions() {
                OnDispose_Stop = this.OnDispose_Stop,
                OnDispose_Kill = this.OnDispose_Kill,
                OnConnect_Attempts_TotalTimeOut = this.OnConnect_Attempts_TotalTimeOut,
                OnConnect_Attempts_Minimum = this.OnConnect_Attempts_Minimum
            };

            return ret;
        }

    }
        

}
