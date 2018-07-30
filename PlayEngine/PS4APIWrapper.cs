using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using librpc;

namespace PlayEngine
{
    public interface PS4APIWrapper
    {
        void Connect();
        librpc.ProcessInfo GetProcessInfo(Int32 pid);
        librpc.ProcessList GetProcessList();
    }
}
