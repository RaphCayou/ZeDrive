using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShareLibrary.Models;
using FileInfo = ShareLibrary.Models.FileInfo;

namespace ShareLibrary.Summary
{
    public class GroupSummary
    {
        public string GroupName;
        public List<FileInfo> Files;
        private string rootFolderpath;

        public GroupSummary() { }
        /// <summary>
        /// Constructor of GroupSummary with the group name and the root folder of all groups.
        /// </summary>
        /// <param name="groupName">Name of the group to construct the GroupSummary.</param>
        /// <param name="rootFolderPath">Root folder containing all groups.</param>
        public GroupSummary(string groupName, string rootFolderPath)
        {
            GroupName = groupName;
            rootFolderpath = rootFolderPath;
            Update();
        }

        public void Update()
        {
            Files = new List<FileInfo>();
            IEnumerable<string> filePaths = Directory.EnumerateFiles(Path.Combine(rootFolderpath, GroupName));
            foreach (string filePath in filePaths)
            {
                FileInfo fileInfo = new FileInfo
                {
                    Name = Path.GetFileName(filePath),
                    CreationDate = System.IO.File.GetCreationTime(filePath),
                    LastModificationDate = System.IO.File.GetLastWriteTime(filePath)
                };
                Files.Add(fileInfo);
            }

        }

        /// <summary>
        /// Generate the revisions list based on an older GroupSummary.
        /// </summary>
        /// <param name="oldGroupSummary">The previous version of the folder summary.</param>
        /// <returns>The list of all revisions in the given group.</returns>
        public List<Revision> GenerateRevisions(GroupSummary oldGroupSummary)
        {
            List<Revision> revisions = new List<Revision>();
            List<string> oldFilesNames = oldGroupSummary.Files.Select(info => info.Name).ToList();

            foreach (FileInfo file in Files)
            {
                FileInfo previousVersionFileInfo = oldGroupSummary.Files.Find(f => f.Name == file.Name);

                //The file was not in the old version, so we need to create the add revision
                if (previousVersionFileInfo == null)
                {
                    revisions.Add(new Revision
                    {
                        GroupName = this.GroupName,
                        Action = Action.Create,
                        Data = GetFileData(file.Name),
                        File = file
                    });
                }
                else
                {
                    //We remove the file from the old list to notify that we have found it.
                    oldFilesNames.Remove(file.Name);

                    //We can compare the modifry date to find if the file have been modify.
                    if (file.LastModificationDate > previousVersionFileInfo.LastModificationDate)
                    {
                        revisions.Add(new Revision
                        {
                            Action = Action.Modify,
                            GroupName = this.GroupName,
                            Data = GetFileData(file.Name),
                            File = file
                        });
                    }
                }
            }
            //TODO trouver une façon plus clean de get les files qui sont removed
            IEnumerable<FileInfo> removedFiles = oldGroupSummary.Files.Where(info => oldFilesNames.Contains(info.Name));
            revisions.AddRange(removedFiles.Select(removedFile => new Revision
            {
                Action = Action.Delete,
                GroupName = this.GroupName,
                File = removedFile
            }));

            return revisions;
        }

        /// <summary>
        /// Equality test of two instance of GroupSummary. Based on the group name
        /// </summary>
        /// <param name="a">Fisrt instance of a GroupSummary</param>
        /// <param name="b">Second instance of a GroupSummary</param>
        /// <returns>Return true if they have the same name.</returns>
        public static bool operator ==(GroupSummary a, GroupSummary b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            // Return true if the fields match:
            return a.GroupName == b.GroupName;
        }

        public static bool operator !=(GroupSummary a, GroupSummary b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Read the bytes of a file
        /// </summary>
        /// <param name="fileName">The name of the file to read the bytes.</param>
        /// <returns>The bytes of the file.</returns>
        private byte[] GetFileData(string fileName)
        {
            return System.IO.File.ReadAllBytes(Path.Combine(rootFolderpath, GroupName, fileName));
        }
    }
}
