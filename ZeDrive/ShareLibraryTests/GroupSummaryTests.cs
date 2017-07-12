using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private const string GROUP2 = "GROUP2";
        private static readonly IList<string> GROUPS = new ReadOnlyCollection<string>( new List<string> { GROUP1, GROUP2 });
        private List<string> TestFilesGroup1;

        [TestInitialize()]
        public void Initialize()
        {
            foreach (string group in GROUPS)
            {
                Directory.CreateDirectory(Path.Combine(TESTING_PATH, group));
                IEnumerable<string> filePaths = Directory.EnumerateFiles(Path.Combine(TESTING_PATH, group));
                foreach (string filePath in filePaths)
                {
                    File.Delete(Path.Combine(filePath));
                }

            }
            TestFilesGroup1 = new List<string>();
            for (int i = 0; i < 5; ++i)
            {
                string file = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());
                TestFilesGroup1.Add(file);
                File.Create(file).Close();
            }
        }

        [TestCleanup()]
        public void Cleanup()
        {
            TestFilesGroup1.ForEach(File.Delete);
            Directory.Delete(Path.Combine(TESTING_PATH, GROUP1));
            Directory.Delete(Path.Combine(TESTING_PATH, GROUP2));
        }

        [TestMethod]
        public void ConstructorTest()
        {
            //TODO faire le vrai test de validation de constructeur
            List<string> files = new List<string>{ Path.Combine(TESTING_PATH, GROUP2, "file1.jp"), Path.Combine(TESTING_PATH, GROUP2, "file2.jp"), Path.Combine(TESTING_PATH, GROUP2, "file3.jp") };

            foreach (string file in files)
            {
                File.Create(file).Close();
            }
            List<DateTime> filesCreations = new List<DateTime>{ new DateTime(2021, 9, 12, 17, 12, 4), new DateTime(2000, 1, 1, 0, 0, 0), new DateTime(2017, 7, 12, 12, 15, 4) };
            List<DateTime> filesModifications = new List<DateTime> { new DateTime(2021, 9, 12, 17, 12, 4), new DateTime(2001, 1, 1, 0, 0, 0), new DateTime(2017, 7, 12, 12, 15, 5) };

            for (int i = 0; i < files.Count; i++)
            {
                File.SetCreationTime(files[i], filesCreations[i]);
            }

            GroupSummary sum1g1 = new GroupSummary(GROUP2, TESTING_PATH);
            
            //TODO Valider les informations set manuellement
            Assert.Equals(sum1g1.GroupName, GROUP2);
            //creer des fichier manuellement et set leur info et checker que les info sont valide
        }

        [TestMethod]
        public void UpdateValidation()
        {
            //tester que les fichiers sont bien ajouter
            GroupSummary sum1g1 = new GroupSummary(GROUP1, TESTING_PATH);
            string fileAdded = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());
            File.Create(fileAdded).Close();
            sum1g1.Update();

        }
        
        [TestMethod]
        public void EqualValidation()
        {
            string fileAdded = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());

            GroupSummary sum1g1 = new GroupSummary(GROUP1, TESTING_PATH);
            //When we create a file, a stream is created, so we need to close the stream because we are done with it.
            File.Create(fileAdded).Close();
            GroupSummary sum2g1 = new GroupSummary(GROUP1, TESTING_PATH);
            GroupSummary sum1g2 = new GroupSummary(GROUP2, TESTING_PATH);

            Assert.IsTrue(sum1g1 == sum1g1);
            Assert.IsFalse(sum1g1 != sum1g1);
            Assert.IsTrue(sum1g1 == sum2g1);
            Assert.IsFalse(sum1g1 != sum2g1);

            Assert.IsTrue(sum1g1 != sum1g2);
            Assert.IsFalse(sum1g1 == sum1g2);

            Assert.IsFalse(sum1g1 == null);
            Assert.IsTrue(sum1g1 != null);
            File.Delete(fileAdded);
        }

        [TestMethod]
        public void GenerateRevision()
        {
            //TODO Unit test pour le generate revision

            string fileAdded = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());

            GroupSummary oldSummary = new GroupSummary(GROUP1, TESTING_PATH);
            //When we create a file, a stream is created, so we need to close the stream because we are done with it.
            File.Create(fileAdded).Close();
            GroupSummary newSummary = new GroupSummary(GROUP1, TESTING_PATH);
            newSummary.GenerateRevisions(oldSummary);
            File.Delete(fileAdded);
        }

        //TODO test le contenu des revision(genre le data des fichiers)
    }
}
