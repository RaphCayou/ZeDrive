using ShareLibrary.Communication;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.TcpCommunication
{
    public class ServerDispatcher
    {
        private IServerBusiness business;
        private Socket socketListener;

        public ServerDispatcher(IServerBusiness business, string ipAddress, int port)
        {
            this.business = business;

            // Can throw if not valid IpAddress
            IPAddress serverIP = IPAddress.Parse(ipAddress);

            // Can throw if invalid port 
            EndPoint ep = new IPEndPoint(serverIP, port);

            // Creates the listener socket
            socketListener = new Socket(serverIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socketListener.Bind(ep);

            socketListener.Listen(100);

            socketListener.BeginAccept(AcceptNewConnection, socketListener);
        }
        
        private void AcceptNewConnection(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            while (IsSocketConnected(handler))
            {
                // Get the message from the socket
                Message message = SocketUtils.ReceiveMessage(handler);

                if (message.Type == MessageType.Request)
                {
                    // Synchronously call the server method associated to the command
                    object result = ExecuteCommand(message);

                    // Serialize the response
                    MemoryStream msResponse = new MemoryStream();
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(msResponse, result);

                    // frame the response
                    Message response = new Message()
                    {
                        Type = MessageType.Response,
                        Content = msResponse.ToArray()
                    };

                    // Send the response
                    handler.Send(response.ToArray());
                }
            }

            handler.Close();
        }

        /// <summary>
        /// Checks if the socket connection has been terminated by remote host (the client)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static bool IsSocketConnected(Socket s)
        {
            return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
        }

        private object ExecuteCommand(Message message)
        {
            // Get the request from the message
            MemoryStream ms = new MemoryStream(message.Content);
            BinaryFormatter bf = new BinaryFormatter();
            RequestMessageContent request = (RequestMessageContent)bf.Deserialize(ms);

            if (typeof(IServerBusiness).GetMethod(request.Command.MethodName, request.Command.ParameterTypes.ToArray()) != null)
            {
                // Interface contains method, get the real implementation. This is meant to protect against a client wanting to execute another method which he is not supposed to know of.
                MethodInfo methodImplementation = typeof(ServerBusiness).GetMethod(request.Command.MethodName, request.Command.ParameterTypes.ToArray());

                // Calls the method
                object methodResult = methodImplementation.Invoke(business, request.Parameters.ToArray());

                // Return the object returned by the method invocation
                return methodResult;
            }

            return null;
        }
    }
}
