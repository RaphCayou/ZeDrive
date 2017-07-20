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


        //*********************************************************************************

        [TestMethod]
        public void TestAddJob()
        {
            business.UpdateServerHistory(CreateJob("Test1.txt", "Bob", "A"));
        }

        [TestMethod]
        public void TestModifyJob()
        {
            business.UpdateServerHistory(CreateJob("Test1.txt", "Bob", "A"));

            using (StreamWriter w = File.AppendText("Test1.txt"))
            {
                w.WriteLine();
                w.WriteLine("Hello from the other side");
            }

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

        [TestMethod]
        public void TestNewEmptyClientJob()
        {
            business.UpdateServerHistory(CreateJob("Test1.txt", "Bob", "A"));
            business.UpdateServerHistory(CreateEmptyJob("Bob", "A"));
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

            // Add an other job (with the last file)
            Job createJob = CreateJob("Test2.txt", "Bob", "A");
            createJob.GroupSummaries[0].Files.Add(new FileInfo
            {
                CreationDate = File.GetCreationTime("Test1.txt"),
                LastModificationDate = File.GetLastWriteTime("Test1.txt"),
                Name = "Test1.txt"
            });

            business.UpdateServerHistory(createJob);
        }
    }
}
