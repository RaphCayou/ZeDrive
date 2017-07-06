namespace ShareLibrary
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
        public File File;
        public Group Group;
        public byte[] Data;
    }
}
