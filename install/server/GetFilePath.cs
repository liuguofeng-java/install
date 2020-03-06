using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace install.server
{
    class GetFilePath
    {
        public static void GetFile(string path, string fileName, ref string smallDir)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Name.Contains("System Volume Information"))
                return;
            FileInfo[] fil = dir.GetFiles();
            DirectoryInfo[] dii = dir.GetDirectories();
            foreach (FileInfo f in fil)
            {
                string name = Path.GetFileName(f.FullName.ToString());
                if (name.Contains(fileName))
                {
                    smallDir = f.FullName;
                    return;
                }
            }
            foreach (DirectoryInfo d in dii)
            {
                if (smallDir == "")
                    GetFile(d.FullName, fileName, ref smallDir);
            }
        }
    }
}
