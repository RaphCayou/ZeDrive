using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
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
        }
    }
}
