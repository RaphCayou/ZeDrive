using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLibrary;
using ShareLibrary.Models;

namespace Server
{
    public class Job
    {
        public SyncTransmission Parameters;
        public Action<SyncTransmission> CallBack;
    }
}
