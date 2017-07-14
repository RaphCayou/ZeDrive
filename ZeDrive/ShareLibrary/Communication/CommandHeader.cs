using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.TcpCommunication
{
    [Serializable]
    public class CommandHeader
    {
        public String MethodName { get; set; }
        public List<String> ParameterAssemblyQualifiedNames { get; set; }
        public List<Type> ParameterTypes
        {
            get
            {
                return ParameterAssemblyQualifiedNames.Select(t => Type.GetType(t)).ToList();
            }
            set
            {
                ParameterAssemblyQualifiedNames = value.Select(t => t.AssemblyQualifiedName).ToList();
            }
        }
    }
}
