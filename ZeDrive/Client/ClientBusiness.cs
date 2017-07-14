using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLibrary.Models;
using Action = ShareLibrary.Models.Action;

namespace Client
{
    public class ClientBusiness
    {
        private string rootFolderPath;

        public ClientBusiness(string rootFolderPath)
        {
            this.rootFolderPath = rootFolderPath;
        }

        public void Connect() { }
        public void CreateGroup() { }
        public void SendJoinGroupRequest() { }
        public void SendGroupInvitation() { }
        public void AcceptInvitation() { }

        //Push generated revisions list
        public void UpdateServerHistory()
        {
        }

        /// <summary>
        /// Updating local version of the files based on the revisions received by the server.
        /// </summary>
        /// <param name="revisions">The revisions to apply localy.</param>
        public void UpdateLocalFiles(List<Revision> revisions)
        {
            foreach (Revision revision in revisions)
            {
                revision.Apply(rootFolderPath);
            }
        }

        public void ChangeAdministratorGroup() { }
        public void DeleteClientFromGroup() { }
    }
}
