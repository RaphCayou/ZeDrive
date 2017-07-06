using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareLibrary;

namespace Server
{
    class JobExecuter
    {
        private BlockingCollection<Job> syncJobs;
        private int executionCount = 0;
        private GroupSummary serverSummary;

        public JobExecuter()
        {
            syncJobs = new BlockingCollection<Job>();
        }

        public void Execute()
        {
            foreach (Job job in syncJobs.GetConsumingEnumerable())
            {
                executionCount++;
                Console.WriteLine("Execute Job {0}", executionCount);

                // TODO Execute

            }
        }

        public void Add(Job job)
        {
            syncJobs.Add(job);
        }
    }
}
