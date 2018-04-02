using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VocalUtau.Formats.Model.Utils
{
    public class PathUtils
    {
        private static string basicpath = "";

        private static string FormatPath(string sPath)
        {
            if (File.Exists(sPath))
            {
                FileInfo fi = new System.IO.FileInfo(sPath);
                return fi.FullName;
            }
            else if (Directory.Exists(sPath))
            {
                DirectoryInfo fi = new System.IO.DirectoryInfo(sPath);
                return fi.FullName;
            }
            return sPath;
        }
        public static string RelativePath(string relativeTo)
        {
            string bFolder = relativeTo;
            if (File.Exists(relativeTo))
            {
                FileInfo fi = new System.IO.FileInfo(relativeTo);
                relativeTo = fi.FullName;
                bFolder = fi.Directory.FullName;
            }
            else
            {
                DirectoryInfo fi = new System.IO.DirectoryInfo(relativeTo);
                if (!fi.Exists) return "";
                relativeTo = fi.FullName;
                bFolder = fi.FullName;
            }
            string absolutePath = AppDomain.CurrentDomain.BaseDirectory;
            if (!bFolder.Contains(absolutePath))
            {
                return relativeTo;
            }
                

            if (relativeTo.Length > 3)
            {
                char c1 = relativeTo.ToLower()[0];
                char c2 = relativeTo[1];
                char c3 = relativeTo[2];
                if (c2 == ':' && c3 == '\\')
                {
                    if (c1 <= 'z' && c1 >= 'a')
                    {
                        if (c1 != absolutePath.ToLower()[0])
                        {
                            return relativeTo;
                        }
                    }
                }
            }
            //from - www.cnphp6.com

            string[] absoluteDirectories = absolutePath.Split('\\');
            string[] relativeDirectories = relativeTo.Split('\\');

            //Get the shortest of the two paths
            int length = absoluteDirectories.Length < relativeDirectories.Length ? absoluteDirectories.Length : relativeDirectories.Length;

            //Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            //Find common root
            for (index = 0; index < length; index++)
                if (absoluteDirectories[index] == relativeDirectories[index])
                    lastCommonRoot = index;
                else
                    break;

            //If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
                throw new ArgumentException("Paths do not have a common base");

            //Build up the relative path
            StringBuilder relativePath = new StringBuilder();

            //Add on the ..
            for (index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++)
                if (absoluteDirectories[index].Length > 0)
                    relativePath.Append("..\\");

            //Add on the folders
            for (index = lastCommonRoot + 1; index < relativeDirectories.Length - 1; index++)
                relativePath.Append(relativeDirectories[index] + "\\");
            relativePath.Append(relativeDirectories[relativeDirectories.Length - 1]);

            return relativePath.ToString();
        }
        public static string RelativePath(string baseFolder,string relativeTo)
        {
            relativeTo = FormatPath(relativeTo);

            string absolutePath = baseFolder;

            absolutePath = FormatPath(absolutePath);


            if (relativeTo.Length > 3)
            {
                char c1 = relativeTo.ToLower()[0];
                char c2 = relativeTo[1];
                char c3 = relativeTo[2];
                if (c2 == ':' && c3 == '\\')
                {
                    if (c1 <= 'z' && c1 >= 'a')
                    {
                        if (c1 != absolutePath.ToLower()[0])
                        {
                            return relativeTo;
                        }
                    }
                }
            }
            //from - www.cnphp6.com

            string[] absoluteDirectories = absolutePath.Split('\\');
            string[] relativeDirectories = relativeTo.Split('\\');

            //Get the shortest of the two paths
            int length = absoluteDirectories.Length < relativeDirectories.Length ? absoluteDirectories.Length : relativeDirectories.Length;

            //Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            //Find common root
            for (index = 0; index < length; index++)
                if (absoluteDirectories[index] == relativeDirectories[index])
                    lastCommonRoot = index;
                else
                    break;

            //If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
                throw new ArgumentException("Paths do not have a common base");

            //Build up the relative path
            StringBuilder relativePath = new StringBuilder();

            //Add on the ..
            for (index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++)
                if (absoluteDirectories[index].Length > 0)
                    relativePath.Append("..\\");

            //Add on the folders
            for (index = lastCommonRoot + 1; index < relativeDirectories.Length - 1; index++)
                relativePath.Append(relativeDirectories[index] + "\\");
            relativePath.Append(relativeDirectories[relativeDirectories.Length - 1]);

            return relativePath.ToString();
        }
        public static string AbsolutePath(string absoluteTo)
        {
            if (absoluteTo.Length > 3)
            {
                char c1 = absoluteTo.ToLower()[0];
                char c2 = absoluteTo[1];
                char c3 = absoluteTo[2];
                if(c2==':' && c3=='\\')
                {
                    if (c1 <= 'z' && c1 >= 'a')
                    {
                        return absoluteTo;
                    }
                }
            }
            string curP = Directory.GetCurrentDirectory();
            string ret = absoluteTo;
            try
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                ret = Path.GetFullPath(absoluteTo);
            }
            catch { ;}
            Directory.SetCurrentDirectory(curP);
            return FormatPath(ret);
        }
        public static string AbsolutePath(string baseFolder, string absoluteTo)
        {
            if (absoluteTo == "") return "";
            if (baseFolder != "")
            {
                string TestFolder = baseFolder + "\\" + absoluteTo;
                if (System.IO.File.Exists(TestFolder))
                {
                    return (new System.IO.FileInfo(TestFolder)).FullName;
                }
                if (System.IO.Directory.Exists(TestFolder))
                {
                    return (new System.IO.DirectoryInfo(TestFolder)).FullName;
                }
            }
            if (absoluteTo.Length > 3)
            {
                char c1 = absoluteTo.ToLower()[0];
                char c2 = absoluteTo[1];
                char c3 = absoluteTo[2];
                if (c2 == ':' && c3 == '\\')
                {
                    if (c1 <= 'z' && c1 >= 'a')
                    {
                        return absoluteTo;
                    }
                }
            }
            string curP = Directory.GetCurrentDirectory();
            string ret = absoluteTo;
            try
            {
                Directory.SetCurrentDirectory(baseFolder);
                ret = Path.GetFullPath(absoluteTo);
            }
            catch { ;}
            Directory.SetCurrentDirectory(curP);
            return FormatPath(ret);
        }


        public static List<FileInfo> GetFirstFiles(System.IO.DirectoryInfo dir,string[] patterns,int maxdeep)
        {
            List<FileInfo> ret = new List<FileInfo>();
            foreach (string pattern in patterns)
            {
                FileInfo[] file = dir.GetFiles(pattern);
                foreach (FileInfo fi in file)
                {
                    ret.Add(fi);
                    return ret;
                }
            }
            DirectoryInfo[] dis = dir.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                ret.AddRange(GetFirstFiles(di,patterns,maxdeep-1).ToArray());
            }
            return ret;
        }
    }
}
