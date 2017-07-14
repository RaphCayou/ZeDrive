using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.TcpCommunication
{
    [Serializable]
    public class RequestMessageContent
    {
        public CommandHeader Command { get; set; }
        public List<object> Parameters { get; set; }
    }
}
