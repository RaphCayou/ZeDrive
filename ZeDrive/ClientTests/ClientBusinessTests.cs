using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareLibrary.Models;
using Action = ShareLibrary.Models.Action;

namespace ClientTests
{
    [TestClass]
    public class ClientBusinessTests
    {
        private const string TESTING_PATH = "./";
        private const string GROUP1 = "GROUP1";

        [TestInitialize()]
        public void Initialize()
        {
            Directory.CreateDirectory(Path.Combine(TESTING_PATH, GROUP1));
        }

        [TestCleanup()]
        public void Cleanup()
        {
            IEnumerable<string> filePaths = Directory.EnumerateFiles(Path.Combine(TESTING_PATH, GROUP1));
            foreach (string filePath in filePaths)
            {
                File.Delete(filePath);
            }
            Directory.Delete(Path.Combine(TESTING_PATH, GROUP1));
        }

        [TestMethod]
        public void TestUpdateLocalFiles()
        {
            DateTime now = DateTime.Now;
            string fileAdded = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());
            Revision revisionCreate = new Revision()
            {
                File = new ShareLibrary.Models.FileInfo
                {
                    Name = Path.GetFileName(fileAdded),
                    LastModificationDate = now,
                    CreationDate = now
                },
                GroupName = GROUP1,
                Action = Action.Create,
                Data = new byte[1]
            };
            string fileDeleted = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());
            Revision revisionDelete = new Revision()
            {
                File = new ShareLibrary.Models.FileInfo
                {
                    Name = Path.GetFileName(fileDeleted),
                    LastModificationDate = now,
                    CreationDate = now
                },
                GroupName = GROUP1,
                Action = ShareLibrary.Models.Action.Delete
            };
            string fileModifed = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());
            Revision revisionModfied = new Revision()
            {
                File = new ShareLibrary.Models.FileInfo
                {
                    Name = Path.GetFileName(fileModifed),
                    LastModificationDate = now,
                    CreationDate = now
                },
                GroupName = GROUP1,
                Action = Action.Modify,
                Data = new byte[1]
            };
            File.Create(fileModifed).Close();
            ClientBusiness client = new ClientBusiness(TESTING_PATH);
            client.UpdateLocalFiles(new List<Revision>() { revisionDelete, revisionCreate, revisionModfied });

            Assert.IsFalse(File.Exists(fileDeleted));
            Assert.AreEqual(now, File.GetLastWriteTime(fileModifed));
            Assert.AreEqual(now, File.GetCreationTime(fileAdded));
            File.Delete(fileModifed);
            File.Delete(fileAdded);
        }
    }
}
