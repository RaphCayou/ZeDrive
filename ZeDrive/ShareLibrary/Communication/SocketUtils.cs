using Server.TcpCommunication;
using ShareLibrary.Communication.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShareLibrary.Communication
{
    public class SocketUtils
    {
        /// <summary>
        /// Receive a full message on an open socket. Throws errors if a problem occured.
        /// </summary>
        /// <param name="s">Socket with open connection</param>
        /// <param name="waitTimeForNewMessage">Time (milliseconds) to wait for a new message to start arriving (throws NoNewMessageException on expiration)</param>
        /// <param name="timeoutForLostConnectivity">Time (milliseconds) with no more data received to consider the connection as lost (throws MessageInterruptedException on expiration)</param>
        /// <returns>Message received, no integrity check</returns>
        public static Message ReceiveMessage(Socket s, int waitTimeForNewMessage, int timeoutForLostConnectivity)
        {
            // For the beginning of the message, we wait for a certain amount of time. This is different from a connection failure.
            s.ReceiveTimeout = waitTimeForNewMessage;

            // Receive message length
            byte[] messageLengthArray = new byte[Message.SizeOfLengthInHeader];
            while (true)
            {
                try
                {
                    ReceiveAllBuffer(s, SocketFlags.Partial, messageLengthArray.Length, ref messageLengthArray);
                    break;
                }
                catch (SocketException e)
                {
                    // receive timeout exceded, we cancel the reception
                    throw new NoNewMessageException();
                }
            }

            // After receiving the length of the message, we must receive the rest of the message.
            // If we receive a part of the data, but not the whole message, we consider the connection as interrupted or lost.
            s.ReceiveTimeout = timeoutForLostConnectivity;
            
            int messageLength = BitConverter.ToInt32(messageLengthArray, 0);
            
            // Receive message type
            byte[] messageTypeArray = new byte[Message.SizeOfTypeInHeader];
            try
            {
                ReceiveAllBuffer(s, SocketFlags.Partial, messageTypeArray.Length, ref messageTypeArray);
            }
            catch (SocketException e)
            {
                throw new MessageInterruptedException();
            }
            MessageType messageType = (MessageType)BitConverter.ToInt32(messageTypeArray, 0);
            
            // Receive message content
            byte[] messageContent = new byte[messageLength];
            if (messageLength > 0)
            {
                try
                {
                    ReceiveAllBuffer(s, SocketFlags.None, messageLength, ref messageContent);
                }
                catch (SocketException e)
                {
                    // Timeout occured. Message is incomplete
                    throw new MessageInterruptedException();
                }
            }

            Message message = new Message()
            {
                Type = messageType,
                Content = messageContent
            };

            return message;
        }

        static public void SendMessage(Socket s, Message message)
        {
            byte[] buffer = message.ToArray();
            SendAllBuffer(s, SocketFlags.None, buffer.Length, buffer);
        }

        static private void ReceiveAllBuffer(Socket handler, SocketFlags flags, int length, ref byte[] buffer)
        {
            int writePosition = 0;
            while (writePosition < length)
            {
                writePosition += handler.Receive(buffer, writePosition, length - writePosition, flags);
            }
        }

        static private void SendAllBuffer(Socket handler, SocketFlags flags, int length, byte[] buffer)
        {
            int writePosition = 0;
            while (writePosition < length)
            {
                writePosition += handler.Send(buffer, writePosition, length - writePosition, flags);
            }
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
    }
}
