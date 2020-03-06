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
                Thread authThread = new Thread(new ThreadStart());
                //启动线程
                authThread.Start();
                //解压mysql
                CompressFileAndDeleFile("mysql");
                //解压nginx
                CompressFileAndDeleFile("nginx");
                //解压mysql
                CompressFileAndDeleFile("PEIS");
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
