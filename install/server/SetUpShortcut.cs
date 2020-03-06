using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace install.server
{
    class SetUpShortcut
    {
        /// <summary>
        /// Environment.GetFolderPath(Environment.SpecialFolder.Startup)
        /// </summary>
        /// <param name="FileStreamPath"></param>
        /// <param name=""></param>
        public static void CreateShortCut(string FileStreamPath, string DesktopPath,string name)
        {
            WshShell shell = new WshShell();
            FileInfo fileInfo = new FileInfo(FileStreamPath);
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(DesktopPath + "\\" + name + ".lnk");
            shortcut.TargetPath = FileStreamPath;
            shortcut.Arguments = "";
            shortcut.Description = "快捷方式";
            shortcut.WorkingDirectory = fileInfo.DirectoryName;
            shortcut.Hotkey = "CTRL+N";
            shortcut.WindowStyle = 1;
            shortcut.Save();
        }
    }
}
