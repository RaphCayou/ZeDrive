using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using Server.TcpCommunication;
using Client;
using Moq;
using System.Threading;

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

            clientLinkToServer.GetNotification();

            Thread.Sleep(1000);

            mockBusiness.Verify(b => b.GetNotification());
        }
    }
}
