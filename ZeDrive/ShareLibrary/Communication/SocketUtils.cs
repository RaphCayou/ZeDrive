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
            // Receive message length
            byte[] messageLengthArray = new byte[Message.SizeOfLengthInHeader];
            s.Receive(messageLengthArray, messageLengthArray.Length, SocketFlags.Partial);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(messageLengthArray);
            }
            int messageLength = BitConverter.ToInt32(messageLengthArray, 0);


            // Receive message type
            byte[] messageTypeArray = new byte[Message.SizeOfTypeInHeader];
            s.Receive(messageTypeArray, messageTypeArray.Length, SocketFlags.Partial);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(messageTypeArray);
            }
            MessageType messageType = (MessageType)BitConverter.ToInt32(messageLengthArray, 0);


            // Receive message content
            byte[] messageContent = new byte[messageLength];
            // TODO Make sure there is no timeout on the Receive. If there is one we must handle it because it's a network error.
            // TODO Also check for other end of connection failure. For example, if the server closes, the client can send a request that is not going to be answered. We need to handle this.
            s.Receive(messageContent, messageLength, SocketFlags.None);

            Message message = new Message()
            {
                Type = messageType,
                Content = messageContent
            };

            return message;
        }
    }
}
