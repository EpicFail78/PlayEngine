using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using librpc;

namespace PS4_Cheater
{
    public interface PS4APIWrapper
    {
        void Connect();
        ProcessInfo GetProcessInfo(Int32 pid);
        ProcessList GetProcessList();
    }
}
