using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareLibrary.Models;
using Action = System.Action;

namespace ShareLibraryTests
{
    [TestClass]
    public class RevisionTests
    {
        private const string TESTING_PATH = "./";
        private const string GROUP1 = "GROUP1";

        private const string FILE_CONTENT =
            "Le TP 2/3 consiste à mettre en place un système de partage de fichier inspiré de systèmes comme\r\n" +
            "Dropbox, GoogleDrive, etc. Le système permettra à différents clients de partager et de\r\n" +
            "synchroniser des fichiers au sein de groupes définis. Le système est constitué de deux\r\n" +
            "composantes : un serveur central et un client. Le serveur est accessible à une adresse fixe,\r\n" +
            "connue de tous les clients. Le serveur a pour rôle de synchroniser, recevoir et transmettre les\r\n" +
            "fichiers des clients, et gérer les groupes auxquels les clients peuvent se rattacher. Les clients\r\n" +
            "doivent se synchroniser avec le serveur. Le système à implémenter est donc un système de clientserveur\r\n" +
            "basé sur le protocole TCP.";

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
        public void TestApplyCreate()
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
                Action = ShareLibrary.Models.Action.Create,
                Data = Encoding.UTF8.GetBytes(FILE_CONTENT)
            };
            revisionCreate.Apply(TESTING_PATH);

            Assert.IsTrue(File.Exists(fileAdded));
            Assert.AreEqual(now, File.GetLastWriteTime(fileAdded));
            Assert.AreEqual(now, File.GetCreationTime(fileAdded));
            byte[] fileBytes = File.ReadAllBytes(fileAdded);
            byte[] textBytes = Encoding.UTF8.GetBytes(FILE_CONTENT);
            Assert.AreEqual(textBytes.Length, fileBytes.Length);
            for (int i = 0; i < textBytes.Length; i++)
            {
                Assert.AreEqual(textBytes[i], fileBytes[i]);
            }
            File.Delete(fileAdded);
        }

        [TestMethod]
        public void TestApplyModify()
        {
            DateTime now = DateTime.Now;
            string fileModifed = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());
            Revision revisionModification = new Revision()
            {
                File = new ShareLibrary.Models.FileInfo
                {
                    Name = Path.GetFileName(fileModifed),
                    LastModificationDate = now,
                    CreationDate = now
                },
                GroupName = GROUP1,
                Action = ShareLibrary.Models.Action.Create,
                Data = Encoding.UTF8.GetBytes(FILE_CONTENT)
            };
            revisionModification.Apply(TESTING_PATH);

            Assert.IsTrue(File.Exists(fileModifed));
            Assert.AreEqual(now, File.GetLastWriteTime(fileModifed));
            Assert.AreEqual(now, File.GetCreationTime(fileModifed));
            byte[] fileBytes = File.ReadAllBytes(fileModifed);
            byte[] textBytes = Encoding.UTF8.GetBytes(FILE_CONTENT);
            Assert.AreEqual(textBytes.Length, fileBytes.Length);
            for (int i = 0; i < textBytes.Length; i++)
            {
                Assert.AreEqual(textBytes[i], fileBytes[i]);
            }
            File.Delete(fileModifed);
        }

        [TestMethod]
        public void TestApplyDelete()
        {
            string fileDeleted = Path.Combine(TESTING_PATH, GROUP1, Path.GetRandomFileName());
            //TODO faire ce test
        }
    }
}