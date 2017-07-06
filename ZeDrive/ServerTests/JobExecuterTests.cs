using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;

namespace ServerTests
{
    [TestClass]
    public class JobExecuterTests
    {
        private ServerBusiness business;

        [TestInitialize]
        public void SetUp()
        {
            business = new ServerBusiness();
        }

        [TestMethod]
        public void TestAddJob()
        {
            business.UpdateServerHistory(new Job());
            business.UpdateServerHistory(new Job());
        }
    }
}
