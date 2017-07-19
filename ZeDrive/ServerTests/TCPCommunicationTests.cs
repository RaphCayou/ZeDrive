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
            Mock<IServerBusiness> mockBusiness = new Mock<IServerBusiness>();
            ServerDispatcher server = new ServerDispatcher(mockBusiness.Object, "127.0.0.1", 10000);

            ClientServerAccess clientLinkToServer = new ClientServerAccess("127.0.0.1", 10000);

            //TODO change user and get return value
            clientLinkToServer.GetNotification("random ass user todo change");

            Thread.Sleep(1000);

            mockBusiness.Verify(b => b.GetNotification("randomAssUsername Change it"));
        }
    }
}
