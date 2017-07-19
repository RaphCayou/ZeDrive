using Server.TcpCommunication;
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
        public static Message ReceiveMessage(Socket s)
        {
            s.ReceiveTimeout = 1000;

            // Receive message length
            byte[] messageLengthArray = new byte[Message.SizeOfLengthInHeader];
            while (true)
            {
                try
                {
                    s.Receive(messageLengthArray, messageLengthArray.Length, SocketFlags.Partial);
                    break;
                }
                catch (SocketException e)
                {
                    // receive timeout exceded, we cancel the reception if no data was received at all
                    if (s.Available == 0)
                    {
                        return null;
                    }
                }
            }

            s.ReceiveTimeout = 0;
            
            int messageLength = BitConverter.ToInt32(messageLengthArray, 0);


            // Receive message type
            byte[] messageTypeArray = new byte[Message.SizeOfTypeInHeader];
            s.Receive(messageTypeArray, messageTypeArray.Length, SocketFlags.Partial);
            
            MessageType messageType = (MessageType)BitConverter.ToInt32(messageTypeArray, 0);


            // Receive message content
            byte[] messageContent = new byte[messageLength];
            // TODO Make sure there is no timeout on the Receive. If there is one we must handle it because it's a network error.
            // TODO Also check for other end of connection failure. For example, if the server closes, the client can send a request that is not going to be answered. We need to handle this.
            if (messageLength > 0)
            {
                s.Receive(messageContent, messageLength, SocketFlags.None);
            }

            Message message = new Message()
            {
                Type = messageType,
                Content = messageContent
            };

            return message;
        }
    }
}
