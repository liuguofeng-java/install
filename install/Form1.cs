using System;s
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading;
using System.Windows.Forms;

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
            string info1 = CommandHelper.Execute(appPath + "bin\\mysqld.exe", " install MySQL2 --defaults-file=\"" + appPath + "my.ini\"", 0);
            Console.WriteLine(info1);
            Thread.Sleep(3000);
            Console.WriteLine("使用net start启动服务");
            //3.启动服务
            string info2 = CommandHelper.Execute("net start MySQL2", 0);
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

       
    }
}
