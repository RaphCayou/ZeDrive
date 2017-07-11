using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareLibrary.Summary;

namespace ShareLibraryTests
{
    [TestClass]
    public class GroupSummaryTests
    {
        private const string TESTING_PATH = "./";
        private const string GROUP1 = "GROUP1";

        [TestInitialize()]
        public void Initialize() { }

        [TestCleanup()]
        public void Cleanup() { }

        [TestMethod]
        public void ConstructorTest()
        {
            string fileAdded = TESTING_PATH + GROUP1 + "/" + Path.GetRandomFileName();

            Directory.CreateDirectory(TESTING_PATH + GROUP1);

            GroupSummary oldSummary = new GroupSummary(GROUP1, TESTING_PATH);
            //When we create a file, a stream is created, so we need to close the stream because we are done with it.
            File.Create(fileAdded).Close();
            GroupSummary newSummary = new GroupSummary(GROUP1, TESTING_PATH);
            newSummary.GenerateRevisions(oldSummary);
            File.Delete(fileAdded);
        }

        //TODO Unit test pour le equals
        //TODO Unit test pour le generate revision
        //TODO unit test pour le add slash
    }
}
