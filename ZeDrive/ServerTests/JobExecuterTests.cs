using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using ShareLibrary.Models;
using ShareLibrary.Summary;
using Action = ShareLibrary.Models.Action;
using FileInfo = ShareLibrary.Models.FileInfo;

namespace ServerTests
{
    [TestClass]
    public class JobExecuterTests
    {
        private ServerBusiness business;
        private List<Revision> actualRevisions;
        private List<Revision> expectedRevisions;
        public Job CreateJob(string filename, string username, string groupname)
        {
            return new Job
            {
                Username = username,
                Revisions = new List<Revision>
                {
                    new Revision
                    {
                        GroupName = groupname,
                        Action = Action.Create,
                        File = new FileInfo
                        {
                            CreationDate = File.GetCreationTime(filename),
                            LastModificationDate = File.GetLastWriteTime(filename),
                            Name = filename
                        },
                        Data = File.ReadAllBytes(filename)
                    }
                },
                GroupSummaries = new List<GroupSummary>
                {
                    new GroupSummary
                    {
                        GroupName = groupname,
                        Files = new List<FileInfo>
                        {
                            new FileInfo
                            {
                                CreationDate = File.GetCreationTime(filename),
                                LastModificationDate = File.GetLastWriteTime(filename),
                                Name = filename
                            }
                        }
                    }
                }
            };
        }
        public Job CreateEmptyJob(string username, string groupname)
        {
            return new Job
            {
                Username = username,
                Revisions = new List<Revision>(),
                GroupSummaries = new List<GroupSummary>
                {
                    new GroupSummary
                    {
                        GroupName = groupname,
                        Files = new List<FileInfo>()
                    }
                }
            };
        }

        public Revision CreateRevision(string filename, string username, string groupname)
        {
            return new Revision
            {
                GroupName = groupname,
                Action = Action.Create,
                File = new FileInfo
                {
                    CreationDate = File.GetCreationTime(filename),
                    LastModificationDate = File.GetLastWriteTime(filename),
                    Name = filename
                },
                Data = File.ReadAllBytes(filename)
            };
        }

        public void AddData(string filename, string username, string groupname)
        {
            
        }

        [TestInitialize]
        public void SetUp()
        {
            business = new ServerBusiness("groupSave","clientSave");
            try
            {
                business.CreateUser("Bob", "psw");
                business.CreateGroup("A", "Desc", "Bob");
            }
            catch {}
        }

        [TestMethod]
        // Test the first client call (with no groups) when the server is virgin
        public void TestFirstClientInitServerVirgin()
        {
            actualRevisions = business.UpdateServerHistory(new Job
            {
                Username = "Bob",
                Revisions = new List<Revision>(),
                GroupSummaries = new List<GroupSummary>()
            });
            
            CollectionAssert.AreEqual(new List<Revision>(), actualRevisions);
        }

        [TestMethod]
        // Test the first client call (with groups) when the server is virgin
        public void TestFirstClientInitGroupServerVirgin()
        {
            actualRevisions = business.UpdateServerHistory(CreateEmptyJob("Bob", "A"));

            CollectionAssert.AreEqual(new List<Revision>(), actualRevisions);
        }

        [TestMethod]
        // Test the first client call (with groups) when the server has data
        // The server return everything to the fresh client
        public void TestFirstClientInitGroupServer()
        {
            // Add data
            business.UpdateServerHistory(CreateJob("Test1.txt", "Bob", "A"));
            business.UpdateServerHistory(CreateJob("Test2.txt", "Bob", "A"));
            business.UpdateServerHistory(CreateJob("Test3.txt", "Bob", "A"));

            actualRevisions = business.UpdateServerHistory(CreateEmptyJob("Bob", "A"));

            expectedRevisions = new List<Revision>
            {
                CreateRevision("Test1.txt", "Bob", "A"),
                CreateRevision("Test2.txt", "Bob", "A"),
                CreateRevision("Test3.txt", "Bob", "A")
            };

            CollectionAssert.AreEqual(expectedRevisions, actualRevisions);
        }

        [TestMethod]
        public void TestCreateAndModifyClientFileJob()
        {
            business.UpdateServerHistory(CreateJob("Test1.txt", "Bob", "A"));

            using (StreamWriter w = File.AppendText("Test1.txt"))
            {
                w.WriteLine();
                w.WriteLine("Hello from the other side");
            }

            actualRevisions = business.UpdateServerHistory(CreateJob("Test1.txt", "Bob", "A"));

            CollectionAssert.AreEqual(new List<Revision>(), actualRevisions);
        }

        [TestMethod]
        public void TestDeleteJobForReal()
        {
            // Add the job to the server
            business.UpdateServerHistory(CreateJob("Test1.txt", "Bob", "A"));

            // Delete the job from the server (and not the client)
            Job deleteJob = CreateJob("Test1.txt", "Bob", "A");
            deleteJob.Revisions[0].Action = Action.Delete;
            deleteJob.GroupSummaries[0].Files = new List<FileInfo>();

            business.UpdateServerHistory(deleteJob);

            // Add an other job (with the last file in the summary)
            Job createJob = CreateJob("Test2.txt", "Bob", "A");
            createJob.GroupSummaries[0].Files.Add(new FileInfo
            {
                CreationDate = File.GetCreationTime("Test1.txt"),
                LastModificationDate = File.GetLastWriteTime("Test1.txt"),
                Name = "Test1.txt"
            });

            Revision rev = CreateRevision("Test1.txt", "Bob", "A");
            rev.Action = Action.Delete;
            rev.Data = new byte[0];

            expectedRevisions = new List<Revision> { rev };

            // The server tells the client to delete the file
            actualRevisions = business.UpdateServerHistory(createJob);

            CollectionAssert.AreEqual(expectedRevisions, actualRevisions);
        }

        // No Assert
        //*********************************************************************************

        [TestMethod]
        public void TestAddJob()
        {
            business.UpdateServerHistory(CreateJob("Test1.txt", "Bob", "A"));
        }

        [TestMethod]
        public void TestAddSecondJob()
        {
            business.UpdateServerHistory(CreateJob("Test1.txt", "Bob", "A"));

            Job job = CreateJob("Test2.txt", "Bob", "A");
            job.GroupSummaries[0].Files.Add(new FileInfo
            {
                CreationDate = File.GetCreationTime("Test1.txt"),
                LastModificationDate = File.GetLastWriteTime("Test1.txt"),
                Name = "Test1.txt"
            });

            business.UpdateServerHistory(job);
        }
    }
}
