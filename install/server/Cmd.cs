using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace install.server
{
    class Cmd
    {
        /// <summary>
        /// 执行系统CMD命令
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns></returns>
        public static string ExecCommand(string commandText, string cmdPath)
        {
            Process process = new Process();//启动Process进程管理对象
            process.StartInfo.FileName = "cmd.exe";//调用cmd shell
            process.StartInfo.UseShellExecute = false;//设置是否启用操作系统的Shell启动进程
            process.StartInfo.RedirectStandardInput = true;//应用程序的输入是否从RedirectStandardInput流中获取
            process.StartInfo.RedirectStandardOutput = true;//应用程序的输出是否写到RedirectStandardOutput流中
            process.StartInfo.RedirectStandardError = true;//是否将错误信息写入到RedirectStandardError流中
            process.StartInfo.CreateNoWindow = true;//是否在新窗口中启动该进程
                                                    //p.StartInfo.WorkingDirectory = Server.MapPath("../resources/") + sessionPath;
            string strOutput = string.Empty;
            string error = string.Empty;
            try
            {
                process.Start();
                process.StandardInput.WriteLine(" cd " + cmdPath + "");
                process.StandardInput.WriteLine(" d: ");
                process.StandardInput.WriteLine(commandText);
                process.StandardInput.WriteLine("exit");
                strOutput = process.StandardOutput.ReadToEnd();
                error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                process.Close();
            }
            catch (Exception e)
            {
                strOutput = e.Message;
                // LogHelper.WriteMessageErrorLog(e.Message);
            }
            return strOutput + error;
        }
        /// <summary>
        /// 执行系统CMD命令
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns></returns>
        public static string ExecCommand(string commandText,string commandText1, string cmdPath)
        {
            Process process = new Process();//启动Process进程管理对象
            process.StartInfo.FileName = "cmd.exe";//调用cmd shell
            process.StartInfo.UseShellExecute = false;//设置是否启用操作系统的Shell启动进程
            process.StartInfo.RedirectStandardInput = true;//应用程序的输入是否从RedirectStandardInput流中获取
            process.StartInfo.RedirectStandardOutput = true;//应用程序的输出是否写到RedirectStandardOutput流中
            process.StartInfo.RedirectStandardError = true;//是否将错误信息写入到RedirectStandardError流中
            process.StartInfo.CreateNoWindow = true;//是否在新窗口中启动该进程
                                                    //p.StartInfo.WorkingDirectory = Server.MapPath("../resources/") + sessionPath;
            string strOutput = string.Empty;
            string error = string.Empty;
            try
            {
                process.Start();
                process.StandardInput.WriteLine(" cd " + cmdPath + "");
                process.StandardInput.WriteLine(" d: ");
                process.StandardInput.WriteLine(commandText);
                process.StandardInput.WriteLine(commandText1);
                process.StandardInput.WriteLine("exit");
                strOutput = process.StandardOutput.ReadToEnd();
                error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                process.Close();
            }
            catch (Exception e)
            {
                strOutput = e.Message;
                // LogHelper.WriteMessageErrorLog(e.Message);
            }
            return strOutput + error;
        }
    }
}
