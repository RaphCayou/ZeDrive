using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;

namespace ServerTests
{
    /// <summary>
    /// Description résumée pour DataStoreTests
    /// </summary>
    [TestClass]
    public class GivenDataStore
    {
        private ServerBusiness _business;
        private const string ClientsSaveFile = "clientsSaveFile.xml";
        private const string GroupsSaveFile = "groupsSaveFile.xml";
        private readonly string _loadClients = Path.Combine("..", "..", "loadClients");
        private readonly string _loadGroups = Path.Combine("..", "..", "loadGroups");

        [TestInitialize]
        public void SetUp()
        {
            CleanUp();
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (File.Exists(GroupsSaveFile))
                File.Delete(GroupsSaveFile);
            if (File.Exists(ClientsSaveFile))
                File.Delete(ClientsSaveFile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void When_Creating_With_Empty_Parameters_Then_Exception()
        {
            _business = new ServerBusiness("", "");
        }

        [TestMethod]
        public void When_Creating_With_No_Extensions_Then_Add_Extensions()
        {
            _business = new ServerBusiness(GroupsSaveFile, ClientsSaveFile);
            Tuple<string, string> files = _business.GetSaveFilesNames();
            Assert.AreEqual(Path.GetExtension(files.Item1), ".xml");
            Assert.AreEqual(Path.GetExtension(files.Item2), ".xml");
        }

        [TestMethod]
        public void When_Creating_User_Then_User_Can_Login()
        {
            _business = new ServerBusiness(GroupsSaveFile, ClientsSaveFile);
            _business.CreateUser("Henry", "hunter2");
            Assert.IsTrue(_business.Connect("Henry", "hunter2"));
            
            _business.CreateUser("Bobby", "IOnlySee*******");
            Assert.IsTrue(_business.Connect("Bobby", "IOnlySee*******"));
        }

        [TestMethod]
        public void When_Creating_With_Existing_NonEmpty_Files_Then_Loads_Correctly()
        {
            _business = new ServerBusiness(_loadGroups, _loadClients);
            List<ShareLibrary.Models.Client> clients = _business.GetClientList();
            Assert.IsNotNull(clients);
            Assert.IsNotNull(clients.FirstOrDefault(c => c.Name == "Henry"));
        }

        [TestMethod]
        public void When_Creating_Group_Then_You_Are_Admin_And_Member()
        {
            _business = new ServerBusiness(GroupsSaveFile, ClientsSaveFile);
            _business.CreateUser("potato", "potàto");
            _business.CreateGroup("FrenchFries", "Fried batons of potato", "potato");
            Assert.AreEqual("potato", _business.GetGroupList().FirstOrDefault(g => g.Name == "FrenchFries")?.Administrator.Name);
            Assert.IsTrue(_business.GetGroupList().FirstOrDefault(g => g.Name == "FrenchFries")?.Members.Exists(m => m.Name == "potato") == true);
        }

        [TestMethod]
        public void When_Changing_Group_Admin_Then_Rights_Are_Swapped()
        {
            _business = new ServerBusiness(GroupsSaveFile, ClientsSaveFile);
            _business.CreateUser("soyElAdmin", "holaSombreros");
            _business.CreateUser("noSoyElAdmin", "laAguaAzul");
            _business.CreateGroup("LosBanditos", "¡Mira este hijo!", "soyElAdmin");
            _business.SendClientGroupInvitation("soyElAdmin", "noSoyElAdmin", "LosBanditos");
            _business.AcknowledgeInvite("noSoyElAdmin", "LosBanditos", true);
            Assert.AreEqual("soyElAdmin", _business.GetGroupList().FirstOrDefault(g => g.Name == "LosBanditos")?.Administrator.Name);
            _business.ChangeAdministratorGroup("soyElAdmin", "noSoyElAdmin", "LosBanditos");
            Assert.AreEqual("noSoyElAdmin", _business.GetGroupList().FirstOrDefault(g => g.Name == "LosBanditos")?.Administrator.Name);
        }
    }
}
