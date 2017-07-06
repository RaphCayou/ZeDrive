using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShareLibraryTests
{
    [TestClass]
    public class GroupSummaryTests
    {
        private const string TESTING_PATH = "./";

        [TestMethod]
        public void ConstructorTest()
        {
            string fileName = Path.GetRandomFileName();

            Directory.CreateDirectory(TESTING_PATH + "Group1");
        }
    }
}
