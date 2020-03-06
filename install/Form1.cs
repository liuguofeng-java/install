using System;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using install.server;

namespace install
{
    public partial class Form1 : Form
    {
        public static string url = @"D:\\ProgramFiles";
        
        public Form1()
        {
            InitializeComponent();
            string connectMysql = Cmd.RunCmd("mysql -uroot -p1234", @"D:\ProgramFiles\mysql\bin");
        }

        /// <summary>
        /// 点击确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //解压mysql
                CompressFileAndDeleFile("mysql");
                //解压nginx
                CompressFileAndDeleFile("nginx");
                //解压mysql
                CompressFileAndDeleFile("PEIS");

                //开启nginx服务
                string nginxPath = "";
                GetFilePath.GetFile("D://", "nginx.exe", ref nginxPath);
                textBox1.Text += "正在nginx服务" + "\r\n";
                label1.Text = "正在nginx服务";
                SetUpShortcut.CreateShortCut(nginxPath, Environment.GetFolderPath(Environment.SpecialFolder.Startup),"nginx.exe");
                textBox1.Text += "已nginx服务" + "\r\n";
                label1.Text = "已nginx服务";

                //创建快捷方式
                string PeisPlatformPath = "";
                GetFilePath.GetFile("D://", "PeisPlatform.exe", ref PeisPlatformPath);
                textBox1.Text += "正在创建快捷方式" + "\r\n";
                label1.Text = "正在创建快捷方式";
                SetUpShortcut.CreateShortCut(PeisPlatformPath, Environment.GetFolderPath(Environment.SpecialFolder.Startup), "新版采集端");
                textBox1.Text += "已创建快捷方式" + "\r\n";
                label1.Text = "已创建快捷方式";

                //安装MySQL
                textBox1.Text += "正在安装MySQL" + "\r\n";
                label1.Text = "正在安装MySQL";
                string mysqld = Cmd.RunCmd("mysqld --initialize --user=mysql --console", @"D:\ProgramFiles\mysql\bin");
                if(mysqld.Contains("A temporary password is generated for root@localhost:"))
                {
                    textBox1.Text += "成功获取MySQL随机密码" + "\r\n";
                    label1.Text = "成功获取MySQL随机密码";
                }
                else
                {
                    textBox1.Text += "安装MySQL出错了" + "\r\n";
                    label1.Text = "安装MySQL出错了";
                    return;
                }
                //安装MySQL服务
                string install = Cmd.RunCmd("mysqld --install", @"D:\ProgramFiles\mysql\bin");
                if (install.Contains("Service successfully installed."))
                {
                    textBox1.Text += "服务成功安装" + "\r\n";
                    label1.Text = "服务成功安装";
                }
                else
                {
                    textBox1.Text += "无法安装MySQL服务" + "\r\n";
                    label1.Text = "无法安装MySQL服务";
                    return;
                }
                //启动MySQL服务
                string startMysql = Cmd.RunCmd("net start mysql", @"D:\ProgramFiles\mysql\bin");
                if (startMysql.Contains("服务已经启动成功"))
                {
                    textBox1.Text += "服务已经启动成功" + "\r\n";
                    label1.Text = "服务已经启动成功";
                }
                else
                {
                    textBox1.Text += "无法启动MySQL服务" + "\r\n";
                    label1.Text = "无法启动MySQL服务";
                    return;
                }
                //连接MySQL
                string connectMysql = Cmd.RunCmd("net start mysql", @"D:\ProgramFiles\mysql\bin");
                if (mysqld.Contains("服务已经启动成功"))
                {
                    textBox1.Text += "服务已经启动成功" + "\r\n";
                    label1.Text = "服务已经启动成功";
                }
                else
                {
                    textBox1.Text += "无法启动MySQL服务" + "\r\n";
                    label1.Text = "无法启动MySQL服务";
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void CompressFileAndDeleFile(string fileName)
        {
            fileName = fileName + ".zip";
            textBox1.Text += "正在解压"+ fileName + "\r\n";
            label1.Text = "正在解压" + fileName + "";
            bool mysql = CompressFile.UnZip(url + "\\" + fileName + "", url, null);
            if (mysql)
            {
                textBox1.Text += "解压" + fileName + "结束" + "\r\n";
                label1.Text = "解压" + fileName + "结束";
            }
            else
            {
                textBox1.Text += "解压" + fileName + "失败" + "\r\n";
                label1.Text = "解压" + fileName + "失败";
                return;
            }
            //删除mysql.zip文件
            textBox1.Text += "正在删除" + fileName + "" + "\r\n";
            label1.Text = "正在删除" + fileName + "";
            if (File.Exists(url + "\\" + fileName + ""))
            {
                Thread.Sleep(1000);
                File.Delete(url + "\\" + fileName + "");
                textBox1.Text += "已删除" + fileName + "" + "\r\n";
                label1.Text = "已删除" + fileName + "";
            }
        }
    }
}
