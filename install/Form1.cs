using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System;

namespace install
{
    public partial class Form1 : Form
    {
        public static string physicalRoot = @"D:\ProgramFiles";
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           string cmd = ExecCommand("java");
            MessageBox.Show(cmd);
        }

        //安装mysql
        protected void InsertMySql()
        {
            string appPath = physicalRoot + "\\MySQL Server 5.5\\";

            //1.修改my.ini配置  为防止本机已装mysql，特修改my.ini中端口号为3307
            string iniFile = File.ReadAllText(appPath + "my.ini");
            iniFile = iniFile.Replace("%BaseDir%", physicalRoot.Replace("\\", "/")); //%BaseDir%为my.ini中自定义的目录参数
            File.WriteAllText(appPath + "my.ini", iniFile);

            Console.WriteLine("创建win服务……");
            //2.创建win服务
            string info1 = null;//ExecCommand(appPath + "bin\\mysqld.exe", " install MySQL2 --defaults-file=\"" + appPath + "my.ini\"");
            Console.WriteLine(info1);
            Thread.Sleep(3000);
            Console.WriteLine("使用net start启动服务");
            //3.启动服务
            string info2 = ExecCommand("net start MySQL2");
            Console.WriteLine(info2);

            Console.WriteLine("启动服务完成!");
            Thread.Sleep(5000);
            MySqlConnection con = new MySqlConnection("Data Source='localhost';Port='3307';Database='';User Id='root';Password='';");
            try
            {
                con.Open();
                con.Close();
                Console.WriteLine("连接成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine("连接失败！" + ex.Message);
            }
        }
        //创建数据库并初始化表
        protected void CreatDataBase()
        {
            
            string mysqlcon = "Data Source='localhost';Port='3307';Database='{0}';User Id='root';Password='';";

            MySqlConnection conn = new MySqlConnection(string.Format(mysqlcon, ""));
            FileInfo file = new FileInfo(physicalRoot + "\\DBInit\\yourDB.sql");  //filename是sql脚本文件路径。
            string sql = file.OpenText().ReadToEnd();

            try
            {
                MySqlScript script = new MySqlScript(conn);
                script.Query = sql;
                int count = script.Execute();
                Console.WriteLine("数据库初始化完成！");

                MySqlConnection con2 = new MySqlConnection(string.Format(mysqlcon, "yourDB"));
                con2.Open();
                MySqlCommand dbcom = new MySqlCommand("select count(*) from t_image", con2);
                dbcom.ExecuteScalar();
                con2.Close();
                Console.WriteLine("数据库创建OK！");

                //修改config.xml中的数据库链接地址
            }
            catch (Exception ex2)
            {
                Console.WriteLine("数据库创建失败！" + ex2.Message); 
            }


        }


        /// <summary>
        /// 执行系统CMD命令
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <returns></returns>
        public static string ExecCommand(string commandText)
        {
            Process process = new Process();//启动Process进程管理对象
            process.StartInfo.FileName = "cmd.exe";//调用cmd shell
            process.StartInfo.UseShellExecute = false;//设置是否启用操作系统的Shell启动进程
            process.StartInfo.RedirectStandardInput = true;//应用程序的输入是否从RedirectStandardInput流中获取
            process.StartInfo.RedirectStandardOutput = true;//应用程序的输出是否写到RedirectStandardOutput流中
            process.StartInfo.RedirectStandardError = true;//是否将错误信息写入到RedirectStandardError流中
            process.StartInfo.CreateNoWindow = true;//是否在新窗口中启动该进程
                                                    //p.StartInfo.WorkingDirectory = Server.MapPath("../resources/") + sessionPath;
            string strOutput = null;
            try
            {
                process.Start();
                process.StandardInput.WriteLine(commandText);
                process.StandardInput.WriteLine("exit");
                strOutput = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
            }
            catch (Exception e)
            {
                strOutput = e.Message;
               // LogHelper.WriteMessageErrorLog(e.Message);
            }
            return strOutput;
        }
    }
}
