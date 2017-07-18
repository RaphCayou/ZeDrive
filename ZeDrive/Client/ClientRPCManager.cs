using Server.TcpCommunication;
using ShareLibrary.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientRPCManager
    {
        private Socket socketListener;

        public ClientRPCManager(string ipAddress, int port)
        {
            // Can throw if not valid IpAddress
            IPAddress serverIP = IPAddress.Parse(ipAddress);

            // Can throw if invalid port 
            EndPoint ep = new IPEndPoint(serverIP, port);

            socketListener = new Socket(serverIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socketListener.Connect(ep);
        }

        public object SendMessage(MethodBase methodInfo)
        {
            return SendMessageToServer(methodInfo, new List<object>());
        }

        public object SendMessage(MethodBase methodInfo, params object[] parameters)
        {
            return SendMessageToServer(methodInfo, parameters.ToList());
        }

        private object SendMessageToServer(MethodBase methodInfo, List<object> parameters)
        {
            // Create request
            RequestMessageContent request = new RequestMessageContent()
            {
                Command = new CommandHeader()
                {
                    MethodName = methodInfo.Name,
                    ParameterAssemblyQualifiedNames = methodInfo.GetParameters().Select(p => p.GetType().AssemblyQualifiedName).ToList()
                },
                Parameters = parameters
            };

            // Serialize the request
            MemoryStream msRequest = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(msRequest, request);

            // Frame in message
            Message message = new Message()
            {
                Type = MessageType.Request,
                Content = msRequest.ToArray()
            };

            // Send over socket
            socketListener.Send(message.ToArray());

            // Wait for server reply
            Message response = SocketUtils.ReceiveMessage(socketListener);

            object result = null;

            if (response.Type == MessageType.Response)
            {
                // When server reply complete, unpack the response
                MemoryStream ms = new MemoryStream(response.Content);
                result = bf.Deserialize(ms);
            }

            // Return the response
            return result;
        }
    }
}
