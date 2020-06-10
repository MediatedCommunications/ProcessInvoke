using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessInvoke {
    public interface IProcessHost : IDisposable {
        void Stop();

        string Echo(string Message);

        I Register<I, T>();

    }

}
