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
        //委托支持异步
        static CompressFile compressFile = new CompressFile();
        delegate void SetTextCallback(string text);
        delegate void SetTextPanel(int text);
        static string url = @"D:\ProgramFiles";
        static string mysqlBinUrl = url + @"\mysql\bin";
        static string myIniPath = url + @"\mysql\my.ini";
        static int panel = 30;
        
        public Form1()
        {
            //压缩文件输出提示
            compressFile.unZipMsg += this.UpZipMsg;
            InitializeComponent();
        }
        //异步给input赋值
        private void SetTextBox(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTextBox);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.Text += text + "\r\n";
            }
        }
        //异步给label赋值
        private void SetLabel(string text)
        {
            if (this.label1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetLabel);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.label1.Text = text;
            }
        }
        //异步给panel赋值
        private void SetPanel(int text)
        {
            if (this.label1.InvokeRequired)
            {
                SetTextPanel d = new SetTextPanel(SetPanel);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.panel2.Width += text;
            }
        }
        //简化调用代码
        public void ExecuteMethod(string text)
        {
            this.SetTextBox(text);
            this.SetLabel(text);
            this.SetPanel(panel);
        }
        //压缩文件输出
        public void UpZipMsg(string text)
        {
            this.SetTextBox(text);
            this.SetLabel(text);
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
                Thread t = new Thread(new ThreadStart(server));
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void server()
        {
            //解压mysql
            CompressFileAndDeleFile("mysql");
            ExecuteMethod("正在添加my.ini");
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
                            + "default-storage-engine = INNODB" + "\r\n"
                            + "# 超时时间" + "\r\n"
                            + "default_password_lifetime=0" + "\r\n"
                            + "# 跳过密码" + "\r\n"
                            + "skip-grant-tables" + "\r\n";
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            FileStream fs = new FileStream(myIniPath, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            ExecuteMethod("添加my.ini成功");
            if (!Directory.Exists(url + @"\mysql\data"))
                Directory.CreateDirectory(url + @"\mysql\data");
            //解压nginx
            CompressFileAndDeleFile("nginx");
            //解压mysql
            CompressFileAndDeleFile("PEIS");

            //开启nginx服务
            string nginxPath = "";
            GetFilePath.GetFile("D://", "nginx.exe", ref nginxPath);
            ExecuteMethod("正在nginx服务");
            SetUpShortcut.CreateShortCut(nginxPath, Environment.GetFolderPath(Environment.SpecialFolder.Startup), "nginx.exe");
            ExecuteMethod("已开启nginx服务");


            //创建快捷方式
            string PeisPlatformPath = "";
            GetFilePath.GetFile("D://", "PeisPlatform.exe", ref PeisPlatformPath);
            ExecuteMethod("正在创建快捷方式");
            SetUpShortcut.CreateShortCut(PeisPlatformPath, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "新版采集端");
            ExecuteMethod("已创建快捷方式");


            //安装MySQL
            ExecuteMethod("正在安装MySQL");
            string mysqld = Cmd.ExecCommand("mysqld --initialize --user=mysql --console", mysqlBinUrl);
            if (mysqld.Contains("A temporary password is generated for root@localhost:"))
            {
                ExecuteMethod("成功获取MySQL随机密码");
            }
            else
            {
                ExecuteMethod("安装MySQL出错了");
                return;
            }
            //安装MySQL服务
            string install = Cmd.ExecCommand("mysqld --install", mysqlBinUrl);
            if (install.Contains("Service successfully installed."))
            {
                ExecuteMethod("服务成功安装");
            }
            else
            {
                ExecuteMethod("无法安装MySQL服务");
                return;
            }
            //启动MySQL服务
            string startMysql = Cmd.ExecCommand("net start mysql", mysqlBinUrl);
            if (startMysql.Contains("服务已经启动成功"))
            {
                ExecuteMethod("服务已经启动成功");
            }
            else
            {
                ExecuteMethod("无法启动MySQL服务");
                return;
            }

            //连接MySQL修改密码
            ExecuteMethod("正在设置MySQL密码");
            int i = DbHelperMySQL.ExecuteSql("update user set authentication_string=password('KunYujk1!') where user='root';");
            if (i > 0)
            {
                ExecuteMethod("设置成功");
            }
            else
            {
                ExecuteMethod("设置失败");
            }
            //添加新用户
            
            //修改配置文件
            FileStream fs1 = new FileStream(myIniPath, FileMode.Open);
            byte[] buffer = new byte[1024*1024];
            int r = fs1.Read(buffer, 0, buffer.Length);
            string str = Encoding.UTF8.GetString(buffer,0,r);
            fs1.Close();
            byte[] bytes1 = Encoding.UTF8.GetBytes(str.Substring(0,str.IndexOf("skip-grant-tables")));
            FileStream fs2 = new FileStream(myIniPath, FileMode.Create);
            fs2.Write(bytes1, 0, bytes1.Length);
            fs2.Close();

            string restartMysql = Cmd.ExecCommand("net stop mysql", mysqlBinUrl);
            restartMysql = Cmd.ExecCommand("net start mysql", mysqlBinUrl);
            if (!restartMysql.Contains("服务已经启动成功"))
            {
                ExecuteMethod("重启mysql服务失败");
            }
            //添加新用户
            DbHelperMySQL.ExecuteSql("CREATE USER 'kyjk'@'%' IDENTIFIED BY 'KunYujk1!';");
            DbHelperMySQL.ExecuteSql("GRANT ALL ON *.* TO 'kyjk'@'%';");
        }
        public void CompressFileAndDeleFile(string fileName)
        {
            fileName = fileName + ".zip";
            this.SetTextBox("正在解压" + fileName);
            compressFile.UnZip(url + "\\" + fileName + "", url);
            ExecuteMethod("解压" + fileName + "结束");

            //删除mysql.zip文件
            ExecuteMethod("正在删除" + fileName + "");
            if (File.Exists(url + "\\" + fileName + ""))
            {
                File.Delete(url + "\\" + fileName + "");
                ExecuteMethod("已删除" + fileName + "");
            }
        }
    }
}
