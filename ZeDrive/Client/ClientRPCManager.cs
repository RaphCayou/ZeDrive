﻿using Server.TcpCommunication;
using ShareLibrary.Communication;
using ShareLibrary.Communication.Exceptions;
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
        private EndPoint ep;

        public ClientRPCManager(string ipAddress, int port)
        {
            // Can throw if not valid IpAddress
            IPAddress serverIP = IPAddress.Parse(ipAddress);

            // Can throw if invalid port 
            ep = new IPEndPoint(serverIP, port);
        }

        private Socket InitializeSocket()
        {
            return new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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
            Socket socketListener = InitializeSocket();
            socketListener.Connect(ep);

            // Create request
            RequestMessageContent request = new RequestMessageContent()
            {
                Command = new CommandHeader()
                {
                    MethodName = methodInfo.Name,
                    ParameterAssemblyQualifiedNames = methodInfo.GetParameters().Select(p => p.ParameterType.AssemblyQualifiedName).ToList()
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
            Message response = null;
            try
            {
                response = SocketUtils.ReceiveMessage(socketListener, 1000, 1000);
            }
            catch (NoNewMessageException ex)
            {
                // TODO must either redo the request or reset the connection
            }
            catch (MessageInterruptedException ex)
            {
                // TODO Must disconnect and reconnect the socket (reset connection)
            }

            object result = null;

            if (response != null && response.Type == MessageType.Response && response.Length > 0)
            {
                // When server reply complete and valid, unpack the response
                MemoryStream ms = new MemoryStream(response.Content);
                result = bf.Deserialize(ms);

                // If the response is an exception, return exception
                bool isException = result.GetType().IsSubclassOf(typeof(Exception));
                if (isException)
                {
                    // Release the socket.  
                    socketListener.Shutdown(SocketShutdown.Both);
                    socketListener.Close();
                    throw (Exception)result;
                }
            }

            // Release the socket.  
            socketListener.Shutdown(SocketShutdown.Both);
            socketListener.Close();

            // Return the response
            return result;
        }
    }
}
