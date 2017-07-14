using System;
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
        [TestMethod]
        public void TestUpdateLocalFiles()
        {
            DateTime now = DateTime.Now;
            string fileAdded = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());
            Revision revisionCreate = new Revision()
            {
                File = new ShareLibrary.Models.FileInfo { Name = fileAdded, LastModificationDate = now, CreationDate = now },
                GroupName = GROUP1,
                Action = Action.Create,
                Data = Encoding.UTF8.GetBytes(fileContent)
            };
            string fileDeleted = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());
            string fileModifed = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());
            File.Create(fileAdded).Close();
            ClientBusiness client = new ClientBusiness(TESTING_PATH);

        }
    }
}
