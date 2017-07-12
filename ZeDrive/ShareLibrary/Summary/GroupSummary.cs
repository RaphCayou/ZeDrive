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

        /// <summary>
        /// Constructor of GroupSummary with the group name and the root folder of all groups.
        /// </summary>
        /// <param name="groupName">Name of the group to construct the GroupSummary.</param>
        /// <param name="rootFolderPath">Root folder containing all groups.</param>
        public GroupSummary(string groupName, string rootFolderPath)
        {
            GroupName = groupName;
            rootFolderpath = PathAddSlash(rootFolderPath);
            Update();
        }

        public void Update()
        {
            Files = new List<FileInfo>();
            IEnumerable<string> filePaths = Directory.EnumerateFiles(rootFolderpath + GroupName);
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
            IEnumerable<FileInfo> removedFiles = oldGroupSummary.Files.Except(Files);
            revisions.AddRange(removedFiles.Select(removedFile => new Revision
            {
                Action = Action.Delete,
                GroupName = this.GroupName,
                File = removedFile
            }));

            return revisions;
        }

        /// <summary>
        /// Adding the corresponding caracter to the path. (/ or \ depending of the rest of the path)
        /// <note>
        /// Based on : https://stackoverflow.com/questions/20405965/how-to-ensure-there-is-trailing-directory-separator-in-paths
        /// Taken on 2017-07-06
        /// </note>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string PathAddSlash(string path)
        {
            // They're always one character but EndsWith is shorter than
            // array style access to last path character. Change this
            // if performance are a (measured) issue.
            string separator1 = Path.DirectorySeparatorChar.ToString();
            string separator2 = Path.AltDirectorySeparatorChar.ToString();

            // Trailing white spaces are always ignored but folders may have
            // leading spaces. It's unusual but it may happen. If it's an issue
            // then just replace TrimEnd() with Trim(). Tnx Paul Groke to point this out.
            path = path.TrimEnd();

            // Argument is always a directory name then if there is one
            // of allowed separators then I have nothing to do.
            if (path.EndsWith(separator1) || path.EndsWith(separator2))
                return path;

            // If there is the "alt" separator then I add a trailing one.
            // Note that URI format (file://drive:\path\filename.ext) is
            // not supported in most .NET I/O functions then we don't support it
            // here too. If you have to then simply revert this check:
            // if (path.Contains(separator1))
            //     return path + separator1;
            //
            // return path + separator2;
            if (path.Contains(separator2))
                return path + separator2;

            // If there is not an "alt" separator I add a "normal" one.
            // It means path may be with normal one or it has not any separator
            // (for example if it's just a directory name). In this case I
            // default to normal as users expect.
            return path + separator1;
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

        private byte[] GetFileData(string fileName)
        {
            return System.IO.File.ReadAllBytes(PathAddSlash(rootFolderpath + GroupName) + fileName);
        }
    }
}
