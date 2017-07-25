using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLibrary.Utils
{
    public class TraceLog
    {
        private static string TraceFileName = "traceFile.txt";
        public static void Trace(string method, string s)
        {
            File.AppendAllText(TraceFileName, Environment.NewLine);
            File.AppendAllText(TraceFileName, method);
            File.AppendAllText(TraceFileName, Environment.NewLine);
            File.AppendAllText(TraceFileName, s);
        }
    }
}
