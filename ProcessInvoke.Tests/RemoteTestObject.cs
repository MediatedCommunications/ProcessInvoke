using System;
using System.Threading.Tasks;

namespace ProcessInvoke.Tests {
    public class RemoteTestObject : IRemoteTestObject {
        public Task<int> HostingProcessIdAsync() {
            var ret = Environment.ProcessId;

            return Task.FromResult(ret);
        }

        public Task<BaseResult?> GetValueAsync(ResultType Type) {

            var ret = Type switch
            {
                ResultType.DerivedResult => new DerivedResult() {
                    Text = "Derived",
                },

                ResultType.BaseResult => new BaseResult {

                },
                _ => default
            };

            if(ret is { }) {
                ret.Type = Type;
            }

            return Task.FromResult(ret);
        }

    }

}
