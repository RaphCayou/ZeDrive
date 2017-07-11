namespace ShareLibrary.Models
{
    public enum Action
    {
        Create,
        Modify,
        Delete,
    }
    public class Revision
    {
        public Action Action;
        public FileInfo File;
        public string GroupName;
        public byte[] Data;
    }
}
