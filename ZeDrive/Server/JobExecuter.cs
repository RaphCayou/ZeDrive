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
                Console.WriteLine("Execute Job {0}", ++executionCount);

                List<Revision> returnRevisions = new List<Revision>();

                // For each client group revision
                foreach (GroupSummary clientGroupSummary in job.GroupSummaries)
                {
                    // Check user authorizations
                    if (dataStore.CheckUserInGroup(job.Username, clientGroupSummary.GroupName))
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
                                ShareLibrary.Models.FileInfo currentServerFile = serverGroupSummary.Files.Find(f => f.Name == rev.File.Name);
                                ShareLibrary.Models.FileInfo currentClientFile = clientGroupSummary.Files.Find(f => f.Name == rev.File.Name);

                                // TODO check this shit :poop:
                                Revision currentRelatedjobRevision = job.Revisions.Find(r => r.File?.Name == currentClientFile?.Name);
                                switch (rev.Action)
                                {
                                    case Action.Create:
                                        // Rebind the currentRelatedjobRevision from the server instead of the client
                                        currentRelatedjobRevision = job.Revisions.Find(r => r.File?.Name == currentServerFile?.Name);

                                        // Case 1 : The file was deleted by the current client
                                        // Update server history (delete the current file)
                                        // TODO test this ...
                                        if (currentRelatedjobRevision?.File.Name == rev.File.Name && currentRelatedjobRevision?.Action == Action.Delete)
                                        {
                                            currentRelatedjobRevision.Apply(rootPath);
                                            serverGroupSummary.Update();
                                        }
                                        // Case 2 : There is a new file on the server and the current client doesn't have it. 
                                        // Add revision to response with data (Create)
                                        else
                                        {
                                            returnRevisions.Add(rev);
                                        }
                                        break;

                                    case Action.Modify:
                                        rev.Data = currentRelatedjobRevision.Data;

                                        // Case 1 : Server modification date > client modification date
                                        // Add revision to response with data (Modify)
                                        if (currentServerFile.LastModificationDate > currentClientFile.LastModificationDate)
                                        {
                                            returnRevisions.Add(rev);
                                        }
                                        // Case 2 : Server modification date < client modification date
                                        // Update server history (replace the current file)
                                        else
                                        {
                                            rev.Apply(rootPath);
                                            serverGroupSummary.Update();
                                        }
                                        break;

                                    case Action.Delete:

                                        // Case 1 : The file was created by the current client.
                                        // Update server history (add the current file)
                                        if (currentRelatedjobRevision?.Action == Action.Create)
                                        {
                                            currentRelatedjobRevision.Apply(rootPath);
                                            serverGroupSummary.Update();
                                        }
                                        // Case 2 : The file is deleted for real (deleted from an other client)
                                        // Add revision to response (Delete)
                                        else
                                        {
                                            rev.Data = new byte[0];
                                            returnRevisions.Add(rev);
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            // Initial case when the server does not have the client group synced
                            // Add the client files (revisions) to the server files
                            
                            List<Revision> clientRevisions =
                                job.Revisions.FindAll(r => r.GroupName == clientGroupSummary.GroupName);

                            // Create the group directory
                            Directory.CreateDirectory(Path.Combine(rootPath, clientGroupSummary.GroupName));

                            // Add every client file to the server
                            foreach (Revision clientRev in clientRevisions)
                            {
                                clientRev.Apply(rootPath);
                            }

                            // Create the server group summary because he didn't exist
                            serverGroupSummaries.Add(new GroupSummary(clientGroupSummary.GroupName, rootPath));
                        }
                    }
                }
                // Return the truth
                job.CallBack(returnRevisions);
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
