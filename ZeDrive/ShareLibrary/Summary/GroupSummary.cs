using System.Collections.Generic;
using System.IO;

namespace ShareLibrary
{
    public class GroupSummary
    {
        public string GroupName;
        public List<File> Files;

        /// <summary>
        /// Constructor of GroupSummary with the group name and the root folder of all groups.
        /// </summary>
        /// <param name="groupName">Name of the group to construct the GroupSummary.</param>
        /// <param name="rootFolderPath">Root folder containing all groups.</param>
        public GroupSummary(string groupName, string rootFolderPath)
        {
            GroupName = groupName;
            IEnumerable<string> filePaths = Directory.EnumerateFiles(PathAddBackslash(rootFolderPath) + groupName);
            foreach (string filePath in filePaths)
            {
                File file = new File
                {
                    Name = Path.GetFileName(filePath),
                    CreationDate = System.IO.File.GetCreationTime(filePath),
                    LastModificationDate = System.IO.File.GetLastWriteTime(filePath)
                };
                Files.Add(file);
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

            foreach (File file in Files)
            {
                File previousVersionFile = oldGroupSummary.Files.Find(f => f.Name == file.Name);
                //TODO voir ça fait quoi si le fichier n'est pas la liste
                //ensuite faire la liste de revision selon la différence entre les 2 listes
                //TODO on va devoir trouver une façon de lister les fichiers qui ont été supprimé
            }

            return revisions;
        }

        //Based on : https://stackoverflow.com/questions/20405965/how-to-ensure-there-is-trailing-directory-separator-in-paths
        //Taken on 2017-07-06
        public string PathAddBackslash(string path)
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
    }
}
