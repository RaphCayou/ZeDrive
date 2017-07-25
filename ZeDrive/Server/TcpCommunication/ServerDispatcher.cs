using ShareLibrary.Communication;
using ShareLibrary.Communication.Exceptions;
using ShareLibrary.Utils;
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
        public class StateObject
        {
            public StateObject(Socket s, Message m)
            {
                workSocket = s;
                message = m;
            }
            public Socket workSocket;
            public int writePosition = 0;
            public Message message;
        }

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

            StartListeningForAnotherConnection();
        }

        public void CloseConnection()
        {
            try
            {
                socketListener.Shutdown(SocketShutdown.Both);
            }
            catch { }
            socketListener.Close();
        }

        private void StartListeningForAnotherConnection()
        {
            socketListener.BeginAccept(Message.CompleteHeaderSize, AcceptNewConnection, socketListener);
        }
        
        private void AcceptNewConnection(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;

            // Receive message length
            byte[] messsageHeaderBuffer;
            int bytesTransferred;
            Socket handler = null;
            try
            {
                handler = listener.EndAccept(out messsageHeaderBuffer, out bytesTransferred, ar);

                StartListeningForAnotherConnection();
            }
            catch
            {
                // Happens when the socket is closed. AcceptNewConnection is called a last time but EndAccept throws
                return;
            }
            
            if (bytesTransferred != Message.CompleteHeaderSize)
            {
                // TODO Trace this : Never supposed to happen, means that we did not read the complete header, which means that the message cannot be retreived correctly.
                //throw new NoNewMessageException();
            }

            int messageLength = BitConverter.ToInt32(messsageHeaderBuffer, 0);
            MessageType messageType = (MessageType)BitConverter.ToInt32(messsageHeaderBuffer, Message.SizeOfLengthInHeader);

            Message message = new Message
            {
                Type = messageType,
                Content = new byte[messageLength]
            };
            StateObject state = new StateObject(handler, message);

            // Asynchronously receive rest of message
            try
            {
                handler.BeginReceive(message.Content, 0, message.Length, 0, ReadCallback, state);
            }
            catch
            {
                // TODO Trace this : May fail if the socket was closed by the client during the transmission
            }
        }

        public void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            Message message = state.message;

            // Read data from the client socket.   
            int bytesRead = 0;
            try
            {
                // Try to end receive
                bytesRead = handler.EndReceive(ar);
            }
            catch
            {
                // Happens when the socket is closed. ReadCallback is called a last time but EndReceive throws
                return;
            }

            if (bytesRead == 0)
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                // TODO Trace this : If not enough data read, connection must have been interrupted
                //throw new MessageInterruptedException();
            }
            else if (state.writePosition + bytesRead < message.Length)
            {
                // There is more data to come
                state.writePosition += bytesRead;

                // Asynchronously receive rest of message
                try
                {
                    handler.BeginReceive(message.Content, state.writePosition, message.Length - state.writePosition, 0, ReadCallback, state);
                }
                catch
                {
                    // TODO Trace this : May fail if the socket was closed by the client during the transmission
                }
            }
            else
            {
                // Here we assume we have the complete message content
                if (message.Type == MessageType.Request)
                {
                    // Synchronously call the server method associated to the command
                    object result = ExecuteCommand(message);
                    Message response = new Message()
                    {
                        Type = MessageType.Response,
                        Content = new byte[0]
                    };

                    if (result != null)
                    {
                        // Serialize the response
                        MemoryStream msResponse = new MemoryStream();
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(msResponse, result);

                        // frame the response
                        response.Content = msResponse.ToArray();
                    }

                    byte[] messageBuffer = response.ToArray();

                    //TraceLog.Trace(message.Length.ToString(), System.Text.Encoding.Default.GetString(messageBuffer));

                    StateObject stateSend = new StateObject(handler, response);
                    try
                    {
                        handler.BeginSend(messageBuffer, 0, messageBuffer.Length, 0, SendCallback, stateSend);
                    }
                    catch
                    {
                        // TODO Trace this : May fail if the socket was closed by the client during the transmission
                    }
                }
                else
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            Message message = state.message;

            // Complete sending the data to the remote device.  
            int bytesSent = 0;
            try
            {
                bytesSent = handler.EndSend(ar);
            }
            catch
            {
                // TODO Trace this : Happens when the socket is closed but there is data that was not yet sent.
                return;
            }

            if (bytesSent != 0 && bytesSent + state.writePosition < message.Length + Message.CompleteHeaderSize)
            {
                // If data not sent entirely, send the rest
                state.writePosition += bytesSent;
                byte[] messageBuffer = message.ToArray();

                try
                {
                    handler.BeginSend(messageBuffer, state.writePosition, messageBuffer.Length - state.writePosition, 0, SendCallback, state);
                }
                catch
                {
                    // TODO Trace this : May fail if the socket was closed by the client during the transmission
                }
            }
            else
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }

        private object ExecuteCommand(Message message)
        {
            // Validate that the message is not empty
            if (message.Length == 0)
            {
                return null;
            }

            // Get the request from the message
            MemoryStream ms = new MemoryStream(message.Content);
            BinaryFormatter bf = new BinaryFormatter();
            RequestMessageContent request = (RequestMessageContent)bf.Deserialize(ms);

            // Validate that the method exists within the IServerBusiness interface
            if (typeof(IServerBusiness).GetMethod(request.Command.MethodName, request.Command.ParameterTypes.ToArray()) == null)
            {
                return null;
            }
            
            // Interface contains method, get the real implementation. This is meant to protect against a client wanting to execute another method which he is not supposed to know of.
            MethodInfo methodImplementation = typeof(ServerBusiness).GetMethod(request.Command.MethodName, request.Command.ParameterTypes.ToArray());

            // Calls the method
            object methodResult = null;
            try
            {
                methodResult = methodImplementation.Invoke(business, request.Parameters.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                return ex.InnerException; // original exception throw by the method invoked
            }

            // Return the object returned by the method invocation
            return methodResult;
        }
    }
}
