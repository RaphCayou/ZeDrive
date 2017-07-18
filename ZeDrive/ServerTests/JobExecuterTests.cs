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

        public Job CreateJob(string filename, string username, string groupname)
        {
            return new Job
            {
                Parameters = new SyncTransmission
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
                }
            };
        }

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
            job.Parameters.GroupSummaries[0].Files.Add(new FileInfo
            {
                CreationDate = File.GetCreationTime("Test1.txt"),
                LastModificationDate = File.GetLastWriteTime("Test1.txt"),
                Name = "Test1.txt"
            });

            business.UpdateServerHistory(job);
        }
    }
}
