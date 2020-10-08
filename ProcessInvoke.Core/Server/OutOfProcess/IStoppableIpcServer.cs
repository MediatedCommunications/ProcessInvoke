using System.Threading.Tasks;

namespace ProcessInvoke.Server.OutOfProcess {
    public interface IStoppableIpcServer {
        Task StopAsync();
    }

}
