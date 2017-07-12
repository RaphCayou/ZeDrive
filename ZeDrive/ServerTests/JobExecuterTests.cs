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
        private Job job1;

        [TestInitialize]
        public void SetUp()
        {
            business = new ServerBusiness("groupSave","clientSave");
            business.CreateUser("Bob", "psw");
            business.CreateGroup("A", "Desc", "Bob");

            job1 = new Job()
            {
                Parameters = new SyncTransmission()
                {
                    Username = "Bob",
                    Revisions = new List<Revision>()
                    {
                        new Revision()
                        {
                            GroupName = "A",
                            Action = Action.Create,
                            File = new FileInfo()
                            {
                                Name = "Test1.txt"
                            },
                            Data = File.ReadAllBytes("Test1.txt")
                        }
                    },
                    GroupSummaries = new List<GroupSummary>()
                    {
                        new GroupSummary()
                        {
                            GroupName = "A",
                            Files = new List<FileInfo>()
                            {
                                new FileInfo()
                                {
                                    Name = "Test1.txt"
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
            business.UpdateServerHistory(job1);
        }
    }
}
