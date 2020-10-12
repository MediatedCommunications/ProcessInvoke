using System.Threading.Tasks;

namespace ProcessInvoke.Tests {
    public interface IRemoteTestObject {
        Task<int> HostingProcessIdAsync();
        Task<BaseResult?> GetValueAsync(ResultType Type);
        Task<bool> TestReferenceAsync();
    }

    public enum ResultType {
        None,
        BaseResult,
        DerivedResult,
    }

    public static class ResultTypeExtensions {

        public static ResultType GetTypeProperty(this BaseResult? This) {
            var ret = This switch
            {
                BaseResult V1 => V1.Type,
                _ => ResultType.None
            };

            return ret;
        }

        public static ResultType GetResultType(this BaseResult? This) {
            var ret = This switch
            {
                DerivedResult => ResultType.DerivedResult,
                BaseResult => ResultType.BaseResult,
                _ => ResultType.None
            };

            return ret;
        }
    }

    public class BaseResult {
        public ResultType Type { get; set; }
    }

    public class DerivedResult : BaseResult {
        public string Text { get; set; } = string.Empty;
    }

}
