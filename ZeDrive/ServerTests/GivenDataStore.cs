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

        [TestInitialize]
        public void SetUp()
        {
            
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
            business = new ServerBusiness("groupsSaveFile", "clientsSaveFile");
            Tuple<string, string> files = business.GetSaveFilesNames();
            Assert.AreEqual(Path.GetExtension(files.Item1), ".xml");
            Assert.AreEqual(Path.GetExtension(files.Item2), ".xml");
        }

        [TestMethod]
        public void When_Creating_User_Then_User_Can_Login()
        {
            business = new ServerBusiness("groupsSaveFile", "clientsSaveFile");
            business.CreateUser("Henry", "hunter2");
            Assert.IsTrue(business.Connect("Henry", "hunter2"));

            business = new ServerBusiness("groupsSaveFile", "clientsSaveFile");
            business.CreateUser("Bobby", "IOnlySee*******");
            Assert.IsTrue(business.Connect("Bobby", "IOnlySee*******"));
        }

        [TestMethod]
        public void When_Creating_With_Existing_NonEmpty_Files_Then_Loads_Correctly()
        {
            business = new ServerBusiness("groupsSaveFile", "clientsSaveFile");
            List<ShareLibrary.Models.Client> clients = business.GetClientLists();
            Assert.IsNotNull(clients);
            Assert.IsNotNull(clients.FirstOrDefault(c => c.Name == "Henry"));
        }
    }
}
