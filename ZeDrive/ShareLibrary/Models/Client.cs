using System;

namespace ShareLibrary.Models
{
    [Serializable]
    public class Client
    {
        public string Name;
        public string Password;
        public DateTime LastSeen;
    }
}
