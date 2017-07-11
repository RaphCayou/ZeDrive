using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareLibrary;
using ShareLibrary.Models;
using ShareLibrary.Summary;

namespace Server
{
    class JobExecuter
    {
        private BlockingCollection<Job> syncJobs;
        private int executionCount = 0;
        private List<GroupSummary> serverGroupSummaries;
        private DataStore dataStore;

        public JobExecuter(DataStore dataStore)
        {
            this.dataStore = dataStore;
            syncJobs = new BlockingCollection<Job>();
            serverGroupSummaries = new List<GroupSummary>();
        }

        public void Execute()
        {
            // Blocking call
            foreach (Job job in syncJobs.GetConsumingEnumerable())
            {
                executionCount++;
                Console.WriteLine("Execute Job {0}", executionCount);

                // TODO Execute

                // For each client group revision
                foreach (GroupSummary clientGroupSummary in job.Parameters.GroupSummaries)
                {
                    // Check user authorizations
                    if (dataStore.CheckUserInGroup(job.Parameters.Username, clientGroupSummary.GroupName))
                    {
                        GroupSummary serverGroupSummary = serverGroupSummaries.Find(s => s.GroupName == clientGroupSummary.GroupName);

                        if (serverGroupSummary != null)
                        {
                            // Build revision list with client
                            List<Revision> revisions = serverGroupSummary.GenerateRevisions(clientGroupSummary);
                        }
                        else
                        {
                            // Add the client group summary to the server group summary
                        }
                    }
                }
            }
        }

        public void Add(Job job)
        {
            syncJobs.Add(job);
        }

        public void CompleteAdding()
        {
            syncJobs.CompleteAdding();
        }
    }
}
