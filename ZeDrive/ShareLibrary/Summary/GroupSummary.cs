using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShareLibrary.Models;
using Action = ShareLibrary.Models.Action;
using FileInfo = ShareLibrary.Models.FileInfo;

namespace ShareLibrary.Summary
{
    [Serializable]
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

        public void Update(Revision revisionToApply)
        {
            if (revisionToApply.Action == Action.Modify)
            {
                Files.RemoveAll(info => info.Name == revisionToApply.File.Name);
                Files.Add(revisionToApply.File);

            }
            else if (revisionToApply.Action == Action.Create)
            {
                Files.Add(revisionToApply.File);
            }
            else if (revisionToApply.Action == Action.Delete)
            {
                Files.RemoveAll(info => info.Name == revisionToApply.File.Name);
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
                    //Server Case
                    else if (file.LastModificationDate < previousVersionFileInfo.LastModificationDate)
                    {
                        revisions.Add(new Revision
                        {
                            Action = Action.Modify,
                            GroupName = this.GroupName,
                            File = previousVersionFileInfo
                        });

                    }
                }
            }
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
