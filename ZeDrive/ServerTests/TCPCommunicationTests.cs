using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Server.TcpCommunication;
using Client;
using Moq;
using System.Threading;
using ShareLibrary.Communication;

namespace ServerTests
{
    [TestClass]
    public class TCPCommunicationTests
    {
        [TestMethod]
        public void TestSendingMessageFromClientToServer()
        {
            IServerBusiness business = new ServerBusiness("groupsSaveFile", "clientsSaveFile");
            ServerDispatcher server = new ServerDispatcher(business, "127.0.0.1", 10000);

            ClientServerAccess clientLinkToServer = new ClientServerAccess("127.0.0.1", 10000);

            try
            {
                clientLinkToServer.CreateUser("Henry", "hunter2");
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
            }
            Assert.IsTrue(clientLinkToServer.Connect("Henry", "hunter2"));
        }
    }
}
