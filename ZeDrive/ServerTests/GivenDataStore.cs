using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using ShareLibrary.Models;

namespace ServerTests
{
    /// <summary>
    /// Description résumée pour DataStoreTests
    /// </summary>
    [TestClass]
    public class GivenDataStore
    {
        private ServerBusiness business;
        private string clientsSaveFile = "clientsSaveFile";
        private string groupsSaveFile = "groupsSaveFile";
        private string loadClients = Path.Combine("..", "..", "loadClients");
        private string loadGroups = Path.Combine("..", "..", "loadGroups");

        [TestInitialize]
        public void SetUp()
        {
            
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (File.Exists(groupsSaveFile))
                File.Delete(groupsSaveFile);
            if (File.Exists(clientsSaveFile))
                File.Delete(clientsSaveFile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void When_Creating_With_Empty_Parameters_Then_Exception()
        {
            business = new ServerBusiness("", "");
        }

        [TestMethod]
        public void When_Creating_With_No_Extensions_Then_Add_Extensions()
        {
            business = new ServerBusiness(groupsSaveFile, clientsSaveFile);
            Tuple<string, string> files = business.GetSaveFilesNames();
            Assert.AreEqual(Path.GetExtension(files.Item1), ".xml");
            Assert.AreEqual(Path.GetExtension(files.Item2), ".xml");
        }

        [TestMethod]
        public void When_Creating_User_Then_User_Can_Login()
        {
            business = new ServerBusiness(groupsSaveFile, clientsSaveFile);
            business.CreateUser("Henry", "hunter2");
            Assert.IsTrue(business.Connect("Henry", "hunter2"));
            
            business.CreateUser("Bobby", "IOnlySee*******");
            Assert.IsTrue(business.Connect("Bobby", "IOnlySee*******"));
        }

        [TestMethod]
        public void When_Creating_With_Existing_NonEmpty_Files_Then_Loads_Correctly()
        {
            business = new ServerBusiness(loadGroups, loadClients);
            List<ShareLibrary.Models.Client> clients = business.GetClientList();
            Assert.IsNotNull(clients);
            Assert.IsNotNull(clients.FirstOrDefault(c => c.Name == "Henry"));
        }

        [TestMethod]
        public void When_Creating_Group_Then_You_Are_Admin_And_Member()
        {
            business = new ServerBusiness(groupsSaveFile, clientsSaveFile);
            business.CreateUser("potato", "potàto");
            business.CreateGroup("FrenchFries", "Fried batons of potato", "potato");
            Assert.AreEqual("potato", business.GetGroupList().FirstOrDefault(g => g.Name == "FrenchFries")?.Administrator.Name);
            Assert.IsTrue(business.GetGroupList().FirstOrDefault(g => g.Name == "FrenchFries")?.Members.Exists(m => m.Name == "potato") == true);
        }
    }
}
