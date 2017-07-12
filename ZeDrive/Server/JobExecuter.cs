using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareLibrary;
using ShareLibrary.Models;
using ShareLibrary.Summary;
using Action = ShareLibrary.Models.Action;

namespace Server
{
    class JobExecuter
    {
        private BlockingCollection<Job> syncJobs;
        private int executionCount = 0;
        private List<GroupSummary> serverGroupSummaries;
        private DataStore dataStore;
        private string rootPath = "Server Root";

        public JobExecuter(DataStore dataStore)
        {
            this.dataStore = dataStore;
            syncJobs = new BlockingCollection<Job>();
            serverGroupSummaries = new List<GroupSummary>();

            if (Directory.Exists(rootPath))
                Directory.Delete(rootPath, true);
            Directory.CreateDirectory(rootPath);
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
                        // Get the server GroupSummary equal to client GroupSummary
                        GroupSummary serverGroupSummary =
                            serverGroupSummaries.Find(s => s.GroupName == clientGroupSummary.GroupName);

                        if (serverGroupSummary != null)
                        {
                            // Build revision list with client
                            List<Revision> revisions = serverGroupSummary.GenerateRevisions(clientGroupSummary);

                            foreach (Revision rev in revisions)
                            {
                                switch (rev.Action)
                                {
                                    case Action.Create:
                                        break;
                                    case Action.Modify:
                                        break;
                                    case Action.Delete:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            // Initial case when the server does not have the client group synced
                            // Add the client client files (revisions) to the server files

                            // TODO Possibly filter for CREATE action too
                            List<Revision> clientRevisions =
                                job.Parameters.Revisions.FindAll(r => r.GroupName == clientGroupSummary.GroupName);

                            Directory.CreateDirectory(Path.Combine(rootPath, clientGroupSummary.GroupName));

                            // Add every client file to the server
                            foreach (Revision clientRev in clientRevisions)
                            {
                                string filePath = Path.Combine(rootPath, clientGroupSummary.GroupName, clientRev.File.Name);
                                File.WriteAllBytes(filePath, clientRev.Data);
                            }

                            // Create the server group summary because he didn't exist
                            serverGroupSummaries.Add(new GroupSummary(clientGroupSummary.GroupName, rootPath));
                        }
                    }
                    else
                    {
                        throw new Exception("User not allowed in group");
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
