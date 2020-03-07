using System;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using install.server;
using Maticsoft.DBUtility;
using System.Data;
using PeisPlatform.Helper;
using IniParser;
using IniParser.Model;
using System.Text;

namespace install
{
    public partial class Form1 : Form
    {
        public static string url = @"D:\\ProgramFiles";
        
        public Form1()
        {
            InitializeComponent();
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
                SetUpShortcut.CreateShortCut(nginxPath, Environment.GetFolderPath(Environment.SpecialFolder.Startup), "nginx.exe");
                textBox1.Text += "已nginx服务" + "\r\n";
                label1.Text = "已nginx服务";

                //创建快捷方式
                string PeisPlatformPath = "";
                GetFilePath.GetFile("D://", "PeisPlatform.exe", ref PeisPlatformPath);
                textBox1.Text += "正在创建快捷方式" + "\r\n";
                label1.Text = "正在创建快捷方式";
                SetUpShortcut.CreateShortCut(PeisPlatformPath, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "新版采集端");
                textBox1.Text += "已创建快捷方式" + "\r\n";
                label1.Text = "已创建快捷方式";

                //安装MySQL
                textBox1.Text += "正在安装MySQL" + "\r\n";
                label1.Text = "正在安装MySQL";
                string mysqld = Cmd.ExecCommand("mysqld --initialize --user=mysql --console", @"D:\ProgramFiles\mysql\bin");
                if (mysqld.Contains("A temporary password is generated for root@localhost:"))
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
                string install = Cmd.ExecCommand("mysqld --install", @"D:\ProgramFiles\mysql\bin");
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
                string startMysql = Cmd.ExecCommand("net start mysql", @"D:\ProgramFiles\mysql\bin");
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
                //连接MySQL修改密码
                textBox1.Text += "正在设置MySQL密码" + "\r\n";
                label1.Text = "正在设置MySQL密码";
                int i = DbHelperMySQL.ExecuteSql("update user set authentication_string=password('KunYujk1!') where user='root';");
                if (i > 0)
                {
                    textBox1.Text += "设置成功" + "\r\n";
                    label1.Text = "设置成功";
                }
                else
                {
                    textBox1.Text += "设置失败" + "\r\n";
                    label1.Text = "设置失败";
                }
                //添加新用户
                //设置ini文件
                string data = "[mysql]" + "\r\n"
                                + "# 设置mysql客户端默认字符集" + "\r\n"
                                + "default-character-set = utf8" + "\r\n"
                                + "[mysqld]" + "\r\n"
                                + "#设置3306端口" + "\r\n"
                                + "port = 3307" + "\r\n"
                                + "# 设置mysql的安装目录" + "\r\n"
                                + "basedir = D:\\ProgramFiles\\mysql" + "\r\n"
                                + "# 设置mysql数据库的数据的存放目录" + "\r\n"
                                + "datadir = D:\\ProgramFiles\\mysql\\data" + "\r\n"
                                + "# 允许最大连接数" + "\r\n"
                                + "max_connections = 200" + "\r\n"
                                + "# 服务端使用的字符集默认为8比特编码的latin1字符集" + "\r\n"
                                + "character-set-server = utf8" + "\r\n"
                                + "# 创建新表时将使用的默认存储引擎" + "\r\n"
                                + "default-storage-engine = INNODB" +"\r\n";
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                FileStream fs = new FileStream(@"D:\ProgramFiles\mysql\my.ini", FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                string restartMysql = Cmd.ExecCommand("net stop mysql", @"D:\ProgramFiles\mysql\bin");
                restartMysql = Cmd.ExecCommand("net start mysql", @"D:\ProgramFiles\mysql\bin");
                if (!restartMysql.Contains("服务已经启动成功"))
                {
                    textBox1.Text += "重启mysql服务失败" + "\r\n";
                    label1.Text = "重启mysql服务失败";
                }
                //添加新用户
                DbHelperMySQL.ExecuteSql("CREATE USER 'kyjk'@'%' IDENTIFIED BY 'KunYujk1!';");
                DbHelperMySQL.ExecuteSql("GRANT ALL ON *.* TO 'kyjk'@'%';");
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
            new CompressFile().UnZip(url + "\\" + fileName + "", url);
            textBox1.Text += "解压" + fileName + "结束" + "\r\n";
            label1.Text = "解压" + fileName + "结束";
            //删除mysql.zip文件
            textBox1.Text += "正在删除" + fileName + "" + "\r\n";
            label1.Text = "正在删除" + fileName + "";
            if (File.Exists(url + "\\" + fileName + ""))
            {
                File.Delete(url + "\\" + fileName + "");
                textBox1.Text += "已删除" + fileName + "" + "\r\n";
                label1.Text = "已删除" + fileName + "";
            }
        }
    }
}
