using ShareLibrary.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.TcpCommunication
{
    public class Message
    {
        public static int SizeOfLengthInHeader = sizeof(int);
        public static int SizeOfTypeInHeader = sizeof(MessageType);
        public static int CompleteHeaderSize = SizeOfLengthInHeader + SizeOfTypeInHeader;

        public MessageType Type { get; set; }
        public byte[] Content { get; set; }
        public int Length
        {
            get
            {
                return Content.Length;
            }
        }

        public byte[] ToArray()
        {
            byte[] message = new byte[SizeOfLengthInHeader + SizeOfTypeInHeader + Content.Length];

            BitConverter.GetBytes(Length).CopyTo(message, 0);
            BitConverter.GetBytes((int)Type).CopyTo(message, SizeOfLengthInHeader);
            Content.CopyTo(message, SizeOfLengthInHeader + SizeOfTypeInHeader);

            return message;
        }
    }
}
